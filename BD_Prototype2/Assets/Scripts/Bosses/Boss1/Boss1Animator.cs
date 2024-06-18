using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using BulletDance.Animation;

/****
    Seperate class containing animation methods, so that
    - Artists have a seperate workspace than programmers
    - Artists only lose direct implementations (when the animation triggers) in case merging goes wrong
    - More legible implementation names than animator.SetTrigger etc.
***/

public class Boss1Animator : MonoBehaviour
{
    [SerializeField]
    private LayeredSpriteAnimator _animator;

    private float _angleToPlayer = 0;
    Direction _animDirection = 0;

    //Keep track of previous and current animation states
    private bool _isTransition = false;
    private bool _isDashing    = false;

    private bool _isIdling     = false;
    private bool _isPrevIdling = false;

    private bool _isWalking     = false;
    private bool _isPrevWalking = false;

    private bool _isAttacking     = false;
    private bool _isPrevAttacking = false;

    //Attack animation cooldown
    private float _attackCoolDown   = 1f;
    private float _coolDownDuration = 3f;

    [Space]

    [SerializeField]
    private SpriteRenderer _torsoRenderer;
    private Material _mat;

    [SerializeField]
    private List<BulletDance.Graphics.ColorName> _energyColors;
    private Dictionary<string, Color> _colorLookUp = new Dictionary<string, Color>();    
    
    private string _oldColor = "Default";
    private string _newColor = "Default";

    [Space]

    [SerializeField]
    private List<PhaseParameters> _phaseParameters;

    [System.Serializable]
    private class PhaseParameters
    {
        public bool  isEnergyGlow;
        public float glowStrength;
        public float whiteGlowModifier;
    }


    [Space]

    [SerializeField]
    private ParticleSystem _smokeParticle;
    [SerializeField]
    private ParticleSystem _soulFormVFX1, _soulFormVFX2, _transformVFX;
    [SerializeField]
    private Transform _shadow;


    void Start()
    {
        //Disable component if SpriteAnimator not found
        if(_animator == null)
        {
            this.enabled = false;
            return;
        }

        SetCoolDownDuration();

        _mat = _torsoRenderer.material;

        //Create energy color lookup
        foreach(var energyColor in _energyColors)
        {
            _colorLookUp.Add(energyColor.name, energyColor.color);
        }

        //Set starting material parameters
        SetMaterialParameters(0);

        //Event-based animation
        _animator.OnAnimActionChanged     += OnEnergyChangeFinished;

        if(EventManager.Instance == null) return;
        // EventManager.Instance.OnBossDamage   += Hurt;
        // EventManager.Instance.OnBossWalk     += OnBossWalk;
        // EventManager.Instance.OnBossStopWalk += OnBossStopWalk;
        // EventManager.Instance.OnBossDash     += OnBossDash;
        // EventManager.Instance.OnBossStopDash += OnBossStopDash;
        // EventManager.Instance.OnBossAttack   += OnBossAttack;
        //EventManager.Instance.OnBossPhaseChangeFinished += OnPhaseChangeFinished;
    }

    //Unsubscribe events
    void OnDestroy()
    {
        if(_animator != null)
            _animator.OnAnimActionChanged -= OnEnergyChangeFinished;

        if(EventManager.Instance == null) return;
        // EventManager.Instance.OnBossDamage   -= Hurt;
        // EventManager.Instance.OnBossWalk     -= OnBossWalk;
        // EventManager.Instance.OnBossStopWalk -= OnBossStopWalk;
        // EventManager.Instance.OnBossDash     -= OnBossDash;
        // EventManager.Instance.OnBossStopDash -= OnBossStopDash;
        // EventManager.Instance.OnBossAttack   -= OnBossAttack;
        //EventManager.Instance.OnBossPhaseChangeFinished -= OnPhaseChangeFinished;
    }


    void Update()
    {
        //Transition cutscene override
        if(_isTransition) return;

        //Determine the animation's direction
        //Dashing will be set with a different direction
        if(!_isDashing)
            SetAnimationDirection();

        //Dash overrides other animation
        if(_isDashing) return;

        _isIdling = (!_isAttacking && !_isWalking);

        AnimationStateSwitch();

        //Only update the previous states if not dashing
        _isPrevAttacking = _isAttacking;
        _isPrevWalking   = _isWalking;
        _isPrevIdling    = _isIdling;
    }


    void AnimationStateSwitch()
    {
        //Idle overrides
        if(_isIdling && !_isPrevIdling)
        {
            _animator.AnimIdle();
            return;
        }

        Attack();
        Walk();
    }


    // -- Utilities -- //
    void SetCoolDownDuration()
    {
        //Sometimes the music manager doesn't correctly instanced the seconds per beat
        // => Call every time attack animation is requested
        if(MusicManager.Instance != null)
            _coolDownDuration = (float)MusicManager.Instance.secondsPerBeat * 4f;
        else
            _coolDownDuration = 3f;
    }

    void SetMaterialParameters(int phaseIndex)
    {
        var _parameters = _phaseParameters[phaseIndex];
        int _isEnergyGlow = _parameters.isEnergyGlow ? 1 : 0;

        _mat.SetInt("_isEnergyGlow", _isEnergyGlow);
        _mat.SetFloat("_GlowStrength", _parameters.glowStrength);
        _mat.SetFloat("_WhiteGlowModifier", _parameters.whiteGlowModifier); //BITCH WHY U NO CHANGE
    }


