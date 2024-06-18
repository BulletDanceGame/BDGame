using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace BulletDance.Cutscene
{


public class SoundBehaviour : PlayableBehaviour
{
    public AK.Wwise.Event sound;
}


[TrackClipType(typeof(SoundClip))]
[TrackColorAttribute(1.0f, 0.3f, 0.75f)]
public class SoundTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<SoundTrackMixer>.Create(graph, inputCount);
    }
}

public class SoundTrackMixer : PlayableBehaviour
{
    DialogueUI dialogueUI = DialogueUI.Instance;

    bool played = false;


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        dialogueUI = DialogueUI.Instance;

        if (dialogueUI == null) 
        { 
            Debug.Log("Dialogue UI does not exist in scene");
            return;
        }

        // Variables
        AK.Wwise.Event sound = null;

        for (int i = 0; i < playable.GetInputCount(); i++)
        {
            if (playable.GetInputWeight(i) > 0f)
            {
                ScriptPlayable<SoundBehaviour> inputPlayable = (ScriptPlayable<SoundBehaviour>)playable.GetInput(i);

                SoundBehaviour input = inputPlayable.GetBehaviour();
                sound = input.sound;
            }
        }

        if (sound == null)
        {
            played = false;
        }
        else
        {
            if (played == false)
            {
                dialogueUI.PlaySoundclip(sound);
                played = true;
            }
            
        }


    }

    //followed this tutorial https://www.youtube.com/watch?v=12bfRIvqLW4&ab_channel=GameDevGuide
}

}