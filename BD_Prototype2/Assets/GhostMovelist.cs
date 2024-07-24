using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
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
    private Vector2 lastPosition;

    [SerializeField] private Vector2 currentPosIndex;
    private Vector2 currentDirection;
    private Vector2 lastPosIndex;
    bool fromMiddle;


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
        if (currentPosIndex == Vector2.one)
        {
            MiddleShot();
        }
        else if (currentPosIndex.x == 1 || currentPosIndex.y == 1)
        {
            LineShot();
        }
        else
        {
            CornerShot();
        }

        GetComponent<BoxCollider2D>().enabled = true;
    }


    private void LineShot()
    {
        Vector2 dir = -transform.position; //towards 0,0
        directionManager.up = dir;

        Shooting.ShootInDirection(line, bulletPrefabs);
    }

    private void CornerShot()
    {
        Vector2 dir = -transform.position; //towards 0,0
        ArrowShot(dir);
        
        //backwards
        //dir = transform.position;
        //ArrowShot(dir);
    }

    private void MiddleShot()
    {
        ArrowShot(new Vector2(1, 1));
        ArrowShot(new Vector2(1, -1));
        ArrowShot(new Vector2(-1, 1));
        ArrowShot(new Vector2(-1, -1));
    }


    private void ArrowShot(Vector2 dir)
    {
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


    private void GetNewDir(bool clockwise)
    {
        if (fromMiddle)
        {
            currentPosIndex = lastPosIndex;
        }


        if (clockwise)
        {
            if (currentPosIndex == new Vector2(0, 0) || currentPosIndex == new Vector2(1, 0)) //Top Left Corner
            {
                currentDirection = Vector2.right;
            }
            else if (currentPosIndex == new Vector2(2, 0) || currentPosIndex == new Vector2(2, 1) ) //Top Right Corner
            {
                currentDirection = Vector2.up;
            }
            else if (currentPosIndex == new Vector2(2, 2) || currentPosIndex == new Vector2(1, 2)) //Bottom Right Corner
            {
                currentDirection = Vector2.left;
            }
            else if (currentPosIndex == new Vector2(0, 2) || currentPosIndex == new Vector2(0, 1)) //Bottom Left Corner
            {
                currentDirection = Vector2.down;
            }
            else
            {
                Debug.Break();
            }
        }
        else //counter
        {
            if (currentPosIndex == new Vector2(0, 0) || currentPosIndex == new Vector2(0, 1)) //Top Left Corner
            {
                currentDirection = Vector2.up;
            }
            else if (currentPosIndex == new Vector2(2, 0) || currentPosIndex == new Vector2(1, 0)) //Top Right Corner
            {
                currentDirection = Vector2.left;
            }
            else if (currentPosIndex == new Vector2(2, 2) || currentPosIndex == new Vector2(2, 1)) //Bottom Right Corner
            {
                currentDirection = Vector2.down;
            }
            else if (currentPosIndex == new Vector2(0, 2) || currentPosIndex == new Vector2(1, 2)) //Bottom Left Corner
            {
                currentDirection = Vector2.right;
            }
            else
            {
                Debug.Break();
            }
        }

        
    }

    private void Move()
    {
        currentPosIndex += currentDirection;

        isMoving = true;
        goalPosition = positionMatrix[(int)currentPosIndex.x][(int)currentPosIndex.y];
        lastPosition = transform.position;

        if (fromMiddle)
        {
            currentDirection = (currentPosIndex - Vector2.one).normalized;
            fromMiddle = false;
        }

    }


    private void MoveClockwise()
    {
        GetNewDir(true);


        Move();

    }

    private void MoveCounterClockwise()
    {
        GetNewDir(false);

        Move();

    }

    

    private void MoveMiddle()
    {
        lastPosIndex = currentPosIndex;

        currentPosIndex = Vector2.one;

        currentDirection = (Vector2.one - lastPosIndex).normalized;

        isMoving = true;
        goalPosition = positionMatrix[(int)currentPosIndex.x][(int)currentPosIndex.y];
        lastPosition = transform.position;

        fromMiddle = true;
    }




    private void Update()
    {
        Moving();
    }

    private void Moving()
    {
        if (isMoving)
        {
            float speed = (14f / (float)MusicManager.Instance.secondsPerBeat) * Time.deltaTime;
            Vector3 dir = new Vector3(currentDirection.x, currentDirection.y * -1, 0);
            transform.position += dir * speed;

            
            if (Vector2.Distance(transform.position, lastPosition) >= Vector2.Distance(goalPosition, lastPosition))
            {
                transform.position = goalPosition;
                isMoving = false;
            }
        }
    }




    //DEACTIVATION

    private void OnDisable()
    {
        EventManager.Instance.OnDeactivateBoss -= Deactivate;
        EventManager.Instance.OnActivateBoss -= Activate;
    }

}
