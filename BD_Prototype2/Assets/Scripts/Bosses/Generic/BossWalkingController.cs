using UnityEngine;

public class BossWalkingController : MonoBehaviour
{
    public static BossWalkingController Instance { get; private set; }

    [SerializeField] float speed;
    [SerializeField] float range;
    [SerializeField] Vector2 maxPosition;
    [SerializeField] Vector2 minPosition;

    Vector2 wayPoint;
    public bool isMoving;

    //For Animation testing, pausing the boss movement and return to idle animation
    //REMINDER: Keep if needed later
    float pauseTime     = 0f;
    [SerializeField] float pauseDuration = 5f;
    [SerializeField] float minDistance = 6f;

    private bool _pauseWalking;

    //To get health-dependent bools
    private BossHealthController healthController;

    //Animation
    private BulletDance.Animation.UnitAnimationHandler _animHandler;

    void Start()
    {
        _animHandler = GetComponentInChildren<BulletDance.Animation.UnitAnimationHandler>(true);
        healthController = GetComponent<BossHealthController>();

        //For Animation testing, pause walking to see if idle > walk transition works
        //REMINDER: Keep if needed later
        pauseTime = pauseDuration;

        // find a place to go to on start
        SetNewDestination();

        isMoving = true;

        EventManager.Instance.OnDeactivateBoss += PauseWalking;
        EventManager.Instance.OnActivateBoss += ResumeWalking;

        if(_animHandler != null)
        {
            _animHandler.OnDashStart += StopWalking;
            _animHandler.OnDashStop += ResumeWalking;
        }
    }


    void Update()
    {
        if (healthController.isLastHit)
        {
            isMoving = false;
        }
        if (isMoving)
        {
            if (_pauseWalking)
            {
                return;
            }

            //For Animation testing, pause walking to see if idle > walk transition and vice versa works
            //REMINDER: Keep if needed later
            if (pauseTime > 0f)
            {
                pauseTime -= Time.deltaTime;

                //Start walk animation when timer is over
                if (pauseTime <= 0f)
                    _animHandler?.WalkStart();

                return;
            }

            // moving to the ramdomly picked destination
            transform.position = Vector2.MoveTowards(transform.position, wayPoint, speed * Time.deltaTime);

            // upon reaching the destination, find a destination to go to
            if (Vector2.Distance(transform.position, wayPoint) < range)
            {
                //Stop walking animation
                _animHandler?.WalkStop();

                //For Animation testing, pause walking to see if idle > walk transition and vice versa works
                //REMINDER: Keep if needed later
                pauseTime = pauseDuration;

                SetNewDestination();
            }
        }
        
    }

    void SetNewDestination()
    {

        float x = Random.Range(minPosition.x, maxPosition.x);
        float y = Random.Range(minPosition.y, maxPosition.y);

        // finding a random spot to move to
        wayPoint = new Vector2(x, y);


        //For Animation Testing, sometimes walking distance is too short to see any changes
        //So keep randomizing until the dist is bigger than minDist
        while(Vector2.Distance(transform.position, wayPoint) < minDistance)
        {
            x = Random.Range(minPosition.x, maxPosition.x);
            y = Random.Range(minPosition.y, maxPosition.y);
            wayPoint = new Vector2(x, y);
        }
    }



    public void PauseWalking()
    {
        _pauseWalking = true;
    }
    public void StopWalking(Vector2 heh)
    {
        _pauseWalking = true;
    }

    public void ResumeWalking()
    {
        _pauseWalking = false;

    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        // if hitting a wall, find a new destination
        if (collision.gameObject.tag == "Wall")
        {
            SetNewDestination();
        }
    }

}
