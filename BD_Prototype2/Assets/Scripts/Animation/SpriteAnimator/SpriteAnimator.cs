using UnityEngine;
using UnityEngine.U2D.Animation;

using BulletDance.Graphics;


/****
    Seperate class containing animation methods, so that
    - Artists have a seperate workspace than programmers
    - Artists only lose direct implementations (when the animation triggers) in case merging goes wrong
    - More legible implementation names than animator.SetTrigger etc.
***/


namespace BulletDance.Animation
{


public class SpriteAnimator : BulletDance.Animation.Animator
{
    // -- Set up -- //
    private UnityEngine.Animator _animator;

    protected override void GetReferences()
    {
        _animator  = GetComponentInChildren<UnityEngine.Animator>();
    }


    // -- AnimState tracker -- //
    public override int GetState()
    { 
        return _animator.GetInteger("State");
    }

    // -- Animator wrapper -- //

    protected override int GetLayerIndex(string layerName)
    {
        return _animator.GetLayerIndex(layerName);
    }

    public override void SetLayerWeight(string layerName, float weight)
    {
        _animator.SetLayerWeight(GetLayerIndex(layerName), weight);
    }

    public override void SetSpeed(float speed)
    {
        _animator.speed = speed;
    }

    public override float GetStatePlaybackPoint(string layerName)
    {
        int layer = GetLayerIndex(layerName);
        float totalPlaybackTime = _animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        float loops = Mathf.Floor(totalPlaybackTime);
        return totalPlaybackTime - loops;
    }


    public override void SetTrigger(string triggerName)
    {
        _animator.SetTrigger(triggerName);
    }

    public override void SetBool(string boolName, bool value)
    {
        _animator.SetBool(boolName, value);
    } 

    public override void SetInt(string intName, int value)
    {
        _animator.SetInteger(intName, value);
    }

    public override void SetFloat(string floatName, float value)
    {
        _animator.SetFloat(floatName, value);
    }


    public override void Anim(string animationName, float playAtPoint = 0, string layerName = "")
    {
        int layer = layerName == "" ? -1 : GetLayerIndex(layerName);
        _animator.Play(animationName, layer, playAtPoint);
    }


    //Uh so it's supposed to update the animator when previewed with timeline
    //idk what the float is for, Unity source code would not tell me
    public override void UpdateInEditor(float time)
    {
        _animator.Update(time);
    }

}


}