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
            _spriteAnimator.SetTrigger("Alert");
            _isAlerted = false;
            return;
        }

        else if(_isHurted)
        {
            _spriteAnimator.SetTrigger("Hurt");
            _isHurted = false;
            _continueAnimation = false;
            return;
        }

        else if(_isDefeated)
        {
            DefeatAnim();

            _isAlerted = false;
            _isHurted = false;
            _isDefeated = false;
            _continueAnimation = false;
            return;
        }

        else if(_isAttack)
        {
            _spriteAnimator.Anim(AnimAction.Attack);
            _isAttack = false;
            return;
        }

        else
            _spriteAnimator.Anim(AnimAction.Idle);

    }

    protected override void AttackStart()
    {
        _spriteAnimator.Anim(AnimAction.Attack);
        //_isAttack = true;
    }


    protected override void Alerted()
    { 
        //_isAlerted = true;

        _spriteAnimator.SetTrigger("Alert");
        _continueAnimation = true;
    }

    protected override void Defeat()
    { 
        _spriteAnimator.AnimDefeat();
        _continueAnimation = false;
        //_isDefeated = true; 
    }

}

}