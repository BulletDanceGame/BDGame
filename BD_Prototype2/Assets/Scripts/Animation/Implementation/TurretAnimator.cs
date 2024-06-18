using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Animation
{


public class TurretAnimator : UnitAnimator
{
    protected override void AnimationStateSwitch()
    {
        if(_isAlerted)
        {
            _spriteAnimator.Anim("activated");
            _isAlerted = false;
        }

        if(_isHurted)
        {
            _spriteAnimator.Anim("shield");
            _isHurted = false;
        }

        else if(_isAttack)
            _spriteAnimator.Anim("shoot");
        
        else
            _spriteAnimator.Anim("idle");

    }

    protected override void Defeat()
    {
        _spriteAnimator.Anim("deactivate");
    }
}

}