    // -- Set Phase -- //
    public void OnPhaseChange(int phaseNum)
    {
        _isTransition = true;
        _isIdling = _isWalking = _isDashing = _isAttacking = _isPrevIdling = _isPrevWalking = _isPrevAttacking = false;

        _animator.SetDirection(Direction.Front);
        _animator.Anim(5);

        _mat.SetColor("_NewColor", Color.white);
    }

    void OnPhaseChangeFinished(int phaseNum)
    {
        _isTransition = false;
        _isIdling = _isWalking = _isDashing = _isAttacking = _isPrevIdling = _isPrevWalking = _isPrevAttacking = false;

        if(phaseNum >= 3)
        {
            _mat.SetColor("_OldColor", Color.white);
            _soulFormVFX1.Play();
            _soulFormVFX2.Play();
        }
        else
            _mat.SetColor("_NewColor", _colorLookUp[_newColor]);

        _animator.SetLibraryByPhase(phaseNum);
        SetMaterialParameters(phaseNum - 1);
    }


    // -- Direction -- //
    void SetAnimationDirection()
    {
        _angleToPlayer = Vector2.SignedAngle(
                        UnitManager.Instance.GetPlayer().transform.position - transform.position,
                        Vector2.up);

        if (_angleToPlayer < -35f && _angleToPlayer > -145f)
            _animDirection = Direction.Left;

        else if(_angleToPlayer < 145f && _angleToPlayer > 35f)
            _animDirection = Direction.Right;

        else if(_angleToPlayer <= -145f || _angleToPlayer >= 145f)
            _animDirection = Direction.Front;

        else
            _animDirection = Direction.Back;


        _animator.SetDirection(_animDirection);
    }


    // -- VFX -- //
    void Hurt(float none)
    {
        _animator.AnimHurt();
    }


    // -- Walk -- //
    void OnBossWalk()     { _isWalking = true;  }
    void OnBossStopWalk() { _isWalking = false; }
    void Walk()
    {
        //Previously not walking
        //Now walking - Return from dash or walk animation is requested
        if(!_isPrevWalking && _isWalking)
        {
            //Torso is attaking
            if(_isAttacking) _animator.AnimWalk("Legs");
            else             _animator.AnimWalk();
        }

        //Previously walking && Stop walkking
        //If it reached here, it has bypassed the Idle override => Only stop Legs
        if(_isPrevWalking && !_isWalking)
        {
            _animator.AnimIdle("Legs");
        }
    }


    // -- Dash -- //
    private Dictionary<Vector2, Direction> _directionLookup = new Dictionary<Vector2, Direction>
    {
        {Vector2.up,    Direction.Back},
        {Vector2.down,  Direction.Front},
        {Vector2.left,  Direction.Left},
        {Vector2.right, Direction.Right},
    };

    void OnBossStopDash() { _isDashing = false; }
    void OnBossDash(Vector2 dashDirection)
    {
        if(_isTransition)
        {
            _isDashing = false;
            return;
        }

        _isDashing = true;
        _animator.SetDirection(_directionLookup[dashDirection]);
        _animator.AnimDash();

        _isPrevIdling    = false;
        _isPrevWalking   = false;
        _isPrevAttacking = false;
    }


    // -- Attack -- //
    void OnBossAttack()
    {
        _isAttacking = true;
        SetCoolDownDuration();
        _attackCoolDown = _coolDownDuration;
    }

    void Attack()
    {
        //Previously not attacking
        //Now attacking - Return from dash or attack animation is requested
        if(!_isPrevAttacking && _isAttacking)
        {
            //Leg is walking
            if(_isWalking) _animator.AnimAttack("Torso");
            else           _animator.AnimAttack();
        }


        //If it reached here, it has bypassed the Idle & Dash override
        // => Only count down when not idle or dash
        _attackCoolDown -= Time.deltaTime;
        if(_attackCoolDown <= 0f)
            _isAttacking = false;


        //Previously attacking && Stop attacking
        //If it reached here, it has bypassed the Idle override => Only stop Torso
        if(_isPrevAttacking && !_isAttacking)
            _animator.AnimIdle("Torso");
    }


    // -- Energy Change -- //
    void OnEnergyChange(string bulletName)
    {
        if(_animator.currentLibrary != "Phase 2") return;

        _animator.Anim(4);

        _oldColor = _newColor;
        _newColor = bulletName;

        _mat.SetColor("_OldColor", _colorLookUp[_oldColor]);
        _mat.SetColor("_NewColor", _colorLookUp[_newColor]);
    }


    void OnEnergyChangeFinished()
    {
        if(_animator.currentLibrary != "Phase 2") return;

        if(_animator.hasActionChangedFrom(4))
        {
            _oldColor = _newColor;
            _mat.SetColor("_OldColor", _colorLookUp[_oldColor]);
        }
    }



    //Cutscene stuff
    public void StartSmokeParticles()
    {
        _smokeParticle.Play();
    }

    public void SetTransformVFX(bool enable)
    {
        if (enable)
        {
            _transformVFX.Play();
            _shadow.position += Vector3.down * 0.3f;
        }
        else
            _transformVFX.Stop();
    }

    public void Death()
    {
        _isTransition = true;
        _animator.SetDirection(Direction.Front);
        _soulFormVFX1.Stop();
        _soulFormVFX2.Stop();
        _animator.Anim(6);
    }
}