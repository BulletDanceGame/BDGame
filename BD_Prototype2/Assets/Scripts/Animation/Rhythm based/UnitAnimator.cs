using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Animation
{


/****
    Base Class for animating any "unit" entity
    includes player, boss, enemies, and npc
    or anything that walks, dashes, attacks, or have defeated animation
***/


[RequireComponent(typeof(UnitAnimationHandler))]
public class UnitAnimator : RhythmAnimator
{
    protected UnitAnimationHandler _animHandler;

    // -- Start && Event subscriptions -- //
    protected virtual void Awake()
    {
        //Disable component if animator not found
        if(_animator == null && _spriteAnimator == null && _layeredAnimator == null)
        {
            this.enabled = false;
            return;
        }

        _animHandler = GetComponent<UnitAnimationHandler>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SubscribeEvents();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UnsubscribeEvents();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnsubscribeEvents();
    }

    protected void SubscribeEvents()
    {
        if(_animHandler == null) return;

        _animHandler.OnAlerted += Alerted;

        _animHandler.OnHurt   += Hurt;
        _animHandler.OnDefeat += Defeat;

        _animHandler.OnWalkStart += WalkStart;
        _animHandler.OnWalkStop  += WalkStop;

        _animHandler.OnDashStart += DashStart;
        _animHandler.OnDashStop  += DashStop;

        _animHandler.OnAttackStart += AttackStart;
        _animHandler.OnSpecialStart     += SpecialStart;
            _animHandler.OnSpecialStop += SpecialStop;

            _animHandler.OnPhaseChange         += PhaseChangeStart;
        _animHandler.OnPhaseChangeFinished += PhaseChangeFinished;
    }

    protected void UnsubscribeEvents()
    {
        if(_animHandler == null) return;

        _animHandler.OnAlerted -= Alerted;

        _animHandler.OnHurt   -= Hurt;
        _animHandler.OnDefeat -= Defeat;

        _animHandler.OnWalkStart -= WalkStart;
        _animHandler.OnWalkStop  -= WalkStop;

        _animHandler.OnDashStart -= DashStart;
        _animHandler.OnDashStop  -= DashStop;

        _animHandler.OnAttackStart -= AttackStart;
            _animHandler.OnSpecialStart -= SpecialStart;
            _animHandler.OnSpecialStop -= SpecialStop;


            _animHandler.OnPhaseChange         -= PhaseChangeStart;
        _animHandler.OnPhaseChangeFinished -= PhaseChangeFinished;
    }


    // -- Update -- //
    public override void PlayAnimation(int anticipation, float duration)
    {
        if(anticipation != 0) return;

        _beatDuration = duration;
        _duration = GetDuration(_beatDuration, _animNoteDuration);

        _spriteAnimator?.SetSpeed(1f / _duration);
        _layeredAnimator?.SetSpeed(1f / _duration);
        
        AnimationStateSwitch();
    }

    [ExecuteAlways]
    protected virtual void Update()
    {
        FaceTowardsPlayer();
    }

    protected virtual void AnimationStateSwitch()
    {
        if(!_continueAnimation)
        {
            Debug.Log("Unit animator is turned off via defeat");
            return;
        }

        if(_isAlerted)
        {
            AlertAnim();
            _isAlerted = false;
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

        if(!_isWalk && !_isDash && !_isAttack && !_isSpecial)
        {
            if(_animType == AnimatorType.Sprite)
                _spriteAnimator.Anim(AnimAction.Idle);
            else
                Debug.Log("Unimplemented idle animation for UnitAnimator type " + _animType);
        }
        
        if(_isWalk&& !_isSpecial)
        {
            if(_animType == AnimatorType.Sprite)
                _spriteAnimator.Anim(AnimAction.Walk);

            else
                Debug.Log("Unimplemented walk animation for UnitAnimator type " + _animType);
        }

        if(_isSpecial)
            {
                if (_animType == AnimatorType.Sprite)
                    _spriteAnimator.Anim(_specialState);

                if (_animType == AnimatorType.Layered)
                    _layeredAnimator.Anim(_specialState);
            }
        }


    // -- Direction -- //
    protected Direction _animDirection = 0;

    //For dash
    protected Dictionary<Vector2, Direction> _directionLookup = new Dictionary<Vector2, Direction>
    {
        {Vector2.up,    Direction.Back},
        {Vector2.down,  Direction.Front},
        {Vector2.left,  Direction.Left},
        {Vector2.right, Direction.Right},
    };


    protected virtual void SetAnimationDirection(Direction direction)
    {
        _spriteAnimator?.SetDirection(direction);
        _layeredAnimator?.SetDirection(direction);
    }

    protected void SetAnimationDirectionTowards(Vector2 position)
    {
        SetAnimationDirection(GetAnimationDirectionTowards(position - (Vector2)transform.position));
    }

    protected Direction GetAnimationDirectionTowards(Vector2 position)
    {
        float _angleToTarget = Vector2.SignedAngle(position, Vector2.up);

        if (_angleToTarget < -35f && _angleToTarget > -145f)
            return Direction.Left;

        else if(_angleToTarget < 145f && _angleToTarget > 35f)
            return Direction.Right;

        else if(_angleToTarget <= -145f || _angleToTarget >= 145f)
            return Direction.Front;

        else
            return Direction.Back;
    }


    // -- Common animations -- //
    protected bool _isAlerted = false;
    protected virtual void Alerted() { _isAlerted = true; }
    protected virtual void AlertAnim()
    {
        if(_animType == AnimatorType.Sprite)
        {
            //_spriteAnimator.SetFloat("AlertSpeed", 1f / GetDuration(_beatDuration, NoteDuration.Eighth));
            _spriteAnimator.SetTrigger("Alert");
        }

        if(_animType == AnimatorType.Layered)
        {
            //_layeredAnimator.SetFloat("AlertSpeed", 1f / GetDuration(_beatDuration, NoteDuration.Eighth));
            _layeredAnimator.SetTrigger("Alert");
        }
    }


    protected bool _isHurted = false;
    protected virtual void Hurt() { _isHurted = true; }
    protected virtual void HurtAnim()
    {
        if(_animType == AnimatorType.Sprite)
        {
            //_spriteAnimator.SetFloat("HurtSpeed", 1f / GetDuration(_beatDuration, NoteDuration.Half));
            _spriteAnimator.AnimHurt();
        }

        if(_animType == AnimatorType.Layered)
        {
            //_layeredAnimator.SetFloat("HurtSpeed", 1f / GetDuration(_beatDuration, NoteDuration.Half));
            _layeredAnimator.AnimHurt();
        }
    }

    protected bool _continueAnimation = true;
    protected bool _isDefeated = false;
    protected virtual void Defeat() { _isDefeated = true; }
    protected virtual void DefeatAnim()
    {
        if(_animType == AnimatorType.Sprite)
            _spriteAnimator.AnimDefeat();

        if(_animType == AnimatorType.Layered)
            _layeredAnimator.AnimDefeat();

        _continueAnimation = false;
    }

    protected bool _isWalk = false;
    protected virtual void WalkStart(){ _isWalk = true; }
    protected virtual void WalkStop() { _isWalk = false; }

    protected bool _isDash = false;
    protected virtual void DashStart(Vector2 direction){}
    protected virtual void DashStop(){}

    protected bool _isAttack = false;
    protected virtual void AttackStart(){}

    protected int  _phase = 1;
    protected bool _isPhaseChange = false;
    protected virtual void PhaseChangeStart(int phase){}
    protected virtual void PhaseChangeFinished(int phase){}

        protected bool _isSpecial = false;
        protected int _specialState = 0;
        protected virtual void SpecialStart(int actionState) 
        {
            _isSpecial = true;
            _specialState = actionState;
        }

        protected virtual void SpecialStop(){ _isSpecial = false;  }


        // -- For cutscenes -- //
        // Excecute always for editor preview

        [ExecuteAlways]
    protected bool CheckCompnents()
    {
        if(!EditorCheck.inEditMode) return true;

        bool success = false;

        if(_animType == AnimatorType.Sprite)
        {
            success = EditorCheck.GetComponentInEditMode<SpriteAnimator>(
                                ref _spriteAnimator, (() => { return GetComponent<SpriteAnimator>(); }));
            if(_spriteAnimator == null && !success) 
            {
                Debug.Log("Cannot get Sprite Animator in edit mode");
                return false;
            }

            return true;
        }

        else if(_animType == AnimatorType.Layered)
        {
            success = EditorCheck.GetComponentInEditMode<LayeredSpriteAnimator>(
                                ref _layeredAnimator, (() => { return GetComponent<LayeredSpriteAnimator>(); }));
            if(_layeredAnimator == null && !success) 
            {
                Debug.Log("Cannot get Layered Animator in edit mode");
                return false;
            }

            return true;
        }

        else
            return false;
    }

    [ExecuteAlways]
    protected void Start() { CheckCompnents(); }

    [ExecuteAlways]
    public void UpdateInEditor(float time)
    {
        _spriteAnimator?.UpdateInEditor(time);
        _layeredAnimator?.UpdateInEditor(time);
    }

    [ExecuteAlways]
    public virtual void EnableAnimUpdate(bool enable)
    {
        _continueAnimation = enable;
        if(enable) return;

        _isAlerted     = false;
        _isHurted      = false;
        _isDefeated    = false;
        _isWalk        = false;
        _isDash        = false;
        _isAttack      = false;
        _isPhaseChange = false;
    }

    [ExecuteAlways]
    public virtual void SetPhaseGraphics(int phase)
    {
        _spriteAnimator?.SetLibraryByPhase(phase);
        _layeredAnimator?.SetLibraryByPhase(phase);
        _phase = phase;
    }

    [ExecuteAlways]
    public void AnimState(int animState)
    {
        CheckCompnents();
        Animate(animState);
    }

    [ExecuteAlways]
    protected virtual void Animate(int actionState)
    {
        _spriteAnimator?.Anim(actionState);
        _layeredAnimator?.Anim(actionState);
    }

    [ExecuteAlways]
    public void CutsceneAnimationLogicUpdate()
    {
        CutsceneAnimationLogic();
    }

    [ExecuteAlways]
    protected virtual void CutsceneAnimationLogic(){}

    [ExecuteAlways]
    public virtual void FaceTowards(Direction direction)
    {
        CheckCompnents();
        SetAnimationDirection(direction);
    }

    //Cache data for editor animation preview
    protected GameObject _player = null;
    protected GameObject _boss   = null;

    [ExecuteAlways]
    public virtual void FaceTowards(GameObject target)
    {
        CheckCompnents();
        SetAnimationDirectionTowards(target.transform.position);
    }

    [ExecuteAlways]
    public void FaceTowardsPlayer()
    {
        if(!EditorCheck.inEditMode)
        {
            if(UnitManager.Instance.GetPlayer())
                FaceTowards(UnitManager.Instance.GetPlayer());
            return;
        }

        bool success = EditorCheck.GetComponentInEditMode<GameObject>(ref _player, 
            (() => { return GameObject.FindObjectOfType<Player>()?.gameObject; }));
        if(_player == null && !success) return;

        FaceTowards(_player);
    }

    [ExecuteAlways]
    public void FaceTowardsBoss()
    {
        if(!EditorCheck.inEditMode)
        {
            FaceTowards(UnitManager.Instance.GetBoss());
            return;
        }

        bool success = EditorCheck.GetComponentInEditMode<GameObject>(ref _boss, 
            (() => { return GameObject.FindObjectOfType<BossConductor>()?.gameObject; }));
        if(_boss == null && !success) return;
        FaceTowards(_boss);
    }
}

}