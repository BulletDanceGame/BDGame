using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigCritterMovelist : Movelist
{

    //for SHOOTING
    private List<BulletBag.BulletTypes> bulletPrefabs = new List<BulletBag.BulletTypes>();

    //For ActionTwo
    [Space]
    public Transform circleShot;

    private Rigidbody2D _rb;
    private int _jumpdirection;
    private float _jumpTime;
    public float startJumpTime;
    public float JumpSpeed;
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

    public static Spawner Instance;

    public Transform[] SpawnPoint;
    public GameObject[] Critter;
    public GameObject[] SpawnVFX;
    private List<Vector3> GatePos = new List<Vector3>();  //List Usage
    public float SpawnCooldown;
    public float Offset;
    // Start is called before the first frame update
    private bool airborne;
    //Animation
    private BulletDance.Animation.UnitAnimationHandler _animHandler = null;
    private bool _runStateChanged = false, _isPrevRun = false;

    NavMeshAgent agent;

    public GameObject SmallCritter;

    private void OnEnable()
    {
        _isCritterRunning = false;
        _activate = false;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        EventManager.Instance.OnDeactivateBoss += Deactivate;
        EventManager.Instance.OnActivateBoss += Activate;
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

        print("act");

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

    private void CircleShot()
    {

        Shooting.ShootAtPlayer(transform, circleShot, bulletPrefabs);
    }

    private void JumpToThePlayer()
    {        
            _jumpdirection = 1;
            _jumpTime = startJumpTime;
    }

    private void JumpOutOfTheScreen()
    {
        //JumpAnimation
    }

    private void JumpinPlace()
    {
        print("JUMPED");
        _jumpTime = startJumpTime;

        airborne = true;
        _animHandler.SpecialStart(49);
        _isCritterRunning = false;
    }

    private void LandAndCircleShot()
    {
        print("LANDED");
        airborne = true;

        _jumpdirection = 2;
        _jumpTime = startJumpTime;

        _animHandler.SpecialStart(48);
    }

    private void LandAndSpawnCritter()
    {
        airborne = true;

        _jumpdirection = 3;
        _jumpTime = startJumpTime;

        _animHandler.SpecialStart(48);

    }

    private void SpawnSmallCritter()
    {
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {
        print(airborne);
        if (!UnitManager.Instance.GetPlayer())
            return;
        Jump();
        _distanceFromPlayer = Vector2.Distance(UnitManager.Instance.GetPlayer().transform.position, transform.position);
        ChasePlayer();

        //Animation
        if (_animHandler != null) Animate();
    }


    void Animate()
    {
        if (_runStateChanged)
        {
            if (_isCritterRunning) _animHandler.WalkStart();
            else _animHandler.WalkStop();
        }
    }

    void Jump()
    {
        if (_jumpTime <= 0)
        {
            _jumpdirection = 0;
            //airborne=false;
            _animHandler?.SpecialStop();
        }
        else
        { // if dash then check direction start timer and add speed
            _jumpTime -= Time.deltaTime;

            if (_jumpdirection == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, UnitManager.Instance.GetPlayer().transform.position, JumpSpeed * Time.deltaTime);

            }
            else if (_jumpdirection == 2)
            {
                if (_jumpTime <= 0)
                {
                    CircleShot();
                }
            }
            else if (_jumpdirection == 3)
            {
                if (_jumpTime <= 0)
                {
                    SpawnSmallCritter();
                }
            }
        }
    }

    void ChasePlayer()
    {
        int layermask = 1 << 11;
        RaycastHit2D hitup = Physics2D.Raycast(transform.position, transform.up, _distanceFromWall, layermask);
        RaycastHit2D hitdown = Physics2D.Raycast(transform.position, -transform.up, _distanceFromWall, layermask);
        RaycastHit2D hitleft = Physics2D.Raycast(transform.position, -transform.right, _distanceFromWall, layermask);
        RaycastHit2D hitright = Physics2D.Raycast(transform.position, transform.right, _distanceFromWall, layermask);

        if(!airborne)
        {
            if (_distanceFromPlayer > stopAndShootRadius && _activate)
            {
                _isCritterRunning = true;
                _runSpeed += Time.deltaTime * _acceleration;
                if (_runSpeed > Speed) _runSpeed = Speed;
            }
            if (_distanceFromPlayer <= stopAndShootRadius)
            {
                _isCritterRunning = false;
                _runSpeed -= Time.deltaTime * _acceleration;
                if (_runSpeed < 0) _runSpeed = 0;

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
        
    }

    IEnumerator StopChasingTimer()
    {
        _isCritterRunning = false;
        yield return new WaitForSeconds(stopAndShootDuration);
        _isCritterRunning = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopAndShootRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _distanceFromWall);

    }




    //DEACTIVATION
    public override void Deactivate()
    {
        _isActive = false;

    }

    private void OnDisable()
    {
        EventManager.Instance.OnDeactivateBoss -= Deactivate;
        EventManager.Instance.OnActivateBoss -= Activate;
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(SpawnCooldown);

        for (int i = 0; i < SpawnPoint.Length; i++)
        {
            GameObject gate = Instantiate(SpawnVFX[i], SpawnPoint[i].position + new Vector3(Random.Range(-Offset, Offset), Random.Range(-Offset, Offset), Random.Range(-Offset, Offset)), Quaternion.identity);
            GatePos.Add(gate.transform.position);
        }
        yield return new WaitForSeconds(SpawnCooldown);

        for (int i = 0; i < SpawnPoint.Length; i++)
        {
            GameObject enemy = Instantiate(Critter[i], GatePos[i], Quaternion.identity);
        }

        GatePos.Clear();
    }
}