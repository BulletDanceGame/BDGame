using UnityEngine;
using System.Collections.Generic;

public class AllRounderMovelist : SmallYokaiMovelist
{

    //public float distanceToWall;
    public float runAwayRadius;
    public float runSpeed;
    private float _distanceFromThePlayer;
    private bool _isRunningAway=false;

    UnityEngine.AI.NavMeshAgent agent;


    //For animation
    bool _runAwayStateChanged = false, _isPrevRunAway = false;

    // Start is called before the first frame update

    //Start is inherited fomr smallyokaimovelist 

    // Update is called once per frame

    private void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

    }

    void Update()
    {
        if (_isActivate)
        {
            Dash();
            RunAwayFromPlayer();
            RandomWalk();
        }

        //Animation
        if(_animHandler != null) Animate();
    }

    void Animate()
    {
        if(_walkStateChanged || _runAwayStateChanged)
        {
            if(_isWalking || _isRunningAway) 
                _animHandler.WalkStart();
            else
               _animHandler.WalkStop();
        }
    }


    //A* for walking :((

    void RunAwayFromPlayer()
    {
        _distanceFromThePlayer = Vector3.Distance(transform.position, UnitManager.Instance.GetPlayer().transform.position);
        if (_distanceFromThePlayer<=runAwayRadius)
        {
            _isRunningAway=true;
            isMoving = false;
        }
        else
        {
            _isRunningAway=false;
            isMoving = true;
        }

        if(_isRunningAway)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, UnitManager.Instance.GetPlayer().transform.position, -runSpeed * Time.deltaTime);
            //Vector3 runaway =(UnitManager.Instance.GetPlayer().transform.position - this.transform.position)*runSpeed;
            //agent.SetDestination(this.transform.position+runaway);

        }

        //Anim bools, inherited from base class
        _runAwayStateChanged = _isPrevRunAway != _isRunningAway;
        _isPrevRunAway = _isRunningAway;
    }


    private void DashAwayFromThePlayer()
    {
        _direction = 6;
        _dashTime = startDashTime;
        PlayerBasedRaycast();
    }

    void RandomDash()
    {       
        _direction = Random.Range(direction1, direction2);
        _dashTime = startDashTime;
        PlayerBasedRaycast();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // if hitting a wall, find a new destination
        if (collision.gameObject.tag == "Wall")
        {
            SetNewDestination();
        }
    }





    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, runAwayRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanceToWall);

    }

}
