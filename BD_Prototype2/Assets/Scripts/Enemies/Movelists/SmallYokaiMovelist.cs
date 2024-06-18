using System.Collections.Generic;
using UnityEngine;

public class SmallYokaiMovelist : Movelist
{
    [Header("RAMDOM WALKING SETTING")]
    //for Walking
    [SerializeField] protected float speed;
    [SerializeField] protected float range;
    [SerializeField] protected float maxDistance;

    protected Vector2 wayPoint;
    protected Vector2 currentPosition;
    public bool isMoving;

    //For Animation testing, pausing the minions movement and return to idle animation
    //REMINDER: Keep if needed later
    protected float waitTime = 0f;
    [SerializeField] protected float waitDuration = 5f;
    [SerializeField] float minDistance = 6f;

    protected bool _pauseWalking;

    [Header("DASHING SETTING")]
    //For Dashing 
    [Space]
    public bool CanDash;
    protected Rigidbody2D _rb;
    private float _dashTimer;
    public float startDashTime;
    protected float _direction;
    public float dashSpeed;
    private bool _isDasing;
    protected float _dashTime;
    private float _dashTimeLeft;
    [SerializeField]
    private float _distanceFromPlayer;


    [field: Header("Choose 2 directions to randomize between for dash (between 1 to 5)")]
    [field: Header("pick 1 and 3 for random dash between 1 and 2 for example ")]
    [field: SerializeField]
    [field: Header("1 is up, 2 is down, 3 is left, 4 is right")]
    [field: Header("direction1 should be > than direction2")]
    public int direction1;
    public int direction2;




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
    public Transform thirdShot;

    [field: Header("Wall avoiding radius")]
    [field: SerializeField]
    public float distanceToWall;

    protected bool _isActivate;

    private Vector2 playerdirection;
    private Vector2 playerleftdirection;
    private Vector2 playerrightdirection;


    //Animation
    protected BulletDance.Animation.UnitAnimationHandler _animHandler = null;
    protected bool _walkStateChanged = false, _isPrevWalk = false,  _isWalking = false;

    private void OnEnable()
    {

        //----------------WALKING
        waitTime = waitDuration;
        // find a place to go to on start
        SetNewDestination();

        isMoving = true;
        _isActivate = false;

    }

    public override void Activate()
    {
        _isActive = true;
        _isActivate = true;
    }


    protected Dictionary<float, Vector2> _directionLookup = new Dictionary<float, Vector2>
    {
        {0, Vector2.zero},
        {1, Vector2.up},
        {2, Vector2.down},
        {3, Vector2.left},
        {4, Vector2.right},
    };


    public override void Start()
    {
        

        // get rb to add speed in later and also somekind of timer for the dashing period
        _rb = GetComponent<Rigidbody2D>();
        _dashTime = startDashTime;

        if (_isActiveOnStart)
        {
            Activate();
        }


        _animHandler = GetComponentInChildren<BulletDance.Animation.UnitAnimationHandler>();
    }


    ///NEEDS TO BE DEACTIVATED

    public override void Action(Note action)
    {
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

    void Update()
    {


        if (_isActivate)
        {
        RandomWalk();
        Dash();

        }

        //Animation
        if(_animHandler != null) Animate();
    }

    void Animate()
    {
        if(_walkStateChanged)
        {
            if(_isWalking) _animHandler.WalkStart();
            else           _animHandler.WalkStop();
        }
    }



    protected void RandomWalk()
    {
        //ChasePlayer();
        if (!UnitManager.Instance.GetPlayer()) return;


        if (_isActive && isMoving)
        {
            if (_pauseWalking)
            {
                return;
            }

            //For Animation testing, pause walking to see if idle > walk transition and vice versa works
            //REMINDER: Keep if needed later
            if (waitTime > 0f)
            {
                waitTime -= Time.deltaTime;

                //Start walk animation when timer is over
                if (waitTime <= 0f)
                    _isWalking = true;

                return;
            }

        }

        transform.position = Vector2.MoveTowards(transform.position, currentPosition + wayPoint, speed * Time.deltaTime);

        // upon reaching the destination, find a destination to go to
        if (Vector2.Distance(transform.position, currentPosition + wayPoint) < range)
        {
            //Stop walking animation
            _isWalking = false;

            //For Animation testing, pause walking to see if idle > walk transition and vice versa works
            //REMINDER: Keep if needed later
            waitTime = waitDuration;

            SetNewDestination();
        }

        _walkStateChanged = _isPrevWalk != _isWalking;
        _isPrevWalk = _isWalking;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // if hitting a wall, find a new destination
        if (collision.gameObject.tag == "Wall")
        {
            SetNewDestination();
            if (_direction == 5)
            {
                    _direction = 6;
            }
            if (_direction == 6)
            {
                    _direction = 5;
            }

            if (_direction == 7)
            {
                    _direction = 8;
            }
            if (_direction == 8)
            {
                    _direction = 7;
            }
        }
    }
    protected virtual void Dash()
    {
        // if not dashing or dashing ended then switch back to no direction, reset time, stop the dashing
        if (_dashTime <= 0)
        {
            _direction = 0;
            _rb.velocity = Vector2.zero;

            EventManager.Instance?.MinionsDashStop();
        }
        else
        { // if dash then check direction start timer and add speed
            _dashTime -= Time.deltaTime;

            // putting a number for each direction 0 mean none, 1 mean up , 2 down, 3 left, 4 right
            if (_direction <= 4 && _direction > 0)
            {
                Vector2 _dashDirection = _directionLookup[_direction];

                _rb.velocity = _dashDirection * dashSpeed;
                EventManager.Instance?.MinionsDashStart(_dashDirection);

            }
            else if (_direction == 5)
            {
                transform.position = Vector2.MoveTowards(transform.position, UnitManager.Instance.GetPlayer().transform.position, dashSpeed * Time.deltaTime);
            }
            else if (_direction == 6)
            {
                transform.position = Vector2.MoveTowards(transform.position, UnitManager.Instance.GetPlayer().transform.position, -dashSpeed * Time.deltaTime);
            }
            else if(_direction == 7)
            {

                DashToTheLeftOfPlayer();
            }
            else if(_direction == 8)
            {
                DashToTheRightOfPlayer();
            }
        }
    }

    protected virtual void DashTowardsPlayer(float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, UnitManager.Instance.GetPlayer().transform.position, speed * Time.deltaTime);
    }

