using UnityEngine;

public class PlayerSwingHitbox : MonoBehaviour
{
    private bool _hasHitBullet = false;
    private bool _hasSoundPlayed = false;
    private Vector2 bulletAngle;
    public int bulletBackSpeed = 1515;

    public static int _hitCombo = 0;

    [Header("Last hit slowmo")]
    [SerializeField] private float slowMoDuration = 1f;
    [SerializeField] private float slowMoScale = 0.01f;
    [SerializeField] private float saigoNoPitchiDuration = 5f;
    bool _isEndHit = false;

    PlayerSwing _playerSwing;

    void Start()
    {
        _playerSwing = GetComponentInParent<PlayerSwing>();
    }


    private void Update()
    {
        bulletAngle = _playerSwing.bulletAngle;
    }

    private void OnDisable()
    {
        //if (!_hasHitBullet)
            //EventManager.Instance.PlayerMiss();

        _hasHitBullet = false;
        _hasSoundPlayed = false;
        _hitCombo = 0;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Bullet" && collision.tag != "Enemy")
            return;


        BeatTiming hitTiming = _playerSwing.hitTiming;


        //BOSS
        if (collision.GetComponent<BossHealthController>())
        {
            if (collision.GetComponent<BossHealthController>().isDead)
            {
                if (hitTiming != BeatTiming.BAD)
                {
                    collision.GetComponent<Rigidbody2D>().velocity = bulletAngle.normalized * 50;
                }
            }
        }



        //BULLET

        //GetComponent() slows things down, replaced with Bullet variable.
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet == null) return;

        _hasHitBullet = true;

        //Escape if unhittable
        if(!bullet.hittable) return;


        if (hitTiming == BeatTiming.BAD && GetComponentInParent<PlayerSwing>().BulletShouldGetDestroyedOnBad)
        {
            bullet.Deactivate();
            return;
        }

        bullet.DeflectBullet(bulletAngle, bulletBackSpeed);

        //Last hit behavior override
        //Condition for player last hit
        LastHitOverride(hitTiming, bullet);

        bullet.BulletHit(hitTiming);


        if (!_hasSoundPlayed)
        {
            EventManager.Instance.PlayerHit(hitTiming);
            _hasSoundPlayed = true;
        }


        if (_isEndHit)
        {
            _isEndHit = false;
            transform.gameObject.SetActive(false);
            return;
        }

        //Max Combo
        if (_hitCombo >= 10)
        {
            if (!ScoreManager.Instance.isMaxCombo) return;

            TimeManager.Instance.RequestSlowMo(slowMoDuration, slowMoScale);
            //VFXManager.Instance?.RequestVFX_SlowMoZoom(UnitManager.Instance?.GetPlayer()?.transform);
            EventManager.Instance.MaxCombo();

            if (_hitCombo >= 10) _hitCombo = 0;        
        }
    }


    private void LastHitOverride(BeatTiming hitTiming, Bullet bullet)
    {
        if(hitTiming == BeatTiming.BAD) return;
        if(BossController.Instance == null) return;
        if(!BossController.Instance.bossHealth.isLastHit) return;

        if(BossController.Instance.bossHealth.isLastPhase)
        {
            switch(BossController.Instance.currentBoss)
            {
                case BossController.Boss.Critter:
                    bullet.CritterEndGameHit();
                    break;

                case BossController.Boss.YokaiHunter:
                    bullet.EndGameHit();
                    VFXManager.Instance?.RequestVFX_SlowMoZoom(UnitManager.Instance?.GetPlayer()?.transform);
                    TimeManager.Instance?.RequestSlowMo(saigoNoPitchiDuration, 0.0000000001f); //brother
                    break;

                default: break;
            }
        }

        else
            bullet.LastHit();
    
        EventManager.Instance.PlayerLastHit(hitTiming);
        _isEndHit = true;
    }
}