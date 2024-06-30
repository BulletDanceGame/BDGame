using UnityEngine;

namespace BulletDance.Audio //Ignore indent of this {} bc that's annoying
{

/*
    This class contains all player SFX & rtpc controlls.
        (excluding cutscene-specific controls, that should be separated bc there could be many of them)
        (excluding level-specific controls, because they'd be loaded for every level if put in here)
    There should be NO REFERENCE this class.

    This is Mo's living space.

    HEY!!! THAT'S MEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE!
*/
public class PlayerSounds : SoundContainer
{
    // -- Event Hooks & sound initialization -- //
    void Start()
    {
        //Init
        Initialize(); //So we can get SoundManager.Instance to access PlaySFX & other common helper methods

        //Events
        EventManager.Instance.OnPlayerDash      += PlayerDash;
        EventManager.Instance.OnPlayerSlowMo    += PlayerSlowMo;
        EventManager.Instance.OnPlayerSlowMoEnd += PlayerSlowMoEnd;

        EventManager.Instance.OnPlayerAttack    += PlayerSwing;
        EventManager.Instance.OnPlayerHitBullet += BulletHit;
        EventManager.Instance.OnMaxCombo        += ResetHitPitch;
        EventManager.Instance.OnPlayerLastHit   += PlayerEndGameHit;
        EventManager.Instance.OnCooldownStart   += PlayerCooldown;

        EventManager.Instance.OnPlayerDamage += PlayerHurt;
        EventManager.Instance.OnPlayerBurn   += PlayerBurn;
        EventManager.Instance.OnPlayerDeath  += PlayerDeath;

        EventManager.Instance.OnBeat           += PlayFootsteps;
        EventManager.Instance.OnPlayerMove     += StartFootstep;
        EventManager.Instance.OnPlayerStopMove += StopFootstep;

        EventManager.Instance.OnAddCurrency    += PickUpCurrency;
    }

    void OnDestroy()
    {
        DeInitialize();

        EventManager.Instance.OnPlayerDash      -= PlayerDash;
        EventManager.Instance.OnPlayerSlowMo    -= PlayerSlowMo;
        EventManager.Instance.OnPlayerSlowMoEnd -= PlayerSlowMoEnd;

        EventManager.Instance.OnPlayerAttack    -= PlayerSwing;
        EventManager.Instance.OnPlayerHitBullet -= BulletHit;
        EventManager.Instance.OnMaxCombo        -= ResetHitPitch;
        EventManager.Instance.OnPlayerLastHit   -= PlayerEndGameHit;
        EventManager.Instance.OnCooldownStart   -= PlayerCooldown;

        EventManager.Instance.OnPlayerDamage    -= PlayerHurt;
        EventManager.Instance.OnPlayerBurn      -= PlayerBurn;
        EventManager.Instance.OnPlayerDeath     -= PlayerDeath;

        EventManager.Instance.OnBeat            -= PlayFootsteps;
        EventManager.Instance.OnPlayerMove      -= StartFootstep;
        EventManager.Instance.OnPlayerStopMove  -= StopFootstep;

        EventManager.Instance.OnAddCurrency     -= PickUpCurrency;
    }


    // -- Update -- //
    void Update()
    {
        HitPitchUpdate();
    }


    // -- SFX Implementation -- //

    //Movement
    void PlayerDash(BeatTiming hitTiming, Vector2 none)
    {
        if(hitTiming == BeatTiming.BAD) return;
        PlaySFX("Dash");
        PlaySFX("Dash Step");
    }

    void PlayerCooldown()
    {
        //set cooldown rtpcs
        PlaySFX("Cooldown Start");
    }
        
    void PlayerSlowMo()
    {
        //set slo mo rtpcs
        PlaySFX("SlowMo Start");
    }

    void PlayerSlowMoEnd()
    {
        //reset slo mo rtpcs
    }


    bool _isPlayerMoving = false;
    void PlayFootsteps(int none)
    {
        if(_isPlayerMoving) PlaySFX("Footstep");
    }

    void StartFootstep(Vector2 none)
    {
        _isPlayerMoving = true;
    }

    void StopFootstep()
    {
        PlaySFX("Last Footstep");      
        _isPlayerMoving = false;
    }


    //Swing
    void PlayerSwing(BeatTiming hitTiming, Vector2 none)
    {
            //on a perfect hit it feels delayed, and when hitting it so it fits you are roughly 50ms early
            switch (hitTiming)
            {
                case BeatTiming.PERFECT:
                    PlaySFX("Swing Perfect"); break;

                case BeatTiming.GOOD:
                    PlaySFX("Swing Good"); break;

                case BeatTiming.BAD:
                    PlaySFX("Swing Miss"); break;

                default: break;
            }
        }


    //Bullet hit
    //Move this out
    [SerializeField] private int _maxComboForPitch = 20;
    [SerializeField] private float _comboDuration = 3f;
    private int   _hitOnBeat, _hitPitch;
    private bool  _inCombo = false;
    private float _timeSinceCombo = 0;

