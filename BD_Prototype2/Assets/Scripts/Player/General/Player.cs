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
    public static float currentHealth { get; private set; } = 0f;
    public bool  isDead        { get { return currentHealth <= 0f; } }
    public float healthRatio   { get { return currentHealth/startingHealth; } }
    public bool  isHealthLower(float ratio) { return currentHealth <= startingHealth * ratio; }

    public int Fails = 0, MaxFails = 3;
    public float ResetTimeSeconds = 10f;

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

    [SerializeField]
    GameObject actionUI;

    private void OnEnable()
    {
        EventManager.Instance.OnPlayerDamage += TakeDamage;
        EventManager.Instance.OnPlayerHeal   += Heal;

        EventManager.Instance.OnPlayerSuccessBeatHit += SuccessBeatCheck;
    }


    private void OnDisable()
    {
        EventManager.Instance.OnPlayerDamage -= TakeDamage;
        EventManager.Instance.OnPlayerHeal   -= Heal;

        EventManager.Instance.OnPlayerSuccessBeatHit -= SuccessBeatCheck;
    }

    private void Start()
    {
        currentHealth = startingHealth;

        EventManager.Instance.PlayerSpawned();

        _checkpointManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CheckpointManager>();
        actionUI = GameObject.Find("ActionsUI");    
    }

    //Debug code -- Remember to remove
    #if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            EventManager.Instance.PlayerDamage(currentHealth);
    }
    #endif

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
            playerFailState = PlayerFailState.FAILED;
            EventManager.Instance.PlayerTired();
        }
    }

    IEnumerator ResetFails()
    {
        yield return new WaitForSeconds(ResetTimeSeconds);
        playerFailState = PlayerFailState.NORMAL;
        Fails = 0;
        EventManager.Instance.PlayerNormal();
    }

    // -- Player health -- //
    public void Heal(float healAmount)
    {
        if (currentHealth <= 0)
            return;

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
        if (currentHealth <= 0)
            return;

        currentHealth -= damage;


        if (currentHealth <= 0)
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
    }

    public void RespawnPlayer(Vector2 spawnPoint)
    {
        transform.position = spawnPoint;
    }
}