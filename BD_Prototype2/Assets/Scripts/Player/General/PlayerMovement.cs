using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]

public class PlayerMovement : MonoBehaviour
{

    //REMINDER: for slide time do bpm/60 and then we get the cooldown 
    [SerializeField]
    float _playerSpeed, _slideSpeed, _slideCooldown, _slideTime, _pushBackSpeed, _pushBackTime;
    public bool canDash = true, slideCooldown = false;
    public bool DashActivated = false;
    public bool canMove = true;

    [SerializeField]
    Vector2 _lastGroundPosition;

    [SerializeField]
    PlayerFeet _feet;

    public Vector2 StartDashPosition { get; private set; }
    [Space]
    [Header("DashStuff")]
    public bool dashOnBeat = false;
    public float dashDistance = 10.0f;

    public enum PlayerState { MOVING, SLIDING, PUSHED_BACK, LOCKED, LEDGEJUMPING, BYLEDGE};
    public PlayerState currentState;

    Rigidbody2D _playerRigidBody;
    PlayerInputActions _playerInput;
    SpriteRenderer _playerSpriteRenderer;

    //For AfterImageEffect
    [Space]
    [Header("After Image FX")]
    [SerializeField] Vector3 _afterImageOffset;
    [SerializeField] private float _distanceBetweenImages;
    [SerializeField] ParticleSystem _dustTrail;
    [SerializeField] ParticleSystem _blink;

    //Slomo   
    public float _slowAmount;
    [SerializeField] private float _slowTime;
    private bool _isSloMo;
    private float _startSlowTime;
    private float _normalTime = 1.0f;

    public bool canSlowMo;

    static Vector2 inputVector;

    private void OnEnable()
    {
        EventManager.Instance.OnPlayerPushBack += StartPushBack;
        EventManager.Instance.OnPlayerDeath += ResetMovement;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnPlayerPushBack -= StartPushBack;
        EventManager.Instance.OnPlayerDeath -= ResetMovement;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _playerRigidBody = gameObject.GetComponent<Rigidbody2D>();

        _playerInput = InputManager.Instance.PlayerInput;

        currentState = PlayerState.MOVING;

        _slideTime = (float)(_slideCooldown / 2);

        _playerInput.Player.Move.canceled += MoveStop;
    }

    private void OnDestroy()
    {
        _playerInput.Player.Disable();

        _playerInput.Player.Move.canceled -= MoveStop;
    }

    float timeByLedge = 0;
    private void Update()
    {
        if (currentState != PlayerState.MOVING)
            return;

        if (_feet.PlayerByLedge)
        {
            _playerRigidBody.velocity = new Vector3() * _playerSpeed;

            timeByLedge += Time.deltaTime;

            if(timeByLedge > 1)
            {
                if(!JLIsRunning)
                    StartCoroutine(jumpLedge());
            }
        }
        else
        {
            _playerRigidBody.velocity = inputVector * _playerSpeed;
            timeByLedge = 0;
        }                

        SlowMotion();
    }

    private void FixedUpdate()
    {
        if (_feet.IsPlayerStandingOnGround())
        {
            _lastGroundPosition = transform.position;
        }
    }


    

    bool JLIsRunning;
    IEnumerator jumpLedge()
    {
        JLIsRunning = true;

        currentState = PlayerState.LEDGEJUMPING;
        _playerRigidBody.AddForce(inputVector * 1000.0f);
        _feet.enabled = false;

        //Do animation here (if u want to code it)

        yield return new WaitForSeconds(0.1f);

        transform.position = _lastGroundPosition;

        _playerRigidBody.velocity = new Vector2();
        _feet.enabled = true;
        EventManager.Instance.PlayerDamage(Pitfall.pitDamage);

        yield return new WaitForSeconds(0.5f);
        currentState = PlayerState.MOVING;
        
        
        JLIsRunning = false;
    }

    public static Vector2 PlayerDirection()
    {
        return inputVector;
    }

    void ResetMovement()
    {
        inputVector = Vector2.zero;
    }

    void OnMove(InputValue value)
    {
        if (!canMove)
            return;

        if (GetComponent<Player>().pauseActions)
        {
            inputVector = new Vector2();
            EventManager.Instance?.PlayerMove(inputVector);
            return;
        }

        inputVector = value.Get<Vector2>();
        EventManager.Instance?.PlayerMove(inputVector);
    }


