using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using BulletDance.Animation;


namespace BulletDance.Cutscene
{

[TrackClipType(typeof(PlayerClip))]
[TrackColorAttribute(1.0f, 0.0f, 0.0f)]
public class PlayerTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<PlayerTrackMixer>.Create(graph, inputCount);
    }
}

public class PlayerTrackMixer : CharacterTrackMixer
{
    PlayerAnimator player = CutsceneBinder.PlayerAnimatorInstance;
    protected override void ValidatePlayableBinding()
    {
        player = CutsceneBinder.PlayerAnimatorInstance;
        isBindingCorrect = player != null;
        characterAnim = isBindingCorrect ? player as UnitAnimator : null;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        SetPlayableBehaviourType(typeof(PlayerBehaviour));
        base.ProcessFrame(playable, info, playerData);
    }

    protected override void SetVariablesFromInspector<TTPlayableBehaviour>(TTPlayableBehaviour input)
    {
        base.SetVariablesFromInspector<CharacterBehaviour>(input as CharacterBehaviour);
        SetVariables(input as PlayerBehaviour);
    }

    private void SetVariables(PlayerBehaviour input)
    {
        spriteSet        = input.spriteSet;

        faceTowardsBoss  = input.faceTowardsBoss;

        var newAnimState = input.animState;
        if(animState != newAnimState)
        {
            requestAnimState = true;
            animState = newAnimState;
        }
    }


    //Set & display
    protected override void SetAndDisplay(Playable playable, FrameData frameData)
    {
        //implement stuff (not direction or animation) here
        if(requestUpdate)
        {
            CutsceneBinder.CutsceneEventsInstance.CS_InputToggle(enableUpdate);
            player.EnableAnimUpdate(enableUpdate);
        }

        SetSpriteSet();
        
        if(!enableUpdate)
            base.SetAndDisplay(playable, frameData);
    }


    PlayerAnimator.PlayerSpriteSet spriteSet = PlayerAnimator.PlayerSpriteSet.Default;
    protected void SetSpriteSet()
    {
        player.SetSpriteSet(spriteSet);
    }

    bool faceTowardsBoss = false;
    protected override void SetDirection()
    {
        if(faceTowardsBoss)
            player.FaceTowardsBoss();
        else if(faceTowardsTarget != null)
            player.FaceTowards(faceTowardsTarget);
        else
            player.FaceTowards(direction);
    }


    bool requestAnimState = false;
    PlayerClip.Animation animState = PlayerClip.Animation.Idle;
    protected override void Animate()
    {
        Debug.Log(animState);
        if(!requestAnimState || enableUpdate) return;
        player.AnimState((int)animState);
        requestAnimState = false;
    }

}


}