using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace BulletDance.Animation
{


public class BossYokaiHunterAnimator : BossAnimator
{
    // -- Set up -- //

    protected override void OnEnable()
    {
        base.OnEnable();
        if(_layeredAnimator != null)
            _layeredAnimator.OnAnimActionChanged += OnEnergyChangeFinished;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if(_layeredAnimator != null)
            _layeredAnimator.OnAnimActionChanged -= OnEnergyChangeFinished;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(_layeredAnimator != null)
            _layeredAnimator.OnAnimActionChanged -= OnEnergyChangeFinished;
    }

    [Space]
    [SerializeField]
    private SpriteRenderer _torsoRenderer;
    private Material _mat;

    new void Start()
    {
        CheckCompnents();

        _mat = _torsoRenderer.material;

        //Boss phase: Set material parameters
        SetMaterialParameters(0);

        //Energy color: Create color lookup
        foreach(var energyColor in _energyColors)
        {
            _colorLookUp.Add(energyColor.name, energyColor.color);
        }        
    }


    // -- Update -- //
    private bool _isIdle = false;

    //Attack animation cooldown
    private bool  _startAttack = false;
    private float _attackCoolDown   = 1f;
    private float _coolDownDuration = 3f;


    protected override void Update()
    {
        //Cutscene override
        if(!_continueAnimation) 
        {
            PhaseChange();
            return;
        }

        if(EditorCheck.inEditMode) return;

        //Determine the animation's direction
        //Dashing will be set with a different direction
        if(!_isDash) FaceTowardsPlayer();

        //Set attack cooldown duration
        //Sometimes the music manager doesn't correctly instanced the seconds per beat so it is updated every frame
        if(MusicManager.Instance != null)
            _coolDownDuration = (float)MusicManager.Instance.secondsPerBeat * 4f;
        else
            _coolDownDuration = 3f;

        //Attack cooldown (determines if the attack loop or attack begin plays)
        if(_startAttack)
            _attackCoolDown += Time.deltaTime;
        if(_attackCoolDown >= _coolDownDuration)
        {
            _isAttack    = false;
            _startAttack = false;
        }

        _isIdle = (!_isAttack && !_isWalk && !_isDash);

        TorsoUpdate();
        LegsUpdate();
    }

    public override void PlayAnimation(int anticipation, float duration)
    {
        //Cutscene override
        if(!_continueAnimation) return;
        if(anticipation != 0) return;

        _beatDuration = duration;
        _duration = GetDuration(_beatDuration, _animNoteDuration);

        _layeredAnimator.SetSpeed(1f / _duration);
    }


    void TorsoUpdate()
    {
        if(_isDash) return;
        if(_isIdle)
            _layeredAnimator.AnimIdle("Torso");

        else if(_isAttack)
        {
            if(!_startAttack)
            {
                _layeredAnimator.AnimAttack("Torso");
                _startAttack = true;
            }
            else
                _layeredAnimator.Anim("Torso", 4);
        }

        else if(_isWalk)
            _layeredAnimator.AnimWalk("Torso");
    }

    void LegsUpdate()
    {
        //Idle overrides
        if(_isIdle)
            _layeredAnimator.AnimIdle("Legs");

        //Walk is higher priority than attack for legs
        else if(_isWalk)
            _layeredAnimator.AnimWalk("Legs");

        else if(_isAttack)
        {
            if(!_startAttack)
                _layeredAnimator.AnimAttack("Legs");
            else
                _layeredAnimator.Anim("Legs", 4);
        }
    }        


    // -- Dash -- //
    protected override void DashStop() { _isDash = false; }
    protected override void DashStart(Vector2 direction)
    {
        if(!_continueAnimation)
        {
            _isDash = false;
            return;
        }

        _isDash = true;
        SetAnimationDirection(GetAnimationDirectionTowards(direction));
        _layeredAnimator.AnimDash();
    }


    // -- Attack -- //
    protected override void AttackStart()
    {
        if(!_continueAnimation)
        {
            _isAttack = false;
            return;
        }

        _isAttack = true;
        _attackCoolDown = 0;
    }


    //Walk
    protected override void WalkStart()
    {
        _isWalk = _continueAnimation ? true : false;
    }


    // -- Energy Change -- //

    [Space]
    [SerializeField]
    private List<BulletDance.Graphics.ColorName> _energyColors;
    private Dictionary<string, Color> _colorLookUp = new Dictionary<string, Color>();    
    
    private string _oldColor = "Default";
    private string _newColor = "Default";    

    void OnEnergyChange(string bulletName)
    {
        if(_layeredAnimator.currentLibrary != "Phase 2") return;

        _layeredAnimator.Anim(5);

        _oldColor = _newColor;
        _newColor = bulletName;

        _mat.SetColor("_OldColor", _colorLookUp[_oldColor]);
        _mat.SetColor("_NewColor", _colorLookUp[_newColor]);
    }


    void OnEnergyChangeFinished()
    {
        if(_layeredAnimator.currentLibrary != "Phase 2") return;

        if(_layeredAnimator.hasActionChangedFrom(5))
        {
            _oldColor = _newColor;
            _mat.SetColor("_OldColor", _colorLookUp[_oldColor]);
        }
    }


    // -- VFX -- //
    [SerializeField]
    private ParticleSystem _hurtParticle;

    void Hurt(float none)
    {
        base.Hurt();
        _hurtParticle.Play();
    }


    // -- Cutscene stuff -- //

    public override void EnableAnimUpdate(bool enable)
    {
        _layeredAnimator?.SetBool("isCutscene", !enable);
        base.EnableAnimUpdate(enable);
    }


    // Phase Change
    [System.Serializable]
    private class PhaseParameters
    {
        public bool  isEnergyGlow;
        public float glowStrength;
        public float whiteGlowModifier;
    }

    [Space] [SerializeField]
    private List<PhaseParameters> _phaseParameters;

    void SetMaterialParameters(int phaseIndex)
    {
        var _parameters = _phaseParameters[phaseIndex];
        int _isEnergyGlow = _parameters.isEnergyGlow ? 1 : 0;

        _mat.SetInt("_isEnergyGlow", _isEnergyGlow);
        _mat.SetFloat("_GlowStrength", _parameters.glowStrength);
        _mat.SetFloat("_WhiteGlowModifier", _parameters.whiteGlowModifier); //BITCH WHY U NO CHANGE
    }

    protected override void PhaseChangeStart(int phase)
    {
        _mat.SetColor("_NewColor", Color.white);
    }

    protected override void PhaseChangeFinished(int phase)
    {
        if(phase <= 0) return;

        base.PhaseChangeFinished(phase);

        if(phase >= 3)
        {
            _layeredAnimator.SetFloat("SoulForm", 1f);
            _mat.SetColor("_OldColor", Color.white);
            _soulFormVFX1.Play();
            _soulFormVFX2.Play();
        }
        else
            _mat.SetColor("_NewColor", _colorLookUp[_newColor]);

        SetMaterialParameters(phase - 1);
    }


    [SerializeField]
    private ParticleSystem _smokeParticle;

    public void StartSmokeParticles()
    {
        _smokeParticle.Play();
    }


    [SerializeField]
    private ParticleSystem _soulFormVFX1, _soulFormVFX2, _transformVFX;
    [SerializeField]
    private Transform _shadow;

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


    protected override void Defeat()
    {
        base.Defeat();
        _soulFormVFX1.Stop();
        _soulFormVFX2.Stop();
    }


}


}