    protected virtual void DashAwayPlayer(float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, UnitManager.Instance.GetPlayer().transform.position, -speed * Time.deltaTime);
    }

    protected virtual void DashToTheLeftOfPlayer()
    {
        Vector2 playerdirection = (transform.position - UnitManager.Instance.GetPlayer().transform.position).normalized;

        Vector2 playerleftdirection = Vector2.Perpendicular(playerdirection);

        _rb.velocity = playerleftdirection * dashSpeed;
    }

    protected virtual void DashToTheRightOfPlayer()
    {
        Vector2 playerdirection = (transform.position - UnitManager.Instance.GetPlayer().transform.position).normalized;

        Vector2 playerrightdirection = -Vector2.Perpendicular(playerdirection);

        _rb.velocity = playerrightdirection * dashSpeed;
    }


    protected virtual void NormalShot()
    {
        Shooting.ShootAtPlayer(transform, normalShot, bulletPrefabs);
    }

    protected virtual void DoubleShot()
    {
            Shooting.ShootAtPlayer(transform, doubleShot, bulletPrefabs);
    }

    protected virtual void ThirdShot()
    {
        Shooting.ShootAtPlayer(transform, thirdShot, bulletPrefabs);
    }

    private void DashUp()
    {
        _direction = 1;
        _dashTime = startDashTime;
        DashRayCast();
    }

    private void DashDown()
    {
        _direction = 2;
        _dashTime = startDashTime;
        DashRayCast();

    }

    private void DashLeft()
    {
        _direction = 3;
        _dashTime = startDashTime;
        DashRayCast();

    }

    private void DashRight()
    {
        _direction = 4;
        _dashTime = startDashTime;
        DashRayCast();

    }

    private void DashTowardThePlayer()
    {
        _direction = 5;
        _dashTime = startDashTime;
        DashRayCast();

    }

    private void DashAwayFromThePlayer()
    {
        _direction = 6;
        _dashTime = startDashTime;
        DashRayCast();

    }

    private void DashToTheLeftOfThePlayer()
    {
        _direction= 7;
        _dashTime = startDashTime;
        DashRayCast();

    }

    private void DashToTheRightOfThePlayer()
    {
        _direction = 8;
        _dashTime = startDashTime;
        DashRayCast();

    }


    protected void SetNewDestination()
    {
        currentPosition = transform.position;

        float x = Random.Range(-maxDistance, maxDistance);
        float y = Random.Range(-maxDistance, maxDistance);

        // finding a random spot to move to
        wayPoint = new Vector2(x, y);


        //For Animation Testing, sometimes walking distance is too short to see any changes
        //So keep randomizing until the dist is bigger than minDist
        while (Vector2.Distance(transform.position, wayPoint) < minDistance)
        {
            x = Random.Range(-maxDistance, maxDistance);
            y = Random.Range(-maxDistance, maxDistance);
            wayPoint = new Vector2(x, y);
        }
    }


    protected virtual void DashRayCast()
    {
        //if (_direction == 0) return;
        ////loop or recursive function?

        ////raycast when it need to

        ////all of them fail?
        //int layermask = LayerMask.NameToLayer("EnvironmentPathfinding");

        //Vector2 playerdirection = (transform.position - UnitManager.Instance.GetPlayer().transform.position).normalized;
        //RaycastHit2D hitup = Physics2D.Raycast(transform.position, transform.up, distanceToWall, layermask);
        //RaycastHit2D hitdown = Physics2D.Raycast(transform.position, -transform.up, distanceToWall, layermask);
        //RaycastHit2D hitleft = Physics2D.Raycast(transform.position, -transform.right, distanceToWall, layermask);
        //RaycastHit2D hitright = Physics2D.Raycast(transform.position, transform.right, distanceToWall, layermask);

        //List<RaycastHit2D> hits = new List<RaycastHit2D>() { hitup, hitdown, hitleft, hitright };


        ////Check if theres a wall at the wanted direction
        ////No wall, do nothing
        //float _wantedDir = _direction;
        //if (hits[(int)_wantedDir - 1].collider == null)
        //    return;

        ////Check if theres a wall at the other directions
        //for (int i = 0; i < hits.Count; i++)
        //{
        //    //No wall, change to that direction
        //    //Prioritizes raycast list order
        //    if (hits[i].collider == null)
        //    {
        //        _direction = i + 1;
        //        return;
        //    }
        //}

    }

    protected virtual void PlayerBasedRaycast()
    {
        if (_direction < 5) return;
        //loop or recursive function?

        //raycast when it need to

        //all of them fail?
        int layermask = LayerMask.NameToLayer("EnvironmentPathfinding");
        Vector2 playerpos = UnitManager.Instance.GetPlayer().transform.position;
        Vector2 playerdirection = (transform.position - UnitManager.Instance.GetPlayer().transform.position).normalized;
        Vector2 playerleftdirection = Vector2.Perpendicular(playerdirection);
        Vector2 playerrightdirection = -Vector2.Perpendicular(playerdirection);

        RaycastHit2D hitplayer = Physics2D.Raycast(transform.position, playerdirection, distanceToWall, layermask);   //to
        RaycastHit2D hitplayerback = Physics2D.Raycast(transform.position, -playerdirection, distanceToWall, layermask);    //away
        RaycastHit2D hitplayerleft = Physics2D.Raycast(transform.position, playerleftdirection, distanceToWall, layermask);   //to the left
        RaycastHit2D hitplayerright = Physics2D.Raycast(transform.position, playerrightdirection, distanceToWall, layermask);  //to the right       

        if(_direction == 5)
        {
            print(hitplayer.collider);
            if (hitplayer.collider != null)
            {
                _direction = 6;
            }
        }
        else if (_direction == 6)
        {
            print(hitplayer.collider);
            if (hitplayerback.collider != null)
            {
                _direction = 5;
            }
        }

        if (_direction == 7)
        {
            if (hitplayerleft.collider != null)
            {
                _direction = 8;
            }
        }
        else if (_direction == 8)
        {
            if (hitplayerleft.collider != null)
            {
                _direction = 7;
            }
        }

        List<RaycastHit2D> hits = new List<RaycastHit2D>() { hitplayer, hitplayerback, hitplayerleft, hitplayerright };


        //Check if theres a wall at the wanted direction
        //No wall, do nothing
        float _wantedDir = _direction;
        if (hits[(int)_wantedDir - 5].collider == null)
        {
            Debug.Log("NO WALLS AT WANTED DIRECTION BITCH");
            //Debug.Break();
            return;
        }

        //Check if theres a wall at the other directions
        for (int i = 0; i < hits.Count; i++)
        {
            //No wall, change to that direction
            //Prioritizes raycast list order
            if (hits[i].collider == null)
            {
                _direction = i + 5;
                Debug.Log("NO WALLS AT DIRECTION " + _direction);
                //Debug.Break();
                return;
            }
        }

        //Every check fails, walls everywhere omg
        //No dashing alloweddddd CANCEL ABORT
        Debug.Log("WALLS EVERYWHERE I CHECK");
        //Debug.Break();
        //_dashTime = 0;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanceToWall);

        Vector2 playerpos = UnitManager.Instance.GetPlayer().transform.position;
        Vector2 playerdirection = (transform.position - UnitManager.Instance.GetPlayer().transform.position).normalized*distanceToWall;
        Vector2 playerleftdirection = (transform.position - new Vector3(playerpos.x - _distanceFromPlayer, playerpos.y, 0)).normalized*distanceToWall;
        Vector2 playerrightdirection = (transform.position - new Vector3(playerpos.x + _distanceFromPlayer, playerpos.y, 0)).normalized*distanceToWall;

        Gizmos.color = Color.grey;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(playerdirection.x, playerdirection.y));
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position - new Vector3(playerdirection.x, playerdirection.y));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(playerleftdirection.x, playerleftdirection.y));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(playerrightdirection.x, playerrightdirection.y));



    }

}

