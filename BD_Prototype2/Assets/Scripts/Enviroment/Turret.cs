using System.Collections.Generic;
using UnityEngine;
using BulletDance.Animation;
using System.Collections;

public class Turret : Movelist
{
    [Header("Turret Specifics")]
    public Transform normalShot;
    public Transform doubleShot;
    private List<BulletBag.BulletTypes> bulletPrefabs = new List<BulletBag.BulletTypes>();

    [Space]
    [SerializeField]
    float turretRespawnTime = 3f;
    [SerializeField]
    bool shouldBeRespawnable = true;

    [Space]
    [SerializeField] private bool _triggeredByButton;
    
    public enum OnActivation { ShootOnce, KeepShooting}
    [SerializeField] private OnActivation _onActivation;


    public enum TurretType { Straight, Homing, Oscillating }
    [SerializeField] private TurretType _turretType;

    [Header("Oscillating Options")]
    [SerializeField] private Transform _direction;
    [SerializeField] private Vector2[] _oscillatingTargetPositions;
    [SerializeField] private int _startingElementOfTargets;
    private int _targetPositionIndex = 0;
    private int _upOrDownList = 1;
    [SerializeField] private float _rotationDuration;
    private Vector2 _targetRotation;
    private bool _rotating;
    [SerializeField] private bool _restartFromFirstPosition;


    //Animation work
    [Space]
    [SerializeField]
    private UnitAnimationHandler _animHandler;

    public override void Start()
    {
        base.Start();

        //rotate oscilating turret to aim at correct target
        if (_turretType == TurretType.Oscillating)
        {
            _targetPositionIndex = _startingElementOfTargets;
            _direction.up = (_oscillatingTargetPositions[_targetPositionIndex] - (Vector2)transform.position).normalized;
            if (!_restartFromFirstPosition)
            {
                if (_targetPositionIndex == _oscillatingTargetPositions.Length - 1)
                {
                    _upOrDownList = -1;
                }
            }
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

            if (_onActivation == OnActivation.ShootOnce)
            {
                _isActive = false;
            }
        }
    }

    private void NormalShot()
    {
        if (_turretType==TurretType.Straight)
        {
            Shooting.ShootInDirection(normalShot, bulletPrefabs);
        }
        else if (_turretType == TurretType.Homing)
        {
            Shooting.ShootAtPlayer(transform,normalShot, bulletPrefabs);
        }
        else if (_turretType == TurretType.Oscillating)
        {
            Shooting.ShootInDirection(normalShot, bulletPrefabs);

            //decide which point to rotate towards
            _targetPositionIndex += _upOrDownList;
            if (_restartFromFirstPosition)
            {
                if (_targetPositionIndex == _oscillatingTargetPositions.Length)
                {
                    _targetPositionIndex = 0;
                }
            }
            else
            {
                if (_targetPositionIndex == 0 || _targetPositionIndex == _oscillatingTargetPositions.Length - 1)
                {
                    _upOrDownList *= -1;
                }
            }
            
            //set target rotation to the point
            Vector2 _nextPosition = _oscillatingTargetPositions[_targetPositionIndex];
            _targetRotation = (_nextPosition - (Vector2)transform.position).normalized;
            _rotating = true;
        }

        _animHandler?.AttackStart();
    }

    private void DoubleShot()
    {
        if (_turretType == TurretType.Straight)
        {
            Shooting.ShootInDirection(doubleShot, bulletPrefabs);
        }
        else if (_turretType == TurretType.Homing)
        {
            Shooting.ShootAtPlayer(transform, doubleShot, bulletPrefabs);

        }
        else if (_turretType == TurretType.Oscillating)
        {
            Shooting.ShootInDirection(doubleShot, bulletPrefabs);

            //decide which point to rotate towards
            _targetPositionIndex += _upOrDownList;
            if (_restartFromFirstPosition)
            {
                if (_targetPositionIndex == _oscillatingTargetPositions.Length)
                {
                    _targetPositionIndex = 0;
                }
            }
            else
            {
                if (_targetPositionIndex == 0 || _targetPositionIndex == _oscillatingTargetPositions.Length - 1)
                {
                    _upOrDownList *= -1;
                }
            }

            //set target rotation to the point
            Vector2 _nextPosition = _oscillatingTargetPositions[_targetPositionIndex];
            _targetRotation = (_nextPosition - (Vector2)transform.position).normalized;
            _rotating = true;
        }

        _animHandler?.AttackStart();
    }



    private void Update()
    {
        if (_turretType == TurretType.Oscillating)
        {
            Rotating();
        }
    }

    private void Rotating()
    {
        if (_isActive && _rotating)
        {
            //Rotate towards target in the set duration
            float speed = 1/_rotationDuration; 
            _direction.up = Vector2.MoveTowards(_direction.up, _targetRotation, Time.deltaTime * speed);
            if ((Vector2)_direction.up == _targetRotation)
            {
                _rotating = false;
            }
        }
    }

    public void ButtonActivate()
    {
        if (!_triggeredByButton) return;

        Activate();
        _animHandler?.Alerted();
    }

    public void ButtonDeactivate()
    {
        if (!_triggeredByButton) return;

        Deactivate();
        _animHandler?.Defeat();
    }

    void OnTriggerEnter2D(Collider2D cld)
    {

        if (cld.GetComponent<Bullet>())
        {
            if (cld.GetComponent<Bullet>().type == BulletOwner.PLAYERBULLET)
            {
                Hit();
            }
        }
        else if (cld.gameObject.tag == "PlayerSwingBox")
        {
            Hit();
        }
        
    }

    private void Hit()
    {
        _animHandler?.Hurt();

        if (_isActive)
            Deactivate();

        if (shouldBeRespawnable)
            StartCoroutine(RespawnTurret());

    }

    IEnumerator RespawnTurret()
    {

        yield return new WaitForSeconds(turretRespawnTime);
        Activate();
        _animHandler?.Alerted();

    }
}
