using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CritterMovelist : Movelist
{

    [field: Header("SHOOTING SETTING")]

    //for SHOOTING
    private List<BulletBag.BulletTypes> bulletPrefabs = new List<BulletBag.BulletTypes>();

    //For ActionOne
    public Transform normalShot;

    //For ActionTwo
    [Space]
    public Transform doubleShot;

    //For ActionThree
    [Space]
    public Transform doubleShotUnHit;

    //For ActionFour
    [Space]
    public Transform circleShot;

    private Rigidbody2D _rb;

    public float stopAndShootRadius;
    public float stopAndShootDuration; // rename this shit
    public float Speed;

    // for speeding up and slowing down 
    // (smoothing so the jump animation don't look like it jumped in place)
    private float _runSpeed = 0, _acceleration = 20f; 


    private bool _isCritterRunning = false;
    private float _distanceFromPlayer;
    public float _distanceFromWall;
    public float _startRadius;

    private bool _activate;

    //Animation
    private BulletDance.Animation.UnitAnimationHandler _animHandler = null;
    private bool _runStateChanged = false, _isPrevRun = false;

    NavMeshAgent agent;

    //For Jumping
    private float _startJumptime;
    private bool _isJumping;
    public float JumpTime;
    public float JumpSpeed;
    private bool _jumpUp;
    private bool JumpDown;
    private float _JumppositionY;
    private float _currentjumpposition;

    private bool _isPushedback;
    [SerializeField] private float _pushbackDuration;
    [SerializeField] private float _pushbackSpeed;


    private void OnEnable()
    {
        _isCritterRunning = false;
        _activate = false;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

    }

    public override void Action(Note action)
    {
        print(action.functionName);
        if (!_isActive)
        {
            return;
        }

        if (action.functionName != null)
        {
            bulletPrefabs.Clear();
            bulletPrefabs.AddRange(action.bullets);
            Invoke(action.functionName, 0);
        }
    }

    public override void Activate()
    {

        _isActive = true;
        _activate = true;

        _rb = GetComponent<Rigidbody2D>();

        StartCoroutine(StopChasingTimer());

        _animHandler = GetComponentInChildren<BulletDance.Animation.UnitAnimationHandler>();
        UnitManager.Instance.ActiveEnemies.Add(gameObject);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

    }

    private void NormalShot()
    {

        Shooting.ShootAtPlayer(transform, normalShot, bulletPrefabs);

    }

    private void CircleShot()
    {
        Shooting.ShootAtPlayer(transform, circleShot, bulletPrefabs);
    }


    private void JumpinPlace()
    {
        _isJumping = true;
        _jumpUp = true;
        _startJumptime = JumpTime;
        JumpUpdate();
        _currentjumpposition=this.transform.position.y;
    }

    private void NormalLand()
    {
        _isJumping = true;
        JumpDown = true;
        _startJumptime = JumpTime;
        JumpUpdate();
    }

    private void LandWithCritterSpawn()
    {
        _isJumping = true;
        JumpDown = true;
        _startJumptime = JumpTime;
        JumpUpdate();

        Spawner.Instance.CritterSpawner();
    }


    private void DoubleShot()
    {
        Shooting.ShootAtPlayer(transform, doubleShot, bulletPrefabs);
    }

    private void DoubleShotUnHit()
    {
        Shooting.ShootAtPlayer(transform, doubleShotUnHit, bulletPrefabs);
    }

    // Update is called once per frame
    void Update()
    {
        if (!UnitManager.Instance.GetPlayer())
            return;

        if (!_isPushedback)
        {
            _distanceFromPlayer = Vector2.Distance(UnitManager.Instance.GetPlayer().transform.position, transform.position);
            ChasePlayer();
        }
        

        //Animation
        if(_animHandler != null) Animate();


    }

    void JumpUpdate()
    {
        if (!_isJumping) return;
        
            if (_startJumptime <= 0)
            {
                _isJumping = false;
                _jumpUp = false;
                _rb.velocity = Vector2.zero;
                _JumppositionY=this.transform.position.y;
}
            else
            { 
                if(_jumpUp)
                {
                _startJumptime -= Time.deltaTime;
                _rb.velocity = Vector2.up * JumpSpeed;
                }         
            }
    }

    void Animate()
    {
        if(_runStateChanged)
        {
            if(_isCritterRunning) _animHandler.WalkStart();
            else                  _animHandler.WalkStop();
        }
    }


    void ChasePlayer()
    {
        int layermask = 1 << 11;
        RaycastHit2D hitup = Physics2D.Raycast(transform.position, transform.up, _distanceFromWall, layermask);
        RaycastHit2D hitdown = Physics2D.Raycast(transform.position, -transform.up, _distanceFromWall, layermask);
        RaycastHit2D hitleft = Physics2D.Raycast(transform.position, -transform.right, _distanceFromWall, layermask);
        RaycastHit2D hitright = Physics2D.Raycast(transform.position, transform.right, _distanceFromWall, layermask);


        if (_distanceFromPlayer > stopAndShootRadius && _activate)
        {
            _isCritterRunning = true;
            _runSpeed += Time.deltaTime * _acceleration;
            if(_runSpeed > Speed) _runSpeed = Speed;
        }
        if (_distanceFromPlayer <= stopAndShootRadius)
        {
            _isCritterRunning = false;
            _runSpeed -= Time.deltaTime * _acceleration;
            if(_runSpeed < 0) _runSpeed = 0;

            StartCoroutine(StopChasingTimer());
        }
        agent.speed = _runSpeed;
        if (_isCritterRunning)
        {
            //commented out cause of causing errors
            agent.SetDestination(UnitManager.Instance.GetPlayer().transform.position);

        }

         
        _runStateChanged = _isPrevRun != _isCritterRunning;
        _isPrevRun = _isCritterRunning;
    }

    IEnumerator StopChasingTimer()
    {
        _isCritterRunning = false;
        yield return new WaitForSeconds(stopAndShootDuration);
        _isCritterRunning = true;
    }

    public IEnumerator Pushback(Vector2 dir)
    {
        _isPushedback = true;
        _rb.velocity = dir * _pushbackSpeed;

        yield return new WaitForSeconds(_pushbackDuration);

        _isPushedback = false;
        _rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopAndShootRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _distanceFromWall);

    }
}