    void BulletHit(BeatTiming hitTiming)
    {
        switch(hitTiming)
        {
            case BeatTiming.PERFECT:
                PlaySFX("Hit Perfect", 1f, null, true);
                break;

            case BeatTiming.GOOD:
                PlaySFX("Hit Good", 1f, null, true);
                break;

            case BeatTiming.BAD:
                PlaySFX("Hit Miss", 1f, null, true);
                ResetHitPitch();
                break;

            default: break;
        }


        //Reset timer every deflection
        _timeSinceCombo = 0;


        //Adjust the pitch for combos
        _inCombo  = hitTiming != BeatTiming.BAD;
        _hitPitch = _inCombo ? (_hitPitch + 100/_maxComboForPitch) : 0; //If combo-d, raise pitch, else reset
        _hitPitch = Mathf.Clamp(_hitPitch, 0, 100);

        RTPCManager.Instance.SetValue("PITCH_SPECIAL____DeflectionElement__PitchElement", _hitPitch, 0.0000000001f, RTPCManager.CurveTypes.linear);
        RTPCManager.Instance.SetValue("VOLUME_SPECIAL____DeflectionElement__PitchElement", _hitPitch, 0.0000000001f, RTPCManager.CurveTypes.linear);
    }

    void ResetHitPitch()
    {
        _hitPitch = 0;
        RTPCManager.Instance.ResetValue("PITCH_SPECIAL____DeflectionElement__PitchElement", 0.0000000001f, RTPCManager.CurveTypes.linear);
        RTPCManager.Instance.ResetValue("VOLUME_SPECIAL____DeflectionElement__PitchElement", 0.0000000001f, RTPCManager.CurveTypes.linear);
    }

    void HitPitchUpdate()
    {
        if(!_inCombo) return;

        _timeSinceCombo += Time.deltaTime;
        if(_timeSinceCombo > _comboDuration)
        {
            ResetHitPitch();
            _inCombo = false;
            _timeSinceCombo = 0f;
        }
    }

    void MaxComboHit()
    {
        PlaySFX("Hit Max Combo");
        PlaySFX("SlowMo Start");
        //nothing's done in here...?
    }

    void PlayerEndGameHit(BeatTiming hitTiming)
    {
        if(!BossController.Instance.bossHealth.isLastPhase)
        {
            //PlaySFX("Hit Max Combo");
        }
        else
        {
            PlaySFX("Hit End Fight");
            RTPCManager.Instance.SetAttributeValue("VOLUME", 0, 0.00000000000001f, RTPCManager.CurveTypes.linear, "VOLUME____PlayerActions__Set");
        }
    }


    //Hurt
    void PlayerBurn(bool isBurning)
    {
        if(!isBurning) return;

        PlaySFX("Burn");
    }

    void PlayerHurt(float damage)
    {
        if (damage <= 1) return; //Don't play if burn

        var player = UnitManager.Instance.GetPlayer().GetComponent<Player>();
        if (player.isDead) return;

        //LowPass on low health
        if (player.isHealthLower(0.30f))
            RTPCManager.Instance.SetAttributeValue("LOW PASS", (30 - (player.healthRatio * 30f)), 0.0000000001f, RTPCManager.CurveTypes.linear);

        //Fading mute when hit
        if (!player.isHealthLower(0.30f))
            RTPCManager.Instance.AddAttributeValue("LOW PASS", 70, 0.00000000001f, RTPCManager.CurveTypes.linear, 0.7f, RTPCManager.CurveTypes.high_curve, 0.2f, "LOW_PASS____PlayerDamage");
        else
            RTPCManager.Instance.AddAttributeValue("LOW PASS", 30, 0.00000000001f, RTPCManager.CurveTypes.linear, 0.7f, RTPCManager.CurveTypes.high_curve, 0.2f, "LOW_PASS____PlayerDamage");

        //Adjust hurt sound
        string soundState = player.isHealthLower(0.20f) ? "low" :
                            player.isHealthLower(0.45f) ? "mid" :
                                                          "high";
        RTPCManager.Instance.SetState("Hurt", soundState);
        PlaySFX("Hurt");
    }

    void PlayerDeath()
    {   
        RTPCManager.Instance.SetValue("PLAYBACK_SPEED____MusicBossBattle", 5, 15, RTPCManager.CurveTypes.high_curve);
        RTPCManager.Instance.SetValue("PLAYBACK_SPEED____MusicEnvBattle", 5, 15, RTPCManager.CurveTypes.high_curve);
        RTPCManager.Instance.SetValue("PLAYBACK_SPEED____MusicEnvRoaming", 5, 15, RTPCManager.CurveTypes.high_curve);
        RTPCManager.Instance.SetValue("LOW_PASS____MusicBossBattle", 70, 0.0000000001f, RTPCManager.CurveTypes.high_curve);
        RTPCManager.Instance.SetValue("LOW_PASS____MusicEnvBattle", 70, 0.0000000001f, RTPCManager.CurveTypes.high_curve);
        RTPCManager.Instance.SetValue("LOW_PASS____MusicEnvRoaming", 70, 0.0000000001f, RTPCManager.CurveTypes.high_curve);
        RTPCManager.Instance.SetValue("VOLUME____MusicBossBattle", 10, 15, RTPCManager.CurveTypes.high_curve);
        RTPCManager.Instance.SetValue("VOLUME____MusicEnvBattle", 10, 15, RTPCManager.CurveTypes.high_curve);
        RTPCManager.Instance.SetValue("VOLUME____MusicEnvRoaming", 10, 15, RTPCManager.CurveTypes.high_curve);
        RTPCManager.Instance.SetValue("PITCH____PlayerDamage", 12.5f, 20, RTPCManager.CurveTypes.high_curve);
        
        RTPCManager.Instance.AddAttributeValue("VOLUME", 0, 0.00000000000001f, RTPCManager.CurveTypes.linear,
            "VOLUME____MusicBossBattle", "VOLUME____MusicEnvBattle", "VOLUME____MusicEnvRoaming",
            "VOLUME____Menu", "VOLUME____PlayerDamage");
    }



    //Pick Up SFX
    void PickUpCurrency(int none)
    {
        PlaySFX("Pick Up Currency");
    }
}


}