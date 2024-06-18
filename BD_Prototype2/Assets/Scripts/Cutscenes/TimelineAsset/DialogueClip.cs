using UnityEngine;
using UnityEngine.Playables;

namespace BulletDance.Cutscene
{


public class DialogueClip : PlayableAsset
{
    public enum Speaker { Player, YokaiHunter };
    public Speaker speaker;

    public DialogueUI.Speaker speakerPosition;
    public Sprite characterArt;

    [TextArea(3,10)]
    public string dialogueText;
    public float speedOfLetters;

    public AK.Wwise.Event voiceline;


    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        ScriptPlayable<DialogueBehaviour> playable = ScriptPlayable<DialogueBehaviour>.Create(graph);
        DialogueBehaviour DialogueBehaviour = playable.GetBehaviour();

        DialogueBehaviour.speaker = speaker;
        
        DialogueBehaviour.speakerPosition = speakerPosition;
        DialogueBehaviour.characterArt    = characterArt;

        DialogueBehaviour.dialogueText   = dialogueText;
        DialogueBehaviour.speedOfLetters = speedOfLetters;

        DialogueBehaviour.voiceline = voiceline;

        return playable;
    }

}


}