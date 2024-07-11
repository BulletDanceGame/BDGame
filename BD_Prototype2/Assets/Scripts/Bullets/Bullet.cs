using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum BulletType { BOSSBULLET, PLAYERBULLET}

public class Bullet : MonoBehaviour
{
    [Header("Base bullet properties")]
    public BulletType type = BulletType.BOSSBULLET;

    private float _currentSpeed;
    public float offBeatSpeed;
    public float onBeatSpeed;
    public float onBeatSpeedDuration;
    private float _onBeatSpeedTimer = 0;
    public float lifeTime;
    public float Damage;
    public int bounces = 0;
    Vector2 dir;
    public float bounceDamage;

    public CircleCollider2D circle;

    //Collision
    [SerializeField]
    private bool _isHittable = true; //Swing collision
    public  bool hittable { get{ return _isHittable; } }

    private bool _canCollide = true; //BG object collision
    [Tooltip("Tags of BG Objects to collide with")]
    [SerializeField]
    private string[] _collideBGObjectTag; //Object Tags to collide with


    [Space]
    [Header("Deflected properties")]
    public  float perfectSpeed;
    public  float goodSpeed;

    [Space]
    [SerializeField]
    private BulletFX _fx;


    [SerializeField] private GameObject[] _rhythmMarkers;


    [SerializeField] private Light2D _light;
    [SerializeField] private float _lightIntensity;
    [SerializeField] private ParticleSystem _beatParticles;

    public void FireBullet(Vector3 direction, Vector2 startingPosition)
    {
        ResetBullet();
        gameObject.SetActive(true);

        transform.position = startingPosition;
        transform.up = direction;

        SetMoveDir(transform.up);

        GetComponent<Rigidbody2D>().AddForce(dir * _currentSpeed);

        Invoke("Deactivate", lifeTime);
    }

    public void DeflectBullet(Vector3 direction, float speed)
    {
        SetMoveDir(direction);
        SetSpeed(speed);
        type = BulletType.PLAYERBULLET;

        CancelInvoke();
        Invoke("Deactivate", lifeTime);

        bounces = 0;
    }

    // --- Set & Get --- //
    public float GetDamage() => Damage;
    public Vector2 GetDir() => dir;
    public void SetMoveDir(Vector3 newdir) => dir = newdir;
    public void SetSpeed(float newSpeed) => _currentSpeed = newSpeed;

    // --- Enable & Disable -- //
    public void SetUp(bool isHittable)
    {
        transform.tag   = "Bullet";
        _isHittable     = isHittable;
        _canCollide     = true;

        SetSpeed(offBeatSpeed);
        _fx.SetUp(this);
        _fx.Reset(); 
    }

    private void OnEnable()
    {
        _canCollide     = true;

        SetSpeed(onBeatSpeed);
        _onBeatSpeedTimer = onBeatSpeedDuration;

        Invoke("Deactivate", lifeTime);
        _fx.SetUp(this);

        EventManager.Instance.OnPlayerRhythmBeat += OnBeat;
    }

    public void OnDisable()
    {
        ResetBullet();
        CancelInvoke();

        Damage = 15;

        EventManager.Instance.OnPlayerRhythmBeat -= OnBeat;
    }

    public void Deactivate()
    {
        ResetBullet();

        if (gameObject.activeSelf)
            gameObject.SetActive(false);

        _rhythmMarkers[0].SetActive(false);
        _rhythmMarkers[1].SetActive(false);

        _fx.DestroyFX();
    }

    public void ResetBullet()
    {
        type = BulletType.BOSSBULLET;
        bounces = 0;
        SetSpeed(offBeatSpeed);
        _fx.Reset();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    // --- Collision with BG objects --- //
    // Boss and Player collisions are handled in their respective scripts //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!_canCollide) return;

        if (_collideBGObjectTag.Contains(collision.transform.tag))
        {
            Deactivate();

        }


        

        if (collision.tag == "Turret" || collision.tag == "BouncedSurface")
        {
            //if (type == BulletType.BOSSBULLET)
            //{
            //    Deactivate();
            //    return;

                
            //}

            if (bounces > 5)
            {
                Deactivate();
            }

            CancelInvoke();
            Invoke("Deactivate", lifeTime);

            dir = -(2*(dir.normalized * dir) * dir.normalized-dir);

            bounces++;
            Damage += bounceDamage;
        }
    }



    private void Movement()
    {
        GetComponent<Rigidbody2D>().velocity = dir.normalized * _currentSpeed;

        if (type == BulletType.PLAYERBULLET)
            return;

        _onBeatSpeedTimer -= Time.deltaTime;
        SetSpeed(Mathf.Lerp(offBeatSpeed, onBeatSpeed, (_onBeatSpeedTimer + onBeatSpeedDuration) / (onBeatSpeedDuration * 2)));
        if (_currentSpeed == onBeatSpeed)
        {
            if (_onBeatSpeedTimer <= 0)
            {
                SetSpeed(offBeatSpeed);
            }
        }  
    }

    private void OnBeat(int anticipation)
    {
        SetSpeed(onBeatSpeed);
        _onBeatSpeedTimer = onBeatSpeedDuration;

        _light.intensity = _lightIntensity;

        _beatParticles.Play();
    }


    // --- Deflection hits --- //
    public void BulletHit(BeatTiming timing)
    {
        switch (timing)
        {
            case BeatTiming.PERFECT:
                SetSpeed(perfectSpeed);

                _fx.PerfectFX();

                _rhythmMarkers[0].SetActive(false);
                _rhythmMarkers[1].SetActive(false);

                ScoreManager.Instance.PerfectHits++;
                break;
            case BeatTiming.GOOD:
                SetSpeed(goodSpeed);

                _fx.GoodFX();
                ScoreManager.Instance.GoodHits++;
                break;
            case BeatTiming.BAD:
                Deactivate();
                _fx.MissFX();
                break;
        }
    }

    public void LastHit()
    {
        _canCollide = false;
        Invoke("SpeedUp", 1.0f);

        //_fx.LastHitFX();
    }

    public void EndGameHit()
    {
        _canCollide = false;
        Invoke("SpeedUp", 1.0f);

        _fx.LastHitFX();
    }


    // --- Out-of-camera Indicator --- //
    public void SpawnedOutsideCamera()
    {
        _fx.CreateBulletIndicator();
    }


    //Cutscenes
    public void TutorialCutsceneShot()
    {
        lifeTime = 1f;
        FireBullet(Vector2.right, transform.position);
    }
}