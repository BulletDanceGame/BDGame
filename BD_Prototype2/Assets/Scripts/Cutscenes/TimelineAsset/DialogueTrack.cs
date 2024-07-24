using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace BulletDance.Cutscene
{


public class DialogueBehaviour : PlayableBehaviour
{
    public DialogueClip.Speaker speaker;
    public DialogueUI.Speaker speakerPosition;
    public Sprite characterArt;
    public string dialogueText;
    public float speedOfLetters;
    public AK.Wwise.Event voiceline;
}



[TrackClipType(typeof(DialogueClip))]
[TrackColorAttribute(0.75f, 0.3f, 1.0f)]
public class DialogueTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<DialogueTrackMixer>.Create(graph, inputCount);
    }
}

public class DialogueTrackMixer : PlayableBehaviour
{
    DialogueUI dialogueUI = DialogueUI.Instance;

    string savedText = "";
    string displayedText = "";
    string richTag = "", closeTag = "";

    DialogueClip.Speaker speaker = DialogueClip.Speaker.Player;

    int charIndex = 0;
    float timer = 0;
    float speed = 0;
    bool updateLetters = false;

    bool voicePlayed = false;


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        dialogueUI = DialogueUI.Instance;

        if (dialogueUI == null) 
        { 
            Debug.Log("Dialogue UI does not exist in scene");
            return;
        }


        //Variables
        DialogueUI.Speaker speakPos = DialogueUI.Speaker.LEFT;
        Sprite sprite      = null;
        string currentText = "";
        AK.Wwise.Event voiceline = null;


        //Get the variables set in DialogueClip (DialogueBehaviour)
        for (int i = 0; i < playable.GetInputCount(); i++)
        {
            if (playable.GetInputWeight(i) > 0f)
            {
                ScriptPlayable<DialogueBehaviour> inputPlayable = (ScriptPlayable<DialogueBehaviour>)playable.GetInput(i);
                DialogueBehaviour input = inputPlayable.GetBehaviour();

                speaker  = input.speaker;
                speakPos = input.speakerPosition;
                sprite   = input.characterArt;

                currentText = input.dialogueText;
                speed = input.speedOfLetters;

                voiceline = input.voiceline;
            }
        }


        //Set & display
        DisplayUI(currentText);
        DisplayImage(sprite, speakPos);
        DisplayText(currentText);
        PlayVoiceline(voiceline);
    }


    private void DisplayUI(string currentText)
    {
        bool isTextNotEmpty = currentText.ToCharArray().Length != 0;
        dialogueUI.SetVisible(isTextNotEmpty);
        dialogueUI.SetDialBoxActive(isTextNotEmpty);
    }

    private void DisplayImage(Sprite sprite, DialogueUI.Speaker speakPos)
    {
        if (sprite == null)
            dialogueUI.DeactivateImage();
        else
            dialogueUI.SetImage(speakPos, sprite);
    }

    private void DisplayText(string currentText)
    {
        //Determine if typewriter effect should happen
        if (currentText != savedText)
        {
            savedText = currentText;
            displayedText = "";
            richTag    = "";
            closeTag   = "";

            charIndex = 0;

            if (currentText.ToCharArray().Length > 0)
                updateLetters = true;
        }

        if (charIndex == currentText.ToCharArray().Length || currentText.ToCharArray().Length == 0)
            updateLetters = false;


        //Typewriter effect
        if (updateLetters)
        {
            timer -= Time.timeScale >= 1f ? Time.deltaTime : Time.unscaledDeltaTime;

            if (timer <= 0)
            {
                bool tagDectected = DetectRichTextTag(currentText, charIndex, out charIndex);

                if(tagDectected)
                {
                    if(richTag.ToCharArray().Length != 0)
                    {
                        displayedText += richTag;
                        richTag = "";
                    }

                    if(closeTag.ToCharArray().Length != 0)
                    {
                        displayedText += closeTag;
                        closeTag = "";
                    }

                    charIndex++;
                }

                displayedText += currentText.ToCharArray()[charIndex];
                charIndex++;
                timer = speed;

                dialogueUI.PlayTextletter(speaker.ToString());
            }
        }

        dialogueUI.SetText(displayedText);
    }

    bool DetectRichTextTag(string currentText, int charIndex, out int newCharIndex)
    {
        //Detecting opening <
        if(currentText.ToCharArray()[charIndex] != Convert.ToChar("<"))
        {
            newCharIndex = charIndex;
            return false;
        }

        int tagCharIndex = charIndex;
        bool closingTag = false;
        string currentTag = "";

        //Store tag
        while(true)
        {
            //Detected closing tag
            if(currentText.ToCharArray()[tagCharIndex] == Convert.ToChar("/"))
                closingTag = true;

            currentTag += currentText.ToCharArray()[tagCharIndex];

            //Escape when current char is >
            if(currentText.ToCharArray()[tagCharIndex] == Convert.ToChar(">"))
                break;

            tagCharIndex++;
        }

        //Set the tags
        if(!closingTag)
            richTag = currentTag;
        else
            closeTag = currentTag;
        
        //Set current index to tag index
        newCharIndex = tagCharIndex;

        return true;
    }

    private void PlayVoiceline(AK.Wwise.Event voiceline)
    {
        if (voiceline == null)
            voicePlayed = false;
        else
        {
            if (voicePlayed == false)
            {
                voicePlayed = true;
                dialogueUI.PlayVoiceline(voiceline);
            }
        }
    }

    //followed this tutorial https://www.youtube.com/watch?v=12bfRIvqLW4&ab_channel=GameDevGuide
}


}