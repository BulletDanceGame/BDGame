using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace BulletDance.Animation
{


public class BossCritterAnimator : BossAnimator
{
    protected override void WalkStart()
    { 
        _isWalk = true;

        //Play a faster move animation if the movement happens offbeat.

        if(_offbeatDuration > 100f) return; //Not offbeat (no beat or offbeat is smaller than minimum duration)

        float speed = 1f / GetDuration(_offbeatDuration, _animNoteDuration);
        if(speed > 4f) return; //The animation is too fast, don't animate

        _spriteAnimator.SetSpeed(speed);
        _spriteAnimator.Anim(AnimAction.Walk);
    }

    protected override void SpecialStart(int actionState) 
    {
        base.SpecialStart(actionState);
        //if(actionState == 46 || actionState == 48)
            _spriteAnimator.Anim(actionState);
    }


    public override void PlayAnimation(int anticipation, float duration)
    {
        //Cutscene override
        if(!_continueAnimation) return;
        base.PlayAnimation(anticipation, duration);
    }


    protected override void Update()
    {
        //Cutscene override
        if(!_continueAnimation) 
        {
            PhaseChange();
            return;
        }

        if(EditorCheck.inEditMode) return;

        base.Update();
    }


    [SerializeField]
    private ParticleSystem[] _screamVFX;
    public void Scream()
    {
        ScreenShake.Instance.ShakeCamera(20, 1.7f);
        foreach(ParticleSystem vfx in _screamVFX)
        {
            vfx.Play();
        }
    }

    [SerializeField]
    private ParticleSystem[] _dustVFX;
    public void LandVFX()
    {
        ScreenShake.Instance.ShakeCamera(20f, 0.4f);
        foreach(ParticleSystem vfx in _dustVFX)
        {
            vfx.Play();
        }
    }
}


}
