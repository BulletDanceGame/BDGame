using System;
using System.Data.Common;
using System.Security.Cryptography;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }


    //Input events -------------------------------------------------------------------------
    public event Action OnEnableInput;
    public event Action OnDisableInput;
    public event Action OnGamePadUsed;
    public event Action OnKeyBoardAndMouseUsed;

    public void EnabableInput()
    {
        OnEnableInput?.Invoke();
    }

    public void DisableInput()
    {
        OnDisableInput?.Invoke();
    }

    public void GamepadUsed()
    {
        OnGamePadUsed?.Invoke();
    }

    public void KeyboardAndMouseUsed()
    {
        OnKeyBoardAndMouseUsed?.Invoke();
    }


    //GameManager events -------------------------------------------------------------------------
    public event Action OnStartPlaying;

    public void StartGamePlay()
    {
        OnStartPlaying?.Invoke();
    }

    //SceneManager events -------------------------------------------------------------------------
    public event Action OnSceneLoad;
    public event Action OnSceneLoaded;

    public void SceneLoad()
    {
        OnSceneLoad?.Invoke();
    }

    public void SceneLoaded()
    {
        OnSceneLoaded?.Invoke();
    }

    //ScoreManager events -------------------------------------------------------------------------
    public event Action<int> OnAddScore;

    public void AddScore(int points)
    {
        OnAddScore?.Invoke(points);
    }

    public event Action OnMaxCombo;

    public void MaxCombo()
    {
        OnMaxCombo?.Invoke();
    }

    public event Action OnStartTimer;
    public event Action OnStopTimer;
    public event Action OnResetTimer;
    public event Action OnUpdateTimer;

    public void StartTimer()
    {
        OnStartTimer?.Invoke();
    }

    public void StopTimer()
    {
        OnStopTimer?.Invoke();
    }

    public void ResetTimer()
    {
        OnResetTimer?.Invoke();
    }

    public void UpdateTimer()
    {
        OnUpdateTimer?.Invoke();
    }

    //ActionLogging events -----------------------------------------------------------------------

    public event Action OnDash;
    public event Action OnHit;
    public event Action OnWallBounce;
    public event Action OnReHit;
    public event Action OnRehitWallBounce;
    public event Action OnRehitWallBounceDamage;
    public event Action OnDamage;

    public void Dash()
    {
        OnDash?.Invoke();
    }

    public void Hit()
    {
        OnHit?.Invoke();
    }

    public void WallBounce()
    {
        OnWallBounce?.Invoke();
    }

    public void ReHit()
    {
        OnReHit?.Invoke();
    }

    public void RehitWallBounce()
    {
        OnRehitWallBounce?.Invoke();
    }

    public void RehitWallBounceDamage()
    {
        OnRehitWallBounceDamage?.Invoke();
    }

    public void Damage()
    {
        OnDamage?.Invoke();
    }


    //Player events ------------------------------------------------------------------------------
    public event Action OnCooldownStart;

    public void CooldownStart()
    {
        OnCooldownStart?.Invoke();
    }


    //Spawn and death
    public event Action OnPlayerSpawned; //After  player spawned
    public event Action OnPlayerDeath;   //Death
    public event Action OnWin;

    public event Action OnPlayerTired;
    public event Action OnPlayerNormal;
    public event Action OnPlayerSuccessBeatHit;

    /// <summary> Player has spawned, after SetupPlayer </summary>
    public void PlayerSpawned()
    {
        OnPlayerSpawned?.Invoke();
    }

    public void PlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }

    public void Win()
    {
        OnWin?.Invoke();
    }

    public void PlayerTired()
    {
        OnPlayerTired?.Invoke();
    }

    public void PlayerNormal()
    {
        OnPlayerNormal?.Invoke();
    }

    public void PlayerSuccessBeatCheck()
    {
        OnPlayerSuccessBeatHit?.Invoke();
    }

    //Health
    public event Action<float> OnPlayerHeal;   //Use every time the player heals
    public event Action<float> OnPlayerDamage; //Use every time the player takes damage
    public event Action<bool> OnPlayerBurn;    //Use every beat the player is burned

    /// <summary>
    /// Use everytime you want the player to heal
    /// </summary>
    /// <param name="healAmount">The heal ammount the player recieves</param>
    public void PlayerHeal(float healAmount)
    {
        OnPlayerHeal?.Invoke(healAmount);
    }

    /// <summary>
    /// Use everytime you want the player to take damage
    /// </summary>
    /// <param name="damage">The damage ammount the player takes</param>
    public void PlayerDamage(float damage)
    {
        OnPlayerDamage?.Invoke(damage);
    }

    public void PlayerBurn(bool isBurning)
    {
        OnPlayerBurn?.Invoke(isBurning);
    }

    //Movement
    public event Action<Vector2> OnPlayerMove;
    public event Action OnPlayerStopMove;

    public void PlayerMove(Vector2 direction)
    {
        OnPlayerMove?.Invoke(direction);
    }

    public void PlayerMoveStop()
    {
        OnPlayerStopMove?.Invoke();
    }


    //Dash
    public event Action<BeatTiming, Vector2> OnPlayerDash;

    public void PlayerDash(BeatTiming quality, Vector2 direction)
    {
        OnPlayerDash?.Invoke(quality, direction);
    }

    //Slomo
    public event Action OnPlayerSlowMo;
    public event Action OnPlayerSlowMoEnd;

    public void PlayerSlowMo()
    {
        OnPlayerSlowMo?.Invoke();
    }

    public void PlayerSlowMoEnd()
    {
        OnPlayerSlowMoEnd?.Invoke();
    }


    //Swing
    public event Action<BeatTiming, Vector2> OnPlayerAttack;
    public event Action<BeatTiming> OnPlayerBunt;

    public event Action OnPlayerMiss;
    public event Action<BeatTiming> OnPlayerHitBullet;
    public event Action<BeatTiming> OnPlayerLastHit;

    /// <summary> Use when player performed a succesful swing (not tired or already swinging) </summary>
    public void PlayerAttack(BeatTiming quality, Vector2 direction)
    {
        OnPlayerAttack?.Invoke(quality, direction);
    }

    public void PlayerBunt(BeatTiming hitTiming)
    {
        OnPlayerBunt?.Invoke(hitTiming);
    }

    public void PlayerHit(BeatTiming hitTiming)
    {
        OnPlayerHitBullet?.Invoke(hitTiming);
    }

    public void PlayerMiss()
    {
        OnPlayerMiss?.Invoke();
    }

    public void PlayerLastHit(BeatTiming timing)
    {
        OnPlayerLastHit?.Invoke(timing);
    }

    //What?
    public event Action<Vector3> OnPlayerPushBack;
    public void PlayerPushBack(Vector3 pusherPos)
    {
        OnPlayerPushBack?.Invoke(pusherPos);
    }


    //Boss events ----------------------------------------------------------------------------------

    //Boss fight set up
    public event Action<int> OnSetupBossFight;

    public event Action OnActivateBoss;
    public event Action OnDeactivateBoss;

    /// <summary>
    /// Use to setup the boss fight
    /// </summary>
    /// <param name="id">ID of the boss you want to fight</param>
    public void SetupBossFight(int id)
    {
        OnSetupBossFight?.Invoke(id);
    }

    public void ActivateBoss()
    {
        OnActivateBoss?.Invoke();
    }

    public void DeactivateBoss()
    {
        OnDeactivateBoss?.Invoke();
    }


    //Spawn & Death
    public event Action OnBossSpawned;
    public event Action OnBossDeath;

    /// <summary>
    /// Tells listeners that the Boss has spawned
    /// </summary>
    public void BossSpawn()
    {
        OnBossSpawned?.Invoke();
    }
    /// <summary>
    /// Tells listeners that the Boss has died
    /// </summary>
    public void BossDeath()
    {
        OnBossDeath?.Invoke();
    }


    //Health
    public Action<float> OnBossDamage, OnBossHeal;
    public event Action OnBossPhaseChange;

    /// <summary>
    /// Use everytime you want the Boss to take damage
    /// </summary>
    /// <param name="damage">The damage ammount the player takes</param>
    public void BossDamage(float damage)
    {
        OnBossDamage?.Invoke(damage);
    }

    public void BossHeal(float damage)
    {
        OnBossHeal?.Invoke(damage);
    }

    /// <summary>
    /// Use everytime the Boss changes phase
    /// </summary>
    public void BossPhaseChange()
    {
        OnBossPhaseChange?.Invoke();
    }


    //What?
    public event Action<bool> OnBossEndPhaseHit;


    /// <summary>
    /// Executes every final hit
    /// </summary>
    /// <param name="isLastPhase">Is the current phase the final phase?</param>
    public void BossEndPhaseHit(bool isLastPhase)
    {
        OnBossEndPhaseHit?.Invoke(isLastPhase);
    }


    //Music events ------------------------------------------------------------------------------------

    //Rename both event and linked function
    public event Action<int> OnBeat;
    public event Action<int> OnPlayerRhythmBeat;
    public event Action<int, float, int> OnBeatForVisuals;
    public event Action<AK.Wwise.Event> OnNewSong;


    public void Beat(int beat)
    {
        OnBeat?.Invoke(beat);
    }

    public void PlayerRhythmBeat(int beatsUntilNext)
    {
        OnPlayerRhythmBeat?.Invoke(beatsUntilNext);
    }

    public void BeatForVisuals(int anticipation, float duration, int beat)
    {
        OnBeatForVisuals?.Invoke(anticipation, duration, beat);
    }

    public void NewSong(AK.Wwise.Event song)
    {
        OnNewSong?.Invoke(song);
    }


    //SFX events ------------------------------------------------------------------------------------

    public event Action<string, float, GameObject, bool> OnPlaySFX;

    /// <summary> Play SFX by its name </summary>
    /// <param name="sfxName">SFX Name</param>
    /// <param name="duration">(Optional) Time until sfx cuts off, default is 1s</param>
    /// <param name="source">(Optional) Which GameObject is the sound source (for spatial sound), default is MusicManager GameObject</param>
    /// <param name="playOnce">(Optional) Only one of this sound plays at any given moment, which prevents voice starvation, default is false</param>
    public void PlaySFX(string sfxName, float duration = 1f, GameObject source = null, bool playOnce = false)
    {
        OnPlaySFX?.Invoke(sfxName, duration, source, playOnce);
    }


    //Cutscene events ------------------------------------------------------------------------------------
    public event Action<string> OnCutsceneStart;
    public event Action OnCutsceneEnd;
    public event Action OnEnableSpeedUp;

    public void StartCutscene(string cutsceneName)
    {
        OnCutsceneStart?.Invoke(cutsceneName);
        OnStopTimer?.Invoke();
        PlayerHeal(-1f); //So it uses the default heal amount
    }

    public void EndCutscene()
    {
        OnCutsceneEnd?.Invoke();
        OnStartTimer?.Invoke();
    }

    public void EnableSpeedUp()
    {
        OnEnableSpeedUp?.Invoke();
    }


    //Minions events -------------------------------------------------------------------------------------

    public event Action<Vector2> OnMinionsDash;
    public event Action OnMinionsStopDash;

    public void MinionsDashStart(Vector2 dashDirection)
    {
        OnMinionsDash?.Invoke(dashDirection);
    }

    public void MinionsDashStop()
    {
        OnMinionsStopDash?.Invoke();
    }

    //Currency events -------------------------------------------------------------------------------------
    public event Action<int> OnAddCurrency;

    public void AddCurrency(int value)
    {
        OnAddCurrency?.Invoke(value);
    }





    public event Action OnPausePressed;
    public event Action<bool> OnPause;

    //PAUSE----------------------------------------------------------------------------------------
    public void PausePressed()
    {
        OnPausePressed?.Invoke();
    }

    public void Pause(bool pause)
    {
        OnPause?.Invoke(pause);

    }









    public event Action<string> OnCalibrationAlert;
    public void CalibrationAlert(string text)
    {
        OnCalibrationAlert?.Invoke(text);
    }

}