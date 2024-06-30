using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Boss1MoveList : Movelist
{
    //To get health-dependent bools
    private BossHealthController healthController;

    //for SHOOTING
    private List<BulletBag.BulletTypes> bulletPrefabs = new List<BulletBag.BulletTypes>();

    //For ActionOne
    public Transform singleShot;

    //For ActionTwo
    [Space]
    public Transform tripleShot;

    //For ActionThree
    [Space]
    public Transform shotgunShot;

    //For ActionThree
    [Space]
    public Transform multiShot;

    //For Spiral
    [Space]
    public Transform circleShot;
    private bool _spiralShotActive;
    private float _spiralShotBulletTimer;
    private int _spiralShotIndex = 0;

    //For Dashing 
    [Space]
    private Rigidbody2D _rb;
    private float _dashTimer;
    public float startDashTime;
    private float _direction;
    private int _index;
    public float distanceBetweenImages;
    public float dashSpeed;
    private bool _isDasing;
    private float _dashTime;
    private float _dashTimeLeft;
    public GameObject dashEffect;
    [SerializeField] Vector3 _afterImageOffset;
    public SpriteRenderer Torso;
    public SpriteRenderer Legs;
    private float afterimagetimer;
    [SerializeField]
    private float afterduration;


    //For Boop
    [Space]
    [SerializeField] private int _beatsForBoop;
    [SerializeField] private GameObject _boopObject;
    private bool _isBooping;
    private bool _playerTooClose;
    private int _currentBoopBeat = 0;

    //containers
    [Header("Container Spawning")]
    [Header("Reticle")]
    [SerializeField]
    private GameObject _attackReticlePrefab;
    private List<GameObject> _attackReticles = new List<GameObject>();
    [SerializeField]
    private Vector2 _reticleSize;

    [Range(0.0f, 50f)]
    public float reticleFollowSpeed;

    [Header("Container")]
    [SerializeField]
    GameObject _canister;

    private List<int> _currentContainerBeats = new List<int>();
    [SerializeField] private int _beatsCanisterReticleDuration;
    [SerializeField] private int _beatsCanisterDropQUEUE;

    private bool _wallCheck;

    //Animation
    private BulletDance.Animation.UnitAnimationHandler _animHandler;

    private Dictionary<float, Vector2> _directionLookup = new Dictionary<float, Vector2>
    {
        {0, Vector2.zero},
        {1, Vector2.up},
        {2, Vector2.down},
        {3, Vector2.left},
        {4, Vector2.right},
    };



    public override void Start()
    {
        base.Start();

        healthController = GetComponent<BossHealthController>();
        _animHandler     = GetComponentInChildren<BulletDance.Animation.UnitAnimationHandler>(true);


        // get rb to add speed in later and also somekind of timer for the dashing period
        _rb = GetComponent<Rigidbody2D>();
        _dashTime = startDashTime;
    }

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

        CheckIfPlayerTooClose();


        if (_isBooping)
        {
            BoopUpdate();
        }
        else
        {
            if (action.functionName != null)
            {
                bulletPrefabs.Clear();
                bulletPrefabs.AddRange(action.bullets);
                Invoke(action.functionName, 0);
            }
        }


        //Stopping spiralshot
        if (_spiralShotActive)
        {
            if (action.functionName != "SpiralShot")
            {
                _spiralShotActive = false;
            }
        }

        //Containers
        for (int i = 0; i < _currentContainerBeats.Count; i++)
        {
            _currentContainerBeats[i]++;
            print( "fuck " + _currentContainerBeats[i] + "   " + Time.timeAsDouble);
        }

    }


    //ACTION CALLS
    //SHOOTING
    private void SingleShot()
    {
        Shooting.ShootAtPlayer(transform, singleShot, bulletPrefabs);
        _animHandler?.AttackStart();
    }

    private void ShotgunShot()
    {
        Shooting.ShootAtPlayer(transform, shotgunShot, bulletPrefabs);
        _animHandler?.AttackStart();
    }

    private void CircleShot()
    {
        Shooting.ShootAtPlayer(transform, circleShot, bulletPrefabs);
        _animHandler?.AttackStart();
    }

    private void MultiShot()
    {
        Shooting.ShootAtPlayer(transform, multiShot, bulletPrefabs);
        _animHandler?.AttackStart();
    }

    private void SpiralShot()
    {
        if (!_spiralShotActive)
            _spiralShotActive = true;

        _animHandler?.AttackStart();
    }

    //DASHING
    private void DashUp()
    {
        if (healthController.isLastHit == false)
        {
            DashUpdate();
            _index = 1;
            _dashTime = startDashTime;
            Instantiate(dashEffect, transform.position, Quaternion.identity);

        }
    }

    private void DashDown()
    {
        if (healthController.isLastHit == false)
        {
            DashUpdate();
            _index = 2;
            _dashTime = startDashTime;
            Instantiate(dashEffect, transform.position, Quaternion.identity);

        }
    }

    private void DashLeft()
    {
        if (healthController.isLastHit == false)
        {
            DashUpdate();
            _index = 3;
            _dashTime = startDashTime;
            Instantiate(dashEffect, transform.position, Quaternion.identity);

        }


    }

    private void DashRight()
    {
        if (healthController.isLastHit == false)
        {
            DashUpdate();
            _index = 4;
            _dashTime = startDashTime;
            Instantiate(dashEffect, transform.position, Quaternion.identity);
        }

    }

    //CONTAINER
    void ContainerDrop()
    {
        _currentContainerBeats.Add(0);

        _attackReticles.Add(Instantiate(_attackReticlePrefab, UnitManager.Instance.GetPlayer().transform.position, Quaternion.identity));

        _attackReticles[^1].transform.parent = null;
    }










    //BOOPING
    private void CheckIfPlayerTooClose()
    {
        if (UnitManager.Instance.GetPlayer().GetComponent<Player>().isDead)
            return;
        float dist = (UnitManager.Instance.GetPlayer().transform.position - transform.position).magnitude;
        _playerTooClose = (dist < _boopObject.transform.lossyScale.x / 2);

        if (_playerTooClose)
        {
            Boop();
        }
    }

    private void Boop()
    {
        if (_isBooping == false)
        {
            _isBooping = true;
            _currentBoopBeat = 0;
            Animator anim = _boopObject.GetComponent<Animator>();
            anim.speed = (1f / (float)0.2) / _beatsForBoop;
            anim.Play("BoopFeedback");
        }
    }



    


    //UPDATES
    private void Update()
    {
        SpiralShotUpdate();
        DashUpdate();
        UpdateContainers();
    }

    void SpiralShotUpdate()
    {
        if (_spiralShotActive)
        {
            _spiralShotBulletTimer -= Time.deltaTime;

            if (_spiralShotBulletTimer <= 0)
            {
                Shooting.ShootFromSpecificPoint(circleShot, _spiralShotIndex, bulletPrefabs);
                Shooting.ShootFromSpecificPoint(circleShot, (_spiralShotIndex + circleShot.childCount / 2) % circleShot.childCount, bulletPrefabs);
                _spiralShotIndex++;
                if (_spiralShotIndex == circleShot.childCount)
                {
                    _spiralShotIndex = 0;
                }
                _spiralShotBulletTimer = 0.1f;
            }

        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // if hitting a wall, find a new destination
        if (collision.gameObject.tag == "Wall")
        {
            _wallCheck = true;
            ContactPoint2D contact = collision.contacts[0];
        }
        else
        {
            _wallCheck = false;

        }
    }

    void DashUpdate()
    {
        if (healthController.isDead)
            return;
        if (_wallCheck) return;
        
        if (healthController.isLastHit == false)
        {
            if (_direction == 0)
            {
                _direction = _index;
            }
            else
            {
                // if not dashing or dashing ended then switch back to no direction, reset time, stop the dashing
                if (_dashTime <= 0)
                {
                    _direction = 0;
                    _rb.velocity = Vector2.zero;
                    afterimagetimer = 0;

                    _animHandler?.DashStop();
                }
                else
                { // if dash then check direction start timer and add speed

                    if (afterimagetimer >= afterduration)
                    {
                        AfterImagePool.Instance.GetFromPool(this.transform, Torso, _afterImageOffset);
                        AfterImagePool.Instance.GetFromPool(this.transform, Legs, _afterImageOffset);
                        afterimagetimer = 0;
                    }
                    //AfterImagePool.Instance.GetFromPool();
                    //_lastImageXPos = transform.position.x;
                    afterimagetimer += Time.deltaTime;
                    _dashTime -= Time.deltaTime;

                    // putting a number for each direction 0 mean none, 1 mean up , 2 down, 3 left, 4 right



                    Vector2 _dashDirection = _directionLookup[_direction];

                    _rb.velocity = _dashDirection * dashSpeed;
                    _animHandler?.DashStart(_dashDirection);
                }

            }
            if(_direction!=0)
            {

            }
        }


    }
    void UpdateContainers()
    {

        for (int i = 0; i < _currentContainerBeats.Count; i++)
        {
            if (_currentContainerBeats[i] < _beatsCanisterReticleDuration)
            {
                float _step = reticleFollowSpeed * Time.deltaTime;
                _attackReticles[i].transform.position = Vector2.MoveTowards(_attackReticles[i].transform.position, UnitManager.Instance.GetPlayer().transform.position, _step);
                _attackReticles[i].transform.localScale = Vector2.Lerp(_attackReticles[i].transform.localScale, _reticleSize, Time.deltaTime);
            }
            else if (_currentContainerBeats[i] >= _beatsCanisterDropQUEUE)
            {
                Instantiate(_canister, _attackReticles[i].transform.position, Quaternion.identity);

                Destroy(_attackReticles[i]);
                _attackReticles.RemoveAt(i);

                _currentContainerBeats.RemoveAt(i);
                i--;
            }
        }
    }

    private void BoopUpdate()
    {
        _currentBoopBeat++;
        if (_currentBoopBeat >= _beatsForBoop)
        {
            if (_playerTooClose)
            {
                EventManager.Instance.PlayerPushBack(transform.position);
            }
            _isBooping = false;
        }
    }




    //CUTSCENE SPAWNING BULLETS
    int nr = 0;
    private List<GameObject> bulletsQUEUEing = new List<GameObject>();

    public void SpawnBulletsForCutscene()
    {
        nr++;
        BulletBag.BulletTypes type;
        if (nr == 1) { type = BulletBag.BulletTypes.normal; }
        else if (nr == 2) { type = BulletBag.BulletTypes.unhittable; }
        else if (nr == 3) { type = BulletBag.BulletTypes.fire; }
        else if (nr == 4) { type = BulletBag.BulletTypes.normal; }
        else { type = BulletBag.BulletTypes.unhittable; }


        bulletPrefabs.Clear();
        for (int i = 0; i < 16; i++)
        {
            bulletPrefabs.Add(type);
        }
        ShootButStill(circleShot);

        _animHandler?.AttackStart();
    }

    void ShootButStill(Transform shootingPattern)
    {
        shootingPattern.up = UnitManager.Instance.GetPlayer().transform.position - transform.position;

        for (int i = 0; i < shootingPattern.childCount; i++)
        {
            GameObject bullet = BulletBag.instance.FindBullet(bulletPrefabs[i]);

            if (bullet == null)
            {
                continue;
            }

            bullet.GetComponent<Bullet>().lifeTime = 100;
            bullet.SetActive(true);
            bullet.transform.position = shootingPattern.GetChild(i).position;
            bullet.transform.up = shootingPattern.GetChild(i).up.normalized;
            bullet.transform.Rotate(Vector3.forward, nr * 11.25f);
            bullet.transform.position += bullet.transform.up * 2 * nr;
            bullet.GetComponent<Bullet>().SetMoveDir(bullet.transform.up);
            bullet.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            bulletsQUEUEing.Add(bullet);
        }
    }

    public void ShootStillBullets()
    {
        for (int i = 0; i < bulletsQUEUEing.Count; i++)
        {
            bulletsQUEUEing[i].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }
        bulletsQUEUEing.Clear();
    }



    //DEACTIVATION
    public override void Deactivate()
    {
        _isActive = false;

        //containers
        for (int i = 0; i < _currentContainerBeats.Count; i++)
        {
            Destroy(_attackReticles[0]);
            _attackReticles.RemoveAt(0);
            _currentContainerBeats.RemoveAt(0);
        }

        //spiral
        _spiralShotActive = false;

        //booping
        _isBooping = false;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnDeactivateBoss -= Deactivate;
        EventManager.Instance.OnActivateBoss -= Activate;
    }

    

    
}

