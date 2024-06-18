using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalking : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] float range;
    [SerializeField] Vector2 maxPosition;
    [SerializeField] Vector2 minPosition;

    Vector2 wayPoint;
    public bool isMoving;

    //For Animation testing, pausing the boss movement and return to idle animation
    //REMINDER: Keep if needed later
    float QUEUETime = 0f;
    [SerializeField] float QUEUEDuration = 5f;
    [SerializeField] float minDistance = 6f;

    private bool _pauseWalking;

    //Animation
    private BulletDance.Animation.UnitAnimationHandler _animHandler;


    void Start()
    {
        //Can't find?
        //anim = gameObject.GetComponentInChildren<Boss1Animator>();

        //For Animation testing, pause walking to see if idle > walk transition works
        //REMINDER: Keep if needed later
        QUEUETime = QUEUEDuration;

        // find a place to go to on start
        SetNewDestination();

        isMoving = true;

        //EventManager.Instance.OnDeactivateBoss += PauseWalking;
        //EventManager.Instance.OnActivateBoss += ResumeWalking;
    }


    void Update()
    {
        if (isMoving)
        {
            if (_pauseWalking)
            {
                return;
            }

            //For Animation testing, pause walking to see if idle > walk transition and vice versa works
            //REMINDER: Keep if needed later
            if (QUEUETime > 0f)
            {
                QUEUETime -= Time.deltaTime;

                //Start walk animation when timer is over
                if (QUEUETime <= 0f)
                    _animHandler?.WalkStart();

                return;
            }

            // moving to the ramdomly picked destination
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, wayPoint, speed * Time.deltaTime);

            // upon reaching the destination, find a destination to go to
            if (Vector2.Distance(transform.localPosition, wayPoint) < range)
            {
                //Stop walking animation
                _animHandler?.WalkStop();

                //For Animation testing, pause walking to see if idle > walk transition and vice versa works
                //REMINDER: Keep if needed later
                QUEUETime = QUEUEDuration;

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
        while (Vector2.Distance(transform.localPosition, wayPoint) < minDistance)
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
