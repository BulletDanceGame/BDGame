using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDashUp : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float dashSpeed;
    private float _dashTime;
    public float startDashTime;
    private int _direction;
    // Start is called before the first frame update
    void Start()
    {
        // get rb to add speed in later and also somekind of timer for the dashing period
        _rb = GetComponent<Rigidbody2D>();
        _dashTime = startDashTime;
    }

    // Update is called once per frame
    void Update()
    {
        // putting a number for each direction 0 mean none, 1 mean up 
        if(_direction==0)
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                _direction = 1;
            }
        }
        else
        {
            // if not dashing or dashing ended then switch back to no direction, reset time, stop the dashing
            if(_dashTime<=0)
            {
                _direction = 0;
                _dashTime = startDashTime;
                _rb.velocity = Vector2.zero;
            }
            else
            {
                // if dash then´check direction start timer and add speed
                _dashTime -=Time.deltaTime;
                if(_direction==1)
                {
                    _rb.velocity = Vector2.up * dashSpeed;
                }
            }
        }
    }
}
