using BulletDance.Audio;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum PlayerFailState { NORMAL, FAILED};

    public PlayerFailState playerFailState = PlayerFailState.NORMAL;

    [field: SerializeField]
    public float startingHealth { get; private set; } = 150f;
    [field: SerializeField]
    public float defaultHealAmount { get; private set; } = 50f;
    public float currentHealth { get; private set; } = 0f;
    public bool  isDead        { get { return currentHealth <= 0f; } }
    public float healthRatio   { get { return currentHealth/startingHealth; } }
    public bool  isHealthLower(float ratio) { return currentHealth <= startingHealth * ratio; }

    public int Fails = 0, MaxFails = 3;
    public float ResetTimeSeconds = 10f;
    public int BeatTilReset;

    public bool pauseActions = false;

    [SerializeField]
    public static bool isSlowmo;

    [SerializeField]
    public bool canSlowmo;

    [SerializeField]
    MonoBehaviour[] _componentsToDisableOnDeath;

    [Header("InteractableObject")]
    public bool AbleToInteract = false;
    public TabletText textToDisplay;
    public Canvas TabletUI;
    public TextMeshProUGUI TabletText;

    [Header("Player hitbox")]
    [SerializeField]
    float _hitBoxDowntime = 2f;

    public Collider2D triggerBox;

    private CheckpointManager _checkpointManager;

    private float _beatTracker;

    [SerializeField]
    GameObject actionUI;

    private void OnEnable()
    {
        EventManager.Instance.OnPlayerDamage += TakeDamage;
        EventManager.Instance.OnPlayerHeal   += Heal;
        EventManager.Instance.OnEnableInput += ActivateInput;
        EventManager.Instance.OnDisableInput += DeactivateInput;

        EventManager.Instance.OnBeat += BeatCounter; 
        EventManager.Instance.OnPlayerSuccessBeatHit += SuccessBeatCheck;

        EventManager.Instance.OnCutsceneStart += ResetFail;
    }


    private void OnDisable()
    {
        EventManager.Instance.OnPlayerDamage -= TakeDamage;
        EventManager.Instance.OnPlayerHeal   -= Heal;

        EventManager.Instance.OnEnableInput -= ActivateInput;
        EventManager.Instance.OnDisableInput -= DeactivateInput;

        EventManager.Instance.OnPlayerSuccessBeatHit -= SuccessBeatCheck;

        EventManager.Instance.OnCutsceneStart -= ResetFail;
    }

    IEnumerator Start()
    {
        currentHealth = startingHealth;

        EventManager.Instance.PlayerSpawned();

        _checkpointManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CheckpointManager>();
        actionUI = GameObject.Find("ActionsUI");

        yield return null;
    }

    //Debug code -- Remember to remove
    #if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            EventManager.Instance.PlayerDamage(currentHealth);
        if (Input.GetKeyDown(KeyCode.O))
            EventManager.Instance.PlayerHeal(startingHealth);
    }
#endif



    void ActivateInput()
    {
        GetComponent<PlayerInput>().ActivateInput();
    }

    void DeactivateInput()
    {
        GetComponent<PlayerInput>().DeactivateInput();
    }



    void OnInteract(InputValue value)
    {
        if (TabletUI.gameObject.activeSelf)
        {
            TabletUI.gameObject.SetActive(false);
            return;
        }

        if (!AbleToInteract)
            return;

        TabletText.text = textToDisplay.textForTablet;
        TabletUI.gameObject.SetActive(true);
        
    }

    void OnOpenActionUI()
    {
        //Currently the slider doesnt update if you disable it, so we will just keep it open for now

        //if (actionUI.activeSelf)
        //    actionUI.SetActive(false);
        //else
        //    actionUI.SetActive(true);
            
    }

    void OnPause()
    {
        if(isDead)
            return;
        EventManager.Instance.PausePressed();
        pauseActions = !pauseActions;
    }


    void SuccessBeatCheck()
    {
        Fails = 0;
    }

    Coroutine routine;

    public void Fail()
    {
        Fails++;

        if(routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ResetFails());


        if (Fails >= MaxFails && playerFailState == PlayerFailState.NORMAL)
        {
            AkSoundEngine.SetState("FailLevel", "Third");
            playerFailState = PlayerFailState.FAILED;
            EventManager.Instance.PlayerTired();
            EventManager.Instance.CooldownStart();
        }
    }

    IEnumerator ResetFails()
    {
        yield return new WaitForSeconds(ResetTimeSeconds);
        playerFailState = PlayerFailState.NORMAL;
        Fails = 0;
        EventManager.Instance.PlayerNormal();
        AkSoundEngine.SetState("FailLevel", "First");
    }

    void ResetFail(string none)
    {
        if(routine != null) StopCoroutine(routine);

        playerFailState = PlayerFailState.NORMAL;
        Fails = 0;
        EventManager.Instance.PlayerNormal();
        AkSoundEngine.SetState("FailLevel", "First"); 
    }


    // -- Player health -- //
    public void Heal(float healAmount)
    {
        if(isDead) return;

        if(healAmount <= 0)
            healAmount = defaultHealAmount;

        currentHealth += healAmount;

        if (currentHealth > startingHealth)
            currentHealth = startingHealth;
    }


    /// <summary>
    /// Main damage taking function
    /// </summary>
    /// <param name="damage">The ammount of damage the player takes</param>
    public void TakeDamage(float damage)
    {
        if(isDead) return;

        currentHealth -= damage;

        if(isDead)
            PlayerDeath();
        else
            StartCoroutine(HurtFrames());
    }

    IEnumerator HurtFrames()
    {
        triggerBox.enabled = false;
        yield return new WaitForSeconds(_hitBoxDowntime);
        triggerBox.enabled = true;
    }


    void PlayerDeath()
    {
        foreach(var component in _componentsToDisableOnDeath)
        {
            component.enabled = false;
        }

        EventManager.Instance.PlayerDeath();
        gameObject.SetActive(false);

        //stop ambience
    }

    public void RespawnPlayer(Vector2 spawnPoint)
    {
        transform.position = spawnPoint;

        //print("respawn??");
    }

    public void BeatCounter(int beat)
    {
        if (beat % 2 != 0)
            return;

        _beatTracker++;

        if(_beatTracker>=BeatTilReset)
        {
            //Put reset fail here
        }
    }
}
