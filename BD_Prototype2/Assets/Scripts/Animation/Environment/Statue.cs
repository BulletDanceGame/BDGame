using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Animation
{


public class Statue : RhythmAnimator
{
    public override void PlayAnimation(int anticipation, float duration)
    {
        if(_spriteAnimator == null) return;

        _beatDuration = duration;
        _duration = GetDuration(_beatDuration, _animNoteDuration);

        //Anticipation = beats in the future
        //This default method will play animations in the current beat (anticipation = 0)
        if(anticipation != 0) return;

        _spriteAnimator.SetSpeed(1f / _duration);
        SwitchAnimation();
    }

    private bool _isActivated = false;
    private bool _finishedActivation = false;

    void SwitchAnimation()
    {
        if(!_isActivated) return;

        if(_finishedActivation)
            _spriteAnimator.Anim(AnimAction.Idle);
        else
            _spriteAnimator.Anim(1);

        _finishedActivation = true;
    }

    void OnTriggerEnter2D(Collider2D cld)
    {
        if(cld.tag != "Player") return;
        if(_isActivated) return;
        if(_spriteAnimator == null) return;

        _isActivated = true;
    }

}


}
