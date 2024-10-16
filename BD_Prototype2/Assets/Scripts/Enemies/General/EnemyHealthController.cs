using System.Collections;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    [Header("Set Health here")]
    
    public float _healthAmount;
    private float _currentHealthAmount;

    [Header("Adjust this to limit how many times the minion can be hit at once")]
    [SerializeField]
    private int _maxHits = 2;
    private int _bulletsHit = 0;
    private bool _isActive;
    [SerializeField]
    private ParticleSystem _hitParticle;

    private RoomController _enemyController;

    private Vector2 _startPosition;

    [Header("Dumbass Directional Sfx")]
    public AK.Wwise.Event hurtSFX;

    private void OnEnable()
    {
        //UnitManager.Instance.Enemies.Add(gameObject);
    }

    void Start()
    {
        _startPosition = transform.position;
        SetUp();
    }    

    public void SetUp()
    {
        _currentHealthAmount = _healthAmount;
        _isActive = true;
        _bulletsHit = 0;
        transform.position = _startPosition;
    }

    private void OnDestroy()
    {
//        EventManager.Instance.OnDeactivateMinions -= Deactivate;
//        EventManager.Instance.OnActivateMinions -= Activate;
    }

    void Update()
    {
        MinionKill();
        //test
        if (Input.GetKeyDown(KeyCode.P))
        {
            MinionTakeDamage(10);
        }
    }
    private void Deactivate()
    {
        _isActive = false;
    }
    private void Activate()
    {
        _isActive = true;
    }

    void MinionKill()
    {
        if(BossController.Instance.bossHealth.isDead)

        {
            MinionTakeDamage(1000);

        }
    }

    public void SetEnemyController(RoomController controller)
    {
        _enemyController = controller;
    }

    IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(1f);
        _bulletsHit = 0;
    }

    IEnumerator TurnOnHitbox()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();

        //Escape if tag is not Deflected
        if (bullet == null) return;
        if (bullet.type == BulletOwner.BOSSBULLET)
            return;
        if (bullet.bulletState == BulletState.NONE) //IDK how this would happen
            return;


        if(bullet.bulletState != BulletState.LASTHIT)
        {
            if (!_isActive) return;
            if (_bulletsHit > _maxHits)
            {
                bullet.Deactivate();
                gameObject.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(TurnOnHitbox());
                return;
            }
        }

        EventManager.Instance.AddScore(100);

        if (collision.gameObject.tag == "PerfectBullet")
            EventManager.Instance.AddScore(200);

        //Add bullet hits
        _bulletsHit++;
        StartCoroutine(ResetHit());

        //Take damage && do phase change (see TakeDamage method)
        MinionTakeDamage(bullet.GetDamage(), bullet.GetDir());

        if(bullet.bulletState != BulletState.LASTHIT)
            bullet.Deactivate();

        Instantiate(_hitParticle, this.transform.position, Quaternion.identity);



        
    }

    public void MinionTakeDamage(float damage, Vector2 dir = default)
    {
        // -- Take Damage-- //
        _currentHealthAmount -= damage;
        transform.GetComponentInChildren<BulletDance.Animation.UnitAnimationHandler>()?.Hurt();
        hurtSFX.Post(gameObject);

        if (_currentHealthAmount <= 0)
        {
            Death();
        }
        else
        {
            //critter pushback
            //i know this is bad code sorry sorry
            if (GetComponent<CritterMovelist>())
            {
                GetComponent<CritterMovelist>().Pushback(dir);
            }
        }


        
    }


    public void Death()
    {
        _isActive = false;
        gameObject.SetActive(false);

        //time for animation
        Invoke("DestroyObject", 0); //0 for now > make animations > put at another number
        //transform.GetComponentInChildren<BulletDance.Animation.UnitAnimationHandler>()?.Defeat();

        Instantiate(_hitParticle, this.transform.position, Quaternion.identity);


        GetComponent<Movelist>().Deactivate();


        if (_enemyController)
        {
            _enemyController.EnemyDeath(gameObject);
        }

    }

    private void DestroyObject()
    {
        Destroy(gameObject, 0);

    }

}
