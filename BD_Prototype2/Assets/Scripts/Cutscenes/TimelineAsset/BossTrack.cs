using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using BulletDance.Animation;


namespace BulletDance.Cutscene
{

[TrackClipType(typeof(BossClip))]
[TrackColorAttribute(1.0f, 0.5f, 0.0f)]
public class BossTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<BossTrackMixer>.Create(graph, inputCount);
    }
}

public class BossTrackMixer : CharacterTrackMixer
{
    BossAnimator boss = CutsceneBinder.BossAnimatorInstance;
    protected override void ValidatePlayableBinding()
    {
        boss = CutsceneBinder.BossAnimatorInstance;
        isBindingCorrect = boss != null;
        characterAnim = isBindingCorrect ? boss as UnitAnimator : null;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        SetPlayableBehaviourType(typeof(BossBehaviour));
        base.ProcessFrame(playable, info, playerData);
    }

    protected override void SetVariablesFromInspector<TTPlayableBehaviour>(TTPlayableBehaviour input)
    {
        base.SetVariablesFromInspector<CharacterBehaviour>(input as CharacterBehaviour);
        SetVariables(input as BossBehaviour);
    }

    private void SetVariables(BossBehaviour input)
    {
        showGraphics = input.showGraphics;
        faceTowardsPlayer = input.faceTowardsPlayer;
        bossphase = input.bossphase;

        var newAnimState = input.animState;
        requestAnimState = animState != newAnimState;
        if(requestAnimState) animState = newAnimState;
    }


    //Set & display
    bool showGraphics = true;
    protected override void SetAndDisplay(Playable playable, FrameData frameData)
    {
        //implement stuff (not direction or animation) here
        if(requestUpdate)
        {
            if(!EditorCheck.inEditMode)
            {
                var walkingController = boss.gameObject.transform.parent.GetComponent<BossWalkingController>();
                if(walkingController != null)
                    walkingController.enabled = enableUpdate;
            }

            boss.EnableAnimUpdate(enableUpdate);
        }

        boss.gameObject.SetActive(showGraphics);
        if(!showGraphics) return;

        base.SetAndDisplay(playable, frameData);
    }


    bool faceTowardsPlayer = false;
    protected override void SetDirection()
    {
        if(faceTowardsPlayer)
            boss.FaceTowardsPlayer();
        else if(faceTowardsTarget != null)
            boss.FaceTowards(faceTowardsTarget);
        else
            boss.FaceTowards(direction);
    }


    bool requestAnimState = false;
    int  bossphase = 1;
    BossClip.Animation animState = BossClip.Animation.Idle;
    protected override void Animate()
    {
        if(!requestAnimState) 
        {
            boss.CutsceneAnimationLogicUpdate();
            return;
        }

        boss.SetPhaseGraphics(bossphase);
        boss.AnimState((int)animState);
        boss.CutsceneAnimationLogicUpdate();
        requestAnimState = false;
    }

}


}