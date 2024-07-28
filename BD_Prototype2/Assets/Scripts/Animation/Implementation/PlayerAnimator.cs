using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Animation
{

public class PlayerAnimator : UnitAnimator
{
    // -- Set up -- //

    protected override void OnEnable()
    {
        base.OnEnable();
        SubscribePlayerEvents();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UnsubscribePlayerEvents();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnsubscribePlayerEvents();
    }

    void SubscribePlayerEvents()
    {
        if(EventManager.Instance == null) return;

        EventManager.Instance.OnPlayerDeath  += Defeat;
        EventManager.Instance.OnPlayerDamage += Damage;

        EventManager.Instance.OnPlayerDash   += Dash;
        EventManager.Instance.OnPlayerAttack += Attack;

        EventManager.Instance.OnPlayerHitBullet += AttackAfterImage;
        EventManager.Instance.OnPlayerLastHit   += FreezeFrame;
    }

    void UnsubscribePlayerEvents()
    {
        if(EventManager.Instance == null) return;

        EventManager.Instance.OnPlayerDamage -= Damage;
        EventManager.Instance.OnPlayerDeath  -= Defeat;

        EventManager.Instance.OnPlayerDash   -= Dash;
        EventManager.Instance.OnPlayerAttack -= Attack;

        EventManager.Instance.OnPlayerHitBullet -= AttackAfterImage;
        EventManager.Instance.OnPlayerLastHit   -= FreezeFrame;
    }


    // -- Update -- //
    //Fucking bullshit ass StateMachineBehaviour OnStateExit don't work as expected
    //  so i'm resetting the state manually here
    int _state { get { return _spriteAnimator.animState; } }
    void ResetState()
    {
        if(_state == (int)AnimAction.Idle || _state == (int)AnimAction.Defeat ||
           _state == (int)PlayerAnimAction.WakeUp)
            return;

        string animLayer = _state > (int)AnimAction.Walk ? "Act" : "Walk";
        float playbackPoint = _spriteAnimator.GetStatePlaybackPoint("Sprite " + animLayer);

        //Animator update is not synced with regular Update
        //   so if we check for the playback point in this script, it would not be at 1
        //This value returns the most consistent result so i'm sticking with that
        if(playbackPoint < 0.9) return;

        _spriteAnimator.SetLayerWeight("Sprite Act", 0);
        //_spriteAnimator.SetLayerWeight("Hair Act", 0);

        int isWalkState = _isWalk ? 1 : 0;
        _spriteAnimator.SetState(isWalkState);
        _spriteAnimator.SetLayerWeight("Sprite Walk", isWalkState);
        //_spriteAnimator.SetLayerWeight("Hair Walk", isWalkState);
    }

    protected override void Update() 
    {
        _spRdr?.material.SetFloat("_OutlineThickness", outlineThickness);

        if(!_continueAnimation)
        {
            //Debug.Log("Unit animator is turned off");
            return;
        }

        ResetState();
        if(_state > (int)AnimAction.Walk) return;

        float _walkLayerVisibility = _isWalk ? 1f : 0f;
        _spriteAnimator.SetLayerWeight("Sprite Walk", _walkLayerVisibility);
        //_spriteAnimator.SetLayerWeight("Hair Walk",   _walkLayerVisibility);

        if(_isWalk)
            _spriteAnimator.Anim((int)AnimAction.Walk);
        else
            _spriteAnimator.Anim((int)AnimAction.Idle);
    }

    protected override void AnimationStateSwitch()
    {
        if(!_continueAnimation)
        {
            Debug.Log("Unit animator is turned off");
            return;
        }

        if(_isHurted)
        {
            HurtAnim();
            _isHurted = false;
        }

        if(_isDefeated)
        {
            DefeatAnim();
            _isDefeated = false;
        }

        if(_state == (int)AnimAction.Dash || _state == (int)AnimAction.Attack) 
            return;

        if(_isWalk)
            _spriteAnimator.Anim(AnimAction.Walk);
        else
            _spriteAnimator.Anim(AnimAction.Idle);
    }


    // -- Direction -- //
    private static readonly Dictionary<(int x, int y), Direction> DirectionLookup = new Dictionary<(int x, int y), Direction>
    {
        {( 0, -1), Direction.Front},
        {( 0,  1), Direction.Back},
        {(-1,  0), Direction.Left},
        {( 1,  0), Direction.Right},
        {(-1, -1), Direction.Left},//Down},
        {(-1,  1), Direction.Left},//Up},
        {( 1, -1), Direction.Right},//Down},
        {( 1,  1), Direction.Right}//Up}
    };


    protected override void SetAnimationDirection(Direction direction)
    {
        base.SetAnimationDirection(direction);
        //_spriteAnimator.SetFloat("HairDirection", HairDirectionLookup[direction]);
    }


    // -- Actions -- //
    [SerializeField]
    private Rigidbody2D _rbd;
    [SerializeField]
    private float _runThreshold = 0.1f;
    private bool _isMoving, _isPrevMoving;
    void LateUpdate()
    {
        if(_rbd == null)
        {
            Debug.LogWarning("Player won't play movement animations because rigidbody is not referenced");
            return;
        }

        //if(!_continueAnimation)
        //{
        //    Debug.LogWarning("Unit animator is turned off");
        //    return;
        //}

        Vector3 direction = _rbd.velocity;

        //Round-int the velocity vector
        int x = direction.x > _runThreshold  ?  1 : 
                direction.x < -_runThreshold ? -1 : 0;
        int y = direction.y > _runThreshold  ?  1 : 
                direction.y < -_runThreshold ? -1 : 0;

        _isMoving = !(x == 0 && y == 0);

        if(_state != (int)AnimAction.Dash && _state != (int)AnimAction.Attack) 
        {
            //Only change facing direction when moving
            if(_isMoving)
            {
                SetAnimationDirection(GetAnimationDirectionTowards(new Vector2(x, y)));
                WalkStart();
            }
            else
                WalkStop();

            //Play offbeat walking animation when player idle->moves
            if(_isPrevMoving != _isMoving && _isMoving)
                OffbeatWalk();
        }

        _isPrevMoving = _isMoving;
    }


    //Play a faster move animation if the movement happens offbeat.
    void OffbeatWalk()
    {
        if(!_continueAnimation)
        {
            Debug.LogWarning("Unit animator is turned off");
            return;
        }

        if(_offbeatDuration > 100f) return; //Not offbeat (no beat or offbeat is smaller than minimum duration)
        if(_state == (int)AnimAction.Dash || _state == (int)AnimAction.Attack) 
            return;

        float playAtPoint = (_beatDuration - _offbeatDuration)/_beatDuration;
        _spriteAnimator.Anim("Walk", playAtPoint, "Sprite Walk");
        _spriteAnimator.Anim("Walk", playAtPoint, "Hair Walk");
        _spriteAnimator.SetSpeed(1f / GetDuration(_beatDuration, NoteDuration.Half));
        _spriteAnimator.SetLayerWeight("Sprite Walk", 1);
        //_spriteAnimator.SetLayerWeight("Hair Walk",   1);
    }


    void Dash(BeatTiming hitTiming, Vector2 direction)
    {
        if(!_continueAnimation)
        {
            Debug.LogWarning("Unit animator is turned off");
            return;
        }

        if(hitTiming == BeatTiming.BAD) return;

        SetAnimationDirection(GetAnimationDirectionTowards(direction));

        _spriteAnimator.Anim(AnimAction.Dash);
        _spriteAnimator.SetLayerWeight("Sprite Act", 1);
        //_spriteAnimator.SetLayerWeight("Hair Act",   1);
        //_spriteAnimator.SetSpeed(1f / GetDuration(_beatDuration, NoteDuration.Sixteenth));
    }

    void Attack(BeatTiming hitTiming, Vector2 direction)
    {
        if(!_continueAnimation)
        {
            Debug.LogWarning("Unit animator is turned off");
            return;
        }

        SetAnimationDirection(GetAnimationDirectionTowards(direction));

        _spriteAnimator.Anim(AnimAction.Attack);
        _spriteAnimator.SetLayerWeight("Sprite Act", 1);
        //_spriteAnimator.SetLayerWeight("Hair Act",   1);
        //_spriteAnimator.SetSpeed(1f / GetDuration(_beatDuration, NoteDuration.Sixteenth));

        _attackDir = direction;
    }

    [SerializeField]
    private GameObject _perfectVFXPrefab;
    [SerializeField]
    private SpriteRenderer _spRdr;
    private Vector2 _attackDir;
    public float outlineThickness;

    void AttackAfterImage(BeatTiming hitTiming)
    {
        if(hitTiming != BeatTiming.PERFECT) return;

        //_spriteAnimator?.SetTrigger("Perfect");

        var vfx = Instantiate(_perfectVFXPrefab, transform.position, Quaternion.identity).GetComponent<BulletDance.VFX.PlayerPerfectVFX>();
        vfx.AttackAfterImage(_spRdr.sprite, _attackDir);
    }



    void Damage(float none)
    {
        Hurt();
    }


    private enum PlayerAnimAction { 
        WakeUp = 10, 
        SoulFormTransform = 30, SoulFormDetransform = 31 };


    // -- For cutscene -- //
    public enum PlayerSpriteSet { Default, NoBat, SoulForm }
    public string _spriteSet = "Default";
    [ExecuteAlways]
    public void SetSpriteSet(PlayerSpriteSet spriteSet)
    {
        switch(spriteSet)
        {
            case PlayerSpriteSet.NoBat:    _spriteSet = "NoBat";            break;
            case PlayerSpriteSet.SoulForm: _spriteSet = "layer1SoulForm";   break;

            default:   _spriteSet = "Default";   break;
        }

        _spriteAnimator?.SetLibraryByName(_spriteSet);
    }

    [ExecuteAlways]
    public override void EnableAnimUpdate(bool enable)
    {
        base.EnableAnimUpdate(enable);
        if(!enable) Animate((int)AnimAction.Idle);
    }

    [ExecuteAlways]
    protected override void Animate(int actionState)
    {
        base.Animate(actionState);
        
        float isWalkState = actionState == (int)AnimAction.Walk ? 1 : 0;
        _spriteAnimator?.SetLayerWeight("Sprite Walk", isWalkState);
        //_spriteAnimator?.SetLayerWeight("Hair Walk",   isWalkState);
        
        float isActState  = actionState == (int)AnimAction.Dash || actionState == (int)AnimAction.Attack ?
                            1 : 0;
        _spriteAnimator?.SetLayerWeight("Sprite Act", isActState);
        //_spriteAnimator?.SetLayerWeight("Hair Act",   isActState);
    }

    void FreezeFrame(BeatTiming none)
    {
        if (BossController.Instance && BossController.Instance.bossHealth.isLastPhase)
        {
            _spriteAnimator?.SetSpeed(0.0000000001f); //Copy paste from Playerswingbox
            Invoke("UnfreezeFrame", 5f); //Copy paste from Playerswingbox
        }
    }

    void UnfreezeFrame()
    {
        _duration = GetDuration(_beatDuration, _animNoteDuration);
        _spriteAnimator?.SetSpeed(1f / _duration);
    }

    [SerializeField]
    private ParticleSystem[] _pickupVFX;
    public void PickUp()
    {
        foreach(ParticleSystem vfx in _pickupVFX)
        {
            vfx.Play();
        }
    }
}

}