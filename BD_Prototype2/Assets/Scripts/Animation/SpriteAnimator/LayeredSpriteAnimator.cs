using System.Collections.Generic;
using UnityEngine;
using BulletDance.Graphics;


/****
    Seperate class containing animation methods, so that
    - Artists have a seperate workspace than programmers
    - Artists only lose direct implementations (when the animation triggers) in case merging goes wrong
    - More legible implementation names than animator.SetTrigger etc.
***/

namespace BulletDance.Animation
{

/// <summary>
/// Use if there's >1 sprite renderer layer
/// </summary>
public class LayeredSpriteAnimator : BulletDance.Animation.Animator
{
    // -- Set up -- //

    //To use animators of seperate layers
    private Dictionary<string, UnityEngine.Animator> _animators = null;

    protected override void GetReferences()
    {
        _animators = new Dictionary<string, UnityEngine.Animator>();

        foreach(Transform child in transform)
        {
            var animator = child.gameObject.GetComponent<UnityEngine.Animator>();
            if(animator != null)
                _animators.Add(child.name, animator);
        }
    }

    private UnityEngine.Animator _firstAnimator
    {
        get
        {
            int count = 0;
            foreach(var animator in _animators.Values)
            {
                if(count == 0) return animator;
            }
            return null;
        }
    }

    // -- AnimState tracker -- //
    public override int GetState()
    { 
        return _firstAnimator.GetInteger("State");
    }


    // -- Animator wrapper -- //

    protected override int GetLayerIndex(string layerName)
    {
        return _firstAnimator.GetLayerIndex(layerName);
    }

    public override void SetLayerWeight(string layerName, float weight)
    {
        foreach(var animator in _animators.Values)
        {
            animator.SetLayerWeight(GetLayerIndex(layerName), weight);
        }
    }

    /// <param name="spriteLayerName">Name of Layer/Child GameObject</param>
    public void SetLayerWeight(string spriteLayerName, string layerName, float weight)
    {
        _animators[spriteLayerName].SetLayerWeight(GetLayerIndex(layerName), weight);
    }

    public override void SetSpeed(float speed)
    {
            if (_animators != null)
                foreach (var animator in _animators.Values)
        {
            animator.speed = speed;
        }
    }

    public override float GetStatePlaybackPoint(string layerName)
    {
        int layer = GetLayerIndex(layerName);
        float totalPlaybackTime = _firstAnimator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        float loops = Mathf.Floor(totalPlaybackTime);
        return totalPlaybackTime - loops;
    }


    public override void SetTrigger(string triggerName)
    {
            if (_animators != null)
                foreach (var animator in _animators.Values)
        {
            animator.SetTrigger(triggerName);
        }
    }

    public override void SetBool(string boolName, bool value)
    {
        if(_animators != null)
        foreach(var animator in _animators.Values)
        {
            animator.SetBool(boolName, value);
        }
    } 

    public override void SetInt(string intName, int value)
    {
            if (_animators != null)
                foreach (var animator in _animators.Values)
        {
            animator.SetInteger(intName, value);
        }
    }

    public override void SetFloat(string floatName, float value)
    {
            if (_animators != null)
                foreach (var animator in _animators.Values)
        {
            animator.SetFloat(floatName, value);
        }
    }

    public float GetFloat(string floatName)
    {
        return _firstAnimator.GetFloat(floatName);
    }


    /// <param name="spriteLayerName">Name of Layer/Child GameObject</param>
    public void Anim(string spriteLayerName, int animationState)
    {
        _animators[spriteLayerName].SetInteger("State", animationState);
    }

    /// <param name="spriteLayerName">Name of Layer/Child GameObject</param>
    public void AnimIdle(string spriteLayerName)
    {
        _animators[spriteLayerName].SetInteger("State", (int) AnimAction.Idle);
    }


    /// <param name="spriteLayerName">Name of Layer/Child GameObject</param>
    public void AnimWalk(string spriteLayerName)
    {
        _animators[spriteLayerName].SetInteger("State", (int) AnimAction.Walk);
    }

    /// <param name="spriteLayerName">Name of Layer/Child GameObject</param>
    public void AnimDash(string spriteLayerName)
    {
        _animators[spriteLayerName].SetInteger("State", (int) AnimAction.Dash);
    }

    /// <param name="spriteLayerName">Name of Layer/Child GameObject</param>
    public void AnimAttack(string spriteLayerName)
    {
        _animators[spriteLayerName].SetInteger("State", (int) AnimAction.Attack);
    }

    /// <param name="spriteLayerName">Name of Layer/Child GameObject</param>
    public void AnimDefeat(string spriteLayerName)
    {
        _animators[spriteLayerName].SetInteger("State", (int) AnimAction.Defeat);
    }


    /// <param name="toggle">true: start, false: stop</param>
    public void AnimHurt(bool toggle)
    {
        SetBool("Hurt", toggle);
    }


    //Uh so it's supposed to update the animator when previewed with timeline
    //idk what the float is for, Unity source code would not tell me
    public override void UpdateInEditor(float time)
    {
        foreach(var animator in _animators.Values)
        {
            animator.Update(time);
        }
    }
}

}