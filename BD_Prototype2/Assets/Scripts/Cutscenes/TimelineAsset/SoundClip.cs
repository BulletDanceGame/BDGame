using UnityEngine;
using UnityEngine.Playables;

namespace BulletDance.Cutscene
{


public class SoundClip : PlayableAsset
{
    public AK.Wwise.Event sound;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        ScriptPlayable<SoundBehaviour> playable = ScriptPlayable<SoundBehaviour>.Create(graph);

        SoundBehaviour soundBehaviour = playable.GetBehaviour();
        soundBehaviour.sound = sound;

        return playable;
    }
}

}