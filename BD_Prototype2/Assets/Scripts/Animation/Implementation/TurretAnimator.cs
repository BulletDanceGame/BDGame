using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Animation
{


public class TurretAnimator : UnitAnimator
{
    private enum TurretDirection { Homing, Front = 1, Back = 2, Left = 3, Right = 4, Oscillating = 5}

    [SerializeField]
    private TurretDirection _turretDirection;

    [SerializeField] 
    private Transform _direction;


    void Start()
    {
        _continueAnimation = false;

        if(_turretDirection != TurretDirection.Homing)
            FaceTowards((Direction)((int)_turretDirection - 1));
    }

    protected override void Update()
    {
        if(!_continueAnimation) return;

        if(_turretDirection == TurretDirection.Homing)
            FaceTowardsPlayer(); 
        if(_turretDirection == TurretDirection.Oscillating)
            _spriteAnimator.SetDirection(GetAnimationDirectionTowards(_direction.up));
    }

    protected override void AnimationStateSwitch()
    {
        if(!_continueAnimation) return;

        if(_isAlerted)
        {
            _isAlerted = false;
            return;
        }

        if(_isAttack) 
        {
            _isAttack = false;
            _spriteAnimator.Anim(AnimAction.Idle);
        }
    }

    protected override void AttackStart()
    {
        if(!_continueAnimation) return;

        _spriteAnimator.Anim(AnimAction.Attack);
        _isAttack = true;
    }


    protected override void Alerted()
    { 
        if(_continueAnimation) return;

        _isAlerted = true;

        _spriteAnimator.SetLayerWeight("HURTBRUH", 0f);
        _spriteAnimator.SetTrigger("Alert");
        _continueAnimation = true;
    }

    protected override void Hurt() 
    { 
        if(!_continueAnimation) return;

        //_isHurted = true;

        _spriteAnimator.SetTrigger("Hurt");
        _spriteAnimator.SetLayerWeight("HURTBRUH", 1f);
        _continueAnimation = false;
    }

    protected override void Defeat()
    { 
        if(!_continueAnimation) return;

        //_isDefeated = true; 

        _spriteAnimator.AnimDefeat();
        _continueAnimation = false;
    }

}

}