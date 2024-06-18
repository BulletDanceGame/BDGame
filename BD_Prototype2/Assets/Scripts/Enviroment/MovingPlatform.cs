using UnityEngine;

public class MovingPlatform : Movelist
{
    [Header("Moving Platform Specifics")]
    [SerializeField] private bool _triggeredByButton;
    [SerializeField] private bool _keepMoving;
    [SerializeField] private bool _startMoving;
    private bool _moving;

    [SerializeField] private int _increments;
    [SerializeField] private float _duration;

    [SerializeField] private Transform _endTransform;
    private Vector2 _endPosition;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;
    private Vector2 _incrementTargetPosition;
    private bool _incrementMoving;

    private Transform _player;


    public override void Start()
    {
        base.Start();

        _startPosition = transform.position;
        _endPosition = _endTransform.position;
        Destroy(_endTransform.gameObject);

        if (_startMoving)
        {
            StartMoving(_endPosition);
        }
    }


    void FixedUpdate()
    {
        Moving();  
    }


    public override void Action(Note action)
    {
        if (!_isActive)
        {
            return;
        }

        if (action.functionName != null)
        {
            Invoke(action.functionName, 0);
        }
    }


    private void Move()
    {
        if (_moving)
        {
            Vector2 _otherPosition = (_targetPosition == _startPosition) ? _endPosition : _startPosition;

            _incrementTargetPosition = (Vector2)transform.position + (1f/_increments) * (_targetPosition - _otherPosition);
            _incrementMoving = true;
        }
    }

    private void Moving()
    {
        if (_moving && _incrementMoving)
        {
            Vector2 lastPosition = transform.position;
            Vector2 _otherPosition = (_targetPosition == _startPosition) ? _endPosition : _startPosition;
            float speed = ((1f / _increments) * Vector2.Distance(_targetPosition, _otherPosition)) / _duration;
            transform.position = Vector2.MoveTowards(transform.position, _incrementTargetPosition, Time.fixedDeltaTime * speed);
            if ((Vector2)transform.position == _incrementTargetPosition)
            {
                _incrementMoving = false;
                print(Vector2.Distance(transform.position, _targetPosition));
                print(Vector2.Distance(transform.position, _targetPosition) < 1f);

                if (Vector2.Distance(transform.position, _targetPosition) < 1f)
                {
                    if (_keepMoving)
                    {
                        StartMoving((_targetPosition == _startPosition) ? _endPosition : _startPosition);
                        print("new pos " + _targetPosition);
                    }
                    else
                    {
                        _moving = false;
                    }
                }
            }
            
            //moving player
            if (_player)
            {
                _player.position = _player.position + (transform.position - (Vector3)lastPosition);
            }

        }
    }



    private void StartMoving(Vector2 goal)
    {
        _moving = true;
        _targetPosition = goal;
    }


    private void StopMoving()
    {
        if (_moving)
        {
            if (!_keepMoving)
            {
                StartMoving((_targetPosition == _startPosition) ? _endPosition : _startPosition);
            }
        }
    }


    public void ButtonActivate()
    {
        if (!_triggeredByButton) return;

        //BUTTON ACTIVATE
        if (_keepMoving && _targetPosition != Vector2.zero)
        {
            _moving = true;
        }
        else
        {
            StartMoving(_endPosition);
        }
    }

    public void ButtonDeactivate()
    {
        if (!_triggeredByButton) return;

        //BUTTON DEACTIVATE
        if (_keepMoving)
        {
            _moving = false;
        }
        else
        {
            StartMoving(_startPosition);
        }
    }


    //TRIGGERS
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _player = collision.transform.parent;

            //player is on
            if (!_triggeredByButton)
            {
                if ((_keepMoving && _targetPosition != Vector2.zero) == false)
                {
                    StartMoving(((Vector2)transform.position == _startPosition) ? _endPosition : _startPosition);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _player = null;

            //player fell off
            if (!_triggeredByButton)
            {
                StopMoving();
            }
        }
    }
}
