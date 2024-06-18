using System.Collections.Generic;
using UnityEngine;

public class TankyYokaiMovelist : Movelist
{
    [field: Header("SHOOTING SETTING")]
    //for SHOOTING
    private List<BulletBag.BulletTypes> bulletPrefabs = new List<BulletBag.BulletTypes>();

    public Transform circleShot;
    public Transform shootPoint4;
    public float jumpHeight;

    //public float MaxJumpHeight;

    private int _jumpdirection;

    private float _jumpTime;
    public float startJumpTime;

    public float jumpSpeed;
    //public float landSpeed;

    private bool _isActivate;

    private Rigidbody2D _rb;

    float playertop;
    Vector2 playertopposition;

    Vector2 playerdirection;
    Vector2 playerleftdirection;
    Vector2 playerrightdirection;

    public override void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _jumpTime = startJumpTime;

        if (_isActiveOnStart)
        {
            Activate();
        }

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


    private void OnEnable()
    {
        _isActivate = false;
    }


    public override void Activate()
    {
        _isActive = true;
        _isActivate = true; //gay furry porn is nice - def an
    }

    private void CirleShot()
    {
        Shooting.ShootAtPlayer(transform, circleShot, bulletPrefabs);
    }

    private void ShootPoint4()
    {
        Shooting.ShootAtPlayer(transform, shootPoint4, bulletPrefabs);
    }

    private void Landing()
    {

        _jumpdirection = 5;
        _jumpTime = startJumpTime;
    }

    private void JumpToPlayer()
    {

        _jumpdirection = 1;
        _jumpTime = startJumpTime;

    }

    private void JumpAwayFromPlayer()
    {

        _jumpdirection = 2;
        _jumpTime = startJumpTime;


    }

    private void JumpToTheLeftOfPlayer()
    {

        _jumpdirection = 3;
        _jumpTime = startJumpTime;


    }

    private void JumpToTheRightOfPlayer()
    {

        _jumpdirection = 4;
        _jumpTime = startJumpTime;
    }

    void Jump()
    {
        float playertop = UnitManager.Instance.GetPlayer().transform.position.y + jumpHeight;
        Vector2 playertopposition = new Vector2(UnitManager.Instance.GetPlayer().transform.position.x, playertop);

        Vector2 playerdirection = (new Vector3(UnitManager.Instance.GetPlayer().transform.position.x-transform.position.x, playertop, 0));
        Vector2 playerleftdirection = Vector2.Perpendicular(playerdirection);
        Vector2 playerrightdirection = -Vector2.Perpendicular(playerdirection);



        // if not dashing or dashing ended then switch back to no direction, reset time, stop the dashing
        if (_jumpTime <= 0)
        {           
            _jumpdirection = 0;
            _rb.velocity = Vector2.zero;
        }
        else
        { // if dash then check direction start timer and add speed
            _jumpTime -= Time.deltaTime;

            if (_jumpdirection == 1)
            {
                _rb.velocity = Vector2.up * jumpSpeed;
                transform.position = Vector2.MoveTowards(transform.position, playertopposition, jumpSpeed * Time.deltaTime);

            }
            else if (_jumpdirection == 2)
            {
                _rb.velocity = Vector2.up * jumpSpeed;
                transform.position = Vector2.MoveTowards(transform.position, playertopposition, -jumpSpeed * Time.deltaTime);
            }
            else if( _jumpdirection == 3)
            {
                _rb.velocity = Vector2.up * jumpSpeed;


            }
            else if(_jumpdirection == 4)
            {
                _rb.velocity = Vector2.up * jumpSpeed;


            }
            else if(_jumpdirection==5)
            {
                _rb.velocity=Vector2.down * jumpSpeed;
                if(_jumpTime <= 0)
                {
                    CirleShot();
                }
            }
        }
    }

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        Jump();
    }
}