    void OnSloMo(InputValue value)
    {
        SlowMotionActive();
        EventManager.Instance?.PlayerSlowMo();
    }

    public void SlowMotionActive()
    {
        _isSloMo = true;
        _startSlowTime = _slowTime;
        Time.timeScale = _slowAmount;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private void MoveStop(InputAction.CallbackContext context)
    {
        EventManager.Instance?.PlayerMoveStop();
    }

    void OnSlide(InputValue value)
    {
        if (!canDash || !DashActivated || GetComponent<Player>().playerFailState == Player.PlayerFailState.FAILED || GetComponent<Player>().pauseActions)
            return;

        float originalDashDistance = dashDistance;

        StartDashPosition = transform.position;

        BeatTiming hitTiming = PlayerRhythm.Instance.GetBeatTiming(ButtonInput.dash);

        if (dashOnBeat) { 
            switch (hitTiming)
            {
                case BeatTiming.BAD:
                    GetComponentInParent<Player>().Fail();
                    EventManager.Instance.PlayerDash(hitTiming, Vector2.zero);
                    EventManager.Instance.PlayerMiss();
                    return;
            }
        }

        Vector2 direction = new Vector2();

        switch (InputManager.Instance.CurrentController)
        {
            case ControllerType.KEYBOARDANDMOUSE:
                // Get the mouse position in world space
                Vector2 mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                direction = mousePos - (Vector2)transform.position;
                break;
            case ControllerType.GAMEPAD:
                direction = _playerInput.Player.Aim.ReadValue<Vector2>();
                break;
        }

        direction = direction.normalized;


        //Stops you from dashing through walls
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, direction, dashDistance);

        foreach (var s in hit)
        {
            if (s.collider.tag == "Wall")
            {
                dashDistance = ((Vector2)s.point - (Vector2)transform.position).magnitude - 0.1f;
                break;
            }
            if (s.collider.tag == "DashStop")
            {
                dashDistance = ((Vector2)s.point - (Vector2)transform.position).magnitude + 0.5f;
                break;
            }
        }


        Instantiate(_dustTrail, transform.position, Quaternion.identity);
        Instantiate(_dustTrail, transform.position, Quaternion.identity);

        Instantiate(_dustTrail, transform.position, Quaternion.identity);
        Instantiate(_blink, transform.position, Quaternion.identity);



        transform.position = (Vector2)transform.position + direction * dashDistance;

        for (int i = 0; i < dashDistance; i+=2)
        {
            AfterImagePool.Instance.GetFromPool(transform, _playerSpriteRenderer, _afterImageOffset).transform.position = (Vector3)StartDashPosition + ((Vector3)direction * i) + _afterImageOffset;
        }
        EventManager.Instance.PlayerDash(hitTiming, direction);

        dashDistance = originalDashDistance;

        EventManager.Instance.Dash();
    }

    private void StartPushBack(Vector3 pos)
    {
        Vector2 dir = (transform.position - pos).normalized;
        if (dir == Vector2.zero) { dir = Vector2.down; }

        currentState = PlayerState.PUSHED_BACK;

        if (!GetComponent<Player>().isDead && gameObject.activeSelf == true)
        {
            StartCoroutine(PushBack());

            _playerRigidBody.velocity = dir * _pushBackSpeed;
        }
    }
    IEnumerator PushBack()
    {
        yield return new WaitForSeconds(_pushBackTime);

        currentState = PlayerState.MOVING;
    }

    void SlowMotion()
    {
        if (!_isSloMo) return;

        _startSlowTime-=Time.deltaTime;
        Player.isSlowmo = true;
        if (_startSlowTime <= 0)
        {
            Player.isSlowmo = false;
            _isSloMo = false;
            Time.timeScale = _normalTime;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            EventManager.Instance.PlayerSlowMoEnd();
        }
    }



    public void EnableDash()
    {
        DashActivated = true;
    }

    //void PlayerTired()
    //{
    //    StartCoroutine(SlideCooldown());
    //}

    //IEnumerator SlideCooldown()
    //{
    //    canDash = false;
    //    while (GetComponent<Player>().isStaminaDepleted)
    //    {
    //        yield return null;
    //    }
    //    canDash = true;
    //}
}
