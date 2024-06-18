using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using BulletDance.Animation;


namespace BulletDance.Cutscene
{

[TrackBindingType(typeof(UnitAnimator))]
[TrackClipType(typeof(NPCClip))]
[TrackColorAttribute(1.0f, 0.75f, 0.0f)]
public class NPCTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<NPCTrackMixer>.Create(graph, inputCount);
    }
}

public class NPCTrackMixer : CharacterTrackMixer
{
    protected override void ValidatePlayableBinding()
    {
        isBindingCorrect = characterAnim != null;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        characterAnim = playerData == null ? null : playerData as UnitAnimator;
        SetPlayableBehaviourType(typeof(NPCBehaviour));
        base.ProcessFrame(playable, info, playerData);
    }

    protected override void SetVariablesFromInspector<TTPlayableBehaviour>(TTPlayableBehaviour input)
    {
        base.SetVariablesFromInspector<CharacterBehaviour>(input as CharacterBehaviour);
        SetVariables(input as NPCBehaviour);
    }

    private void SetVariables(NPCBehaviour input)
    {
        showGraphics = input.showGraphics;
        faceTowardsPlayer = input.faceTowardsPlayer;

        var newAnimState = input.animState;
        requestAnimState = animState != newAnimState;
        if(requestAnimState) animState = newAnimState;
    }


    //Set & display
    bool showGraphics = true;
    protected override void SetAndDisplay(Playable playable, FrameData frameData)
    {
        //implement stuff (not direction or animation) here
        characterAnim.gameObject.SetActive(showGraphics);
        if(!showGraphics) return;

        base.SetAndDisplay(playable, frameData);
    }


    bool faceTowardsPlayer = false;
    protected override void SetDirection()
    {
        if(faceTowardsPlayer)
            characterAnim.FaceTowardsPlayer();
        else if(faceTowardsTarget != null)
            characterAnim.FaceTowards(faceTowardsTarget);
        else
            characterAnim.FaceTowards(direction);
    }


    bool requestAnimState = false;
    NPCClip.Animation animState = NPCClip.Animation.Idle;
    protected override void Animate()
    {
        if(!requestAnimState) return;
        characterAnim.AnimState((int)animState);
        requestAnimState = false;
    }

}


}