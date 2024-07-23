using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Animation
{


public class TurretAnimator : UnitAnimator
{
    private enum TurretDirection { Homing, Front = 1, Back = 2, Left = 3, Right = 4 }

    [SerializeField]
    private TurretDirection _turretDirection;

    void Start()
    {
        if(_turretDirection != TurretDirection.Homing)
            FaceTowards((Direction)((int)_turretDirection - 1));
    }

    protected override void Update()
    {
        if(_turretDirection == TurretDirection.Homing)
            FaceTowardsPlayer(); 
    }

    protected override void AnimationStateSwitch()
    {
        if(!_continueAnimation) return;

        if(_isAlerted)
        {
            _spriteAnimator.SetTrigger("Alert");
            _isAlerted = false;
        }

        else if(_isHurted)
        {
            _spriteAnimator.SetTrigger("Hurt");
            _isHurted = false;
            _continueAnimation = false;
        }

        else
            _spriteAnimator.Anim(AnimAction.Idle);

    }

    protected override void Alerted()
    { 
        _isAlerted = true;
        _continueAnimation = true;
    }
}

}