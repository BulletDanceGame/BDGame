using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostMovelist : Movelist
{

    //for SHOOTING
    private List<BulletBag.BulletTypes> bulletPrefabs = new List<BulletBag.BulletTypes>();


    [SerializeField] private Animator anim;

    //For ActionTwo
    [Space]
    [SerializeField] private Transform directionManager;
    [SerializeField] private Transform line;
    [SerializeField] private Transform arrow;



    //For Moving
    private static Vector2[][] positionMatrix = { 
        new Vector2[]{ new Vector2(-14, 14),    new Vector2(-14, 0),    new Vector2(-14, -14) }, //its diagonally flipped
        new Vector2[]{ new Vector2(0, 14),      new Vector2(0, 0),      new Vector2(0, -14) },
        new Vector2[]{ new Vector2(14, 14),     new Vector2(14, 0),     new Vector2(14, -14)  },
    };

    private bool isMoving;
    private Vector2 goalPosition;

    [SerializeField] private Vector2 currentPosIndex;
    private Vector2 currentDirection;
    bool clockwise;


    private void OnEnable()
    {

        EventManager.Instance.OnDeactivateBoss += Deactivate;
        EventManager.Instance.OnActivateBoss += Activate;
    }

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



    private void Shoot()
    {
        if (currentPosIndex.x == 1 || currentPosIndex.y == 1)
        {
            LineShot();
        }
        else
        {
            ArrowShot();
        }

        GetComponent<BoxCollider2D>().enabled = true;
    }


    private void LineShot()
    {
        Vector2 dir = -transform.position; //towards 0,0
        directionManager.up = dir;

        Shooting.ShootInDirection(line, bulletPrefabs);
    }

    private void ArrowShot()
    {

        Vector2 dir = -transform.position; //towards 0,0
        directionManager.up = dir;

        Shooting.ShootInDirection(arrow, bulletPrefabs);
    }



    private void Rise()
    {
        anim.speed = 1/(float)MusicManager.Instance.secondsPerBeat;
        anim.Play("StatueRise");

        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void Fall()
    {
        anim.speed = 1 / (float)MusicManager.Instance.secondsPerBeat;
        anim.Play("StatueFall");

    }

    private void MoveClockwise()
    {
        if (currentPosIndex.x == 1 || currentPosIndex.y == 1)
        {
            //same direction or switch if switching clockwise

            if (clockwise == false)
            {
                currentDirection *= -1;
            }
        }
        else
        {
            if (currentPosIndex == new Vector2(0, 0)) //Top Left Corner
            {
                currentDirection = new Vector2(1, 0); //right
            }
            else if (currentPosIndex == new Vector2(2, 0)) //Top Right Corner
            {
                currentDirection = new Vector2(0, 1); //down
            }
            else if(currentPosIndex == new Vector2(2, 2)) //Bottom Right Corner
            {
                currentDirection = new Vector2(-1, 0); //left
            }
            else if(currentPosIndex == new Vector2(0, 2)) //Bottom Left Corner
            {
                currentDirection = new Vector2(0, -1); //up
            }
        }

        currentPosIndex += currentDirection;

        isMoving = true;
        goalPosition = positionMatrix[(int)currentPosIndex.x][(int)currentPosIndex.y];

        clockwise = true;
    }

    private void MoveCounterClockwise()
    {
        if (currentPosIndex.x == 1 || currentPosIndex.y == 1)
        {
            //same direction or switch if switching clockwise

            if (clockwise == true)
            {
                currentDirection *= -1;
            }
        }
        else
        {
            if (currentPosIndex == new Vector2(0, 0)) //Top Left Corner
            {
                currentDirection = new Vector2(0, -1); //down
            }
            else if (currentPosIndex == new Vector2(2, 0)) //Top Right Corner
            {
                currentDirection = new Vector2(-1, 0); //left
            }
            else if (currentPosIndex == new Vector2(2, 2)) //Bottom Right Corner
            {
                currentDirection = new Vector2(0, -1); //up
            }
            else if (currentPosIndex == new Vector2(0, 2)) //Bottom Left Corner
            {
                currentDirection = new Vector2(1, 0); //right
            }
        }

        currentPosIndex += currentDirection;

        isMoving = true;
        goalPosition = positionMatrix[(int)currentPosIndex.x][(int)currentPosIndex.y];

        clockwise = false;
    }




    private void Update()
    {
        if (isMoving)
        {
            float speed = (14f / (float)MusicManager.Instance.secondsPerBeat) * Time.deltaTime;
            transform.position += new Vector3(currentDirection.x, currentDirection.y * -1, 0) * speed;

            if (Vector3.Distance(transform.position, goalPosition) <= 0.5f)
            {
                transform.position = goalPosition;
                isMoving = false;
            }
        }
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

}
