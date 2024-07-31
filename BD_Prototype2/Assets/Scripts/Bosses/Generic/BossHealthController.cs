using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossPhaseInfo
{
    public float phaseHealth;
    public bool  hideUI  = false;
    public Color UIColor = Color.red;
}

public class BossHealthController : MonoBehaviour
{
    //lol idk where to put this, it's for the boss health bar name
    [SerializeField]
    private string _bossName;
    public  string BossName { get { return _bossName; } }

    [SerializeField]
    public List<BossPhaseInfo> phaseInfo;
    public int   currentPhase { get; private set; } = 0;
    public float currentPhaseHealth { get; private set; }
    public bool  isLastPhase { get { return currentPhase >= phaseInfo.Count - 1; } }


    [Header ("Adjust this to limit how many times the boss can be hit at once")]
    [SerializeField]
    private int _maxHits = 2;
    private int _bulletsHit = 0;
    private bool _isActive;

    [SerializeField] private bool _pushBackPlayer;

    [Space] [SerializeField]
    private int debugDamage = 20;

    //Last hit tracking
    private float _bulletDamage;
    public bool isLastHit { get { return currentPhaseHealth <= _bulletDamage && currentPhaseHealth > 0; } }
    public bool isDead    { get { return currentPhaseHealth <= 0 && currentPhase >= phaseInfo.Count - 1; } }
    //Animation
    private BulletDance.Animation.UnitAnimationHandler _animHandler;

    void Start()
    {
        _animHandler = GetComponentInChildren<BulletDance.Animation.UnitAnimationHandler>(true);
        currentPhase = 0;

        currentPhaseHealth = phaseInfo[currentPhase].phaseHealth;

        EventManager.Instance.OnDeactivateBoss += Deactivate;
        EventManager.Instance.OnActivateBoss   += Activate;

        EventManager.Instance.OnBossDamage      += TakeDamage;
        EventManager.Instance.OnBossPhaseChange += PhaseChange;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDeactivateBoss -= Deactivate;
        EventManager.Instance.OnActivateBoss   -= Activate;

        EventManager.Instance.OnBossDamage      -= TakeDamage;
        EventManager.Instance.OnBossPhaseChange -= PhaseChange;
    }

#if UNITY_EDITOR
    void Update()
    {
        TurnOffBossFightTrigger();
        //testing
        if (Input.GetKeyDown(KeyCode.U))
            EventManager.Instance.BossDamage(debugDamage);
        if (Input.GetKeyDown(KeyCode.I))
            HealDamage(150f);
    }
#endif

    // -- Boss over-hit prevention -- //
    private void Deactivate()
    {
        _isActive = false;
    }
    private void Activate()
    {
        _isActive = true;
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

    
    void TurnOffBossFightTrigger()
    {
        if(isDead)
        {
            SaveSystem.Instance.GetData().bossdeath = isDead;
            SaveSystem.Instance.Save();
        }
    }


    // -- Boss gets hit -- //
    void OnTriggerEnter2D(Collider2D collision)
    {
        ////Escape if tag is not Deflected nor Perfect
        //if(collision.gameObject.tag != "DeflectedBullet")// && 
        //   //collision.gameObject.tag != "PerfectBullet")
        //    return;


        if (_pushBackPlayer)
        {
            if (collision.CompareTag("Player"))
            {
                if (collision.GetComponent<PlayerTriggerBox>())
                {
                    EventManager.Instance.PlayerDamage(10);
                    ScoreManager.Instance.GotHit++;
                }
                EventManager.Instance.PlayerPushBack(transform.position);
            }
        }
        

        Bullet bullet = collision.GetComponent<Bullet>(); //Replacing GetComponent() with Bullet var
        if (bullet == null)
            return; 
        if (bullet.type == BulletOwner.BOSSBULLET)
            return;
        if (bullet.bulletState == BulletState.NONE) //IDK how this would happen
            return;

        //Prevent boss from getting over-hit
        //Only apply to normal bullets, last hit should ignore this
        if(bullet.bulletState != BulletState.LASTHIT)
        {
            if (!_isActive) return;
            if (_bulletsHit >= _maxHits)
            {
                bullet.Deactivate();
                gameObject.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(TurnOnHitbox());
                return;
            }
        }


        //Boss gets hit by bullet


        //Add score
        int score = bullet.bulletState == BulletState.LASTHIT ? 300 :
                    bullet.bulletState == BulletState.PERFECT ? 200 : 
                    bullet.bulletState == BulletState.GOOD    ? 100 : 0;

        EventManager.Instance.AddScore(score);


        //Add bullet hits
        _bulletsHit++;
        StartCoroutine(ResetHit());


        //Cache bullet damage to calculate if boss is at last hit
        _bulletDamage = bullet.GetDamage();

        //Take damage && do phase change (see TakeDamage method)
        EventManager.Instance.BossDamage(_bulletDamage);
        bullet.Deactivate();
    }

    public void HealDamage(float damage)
    {
        currentPhaseHealth += damage;
        EventManager.Instance.BossHeal(damage);
    }


    public void TakeDamage(float damage)
    {
        // -- Take Damage-- //
        currentPhaseHealth -= damage;
        _animHandler?.Hurt();

        // -- Phase change -- //
        if(currentPhaseHealth > 0f) return;   //Escape if health > 0

        print("deactivate " + isLastPhase + " - " + currentPhase);
        bool fuckoff = isLastPhase;
        EventManager.Instance.BossEndPhaseHit(fuckoff);
        EventManager.Instance.BossPhaseChange();
        if(isDead) 
        {
            EventManager.Instance.BossDeath();
            _animHandler?.Defeat();
        }
    }

    void PhaseChange()
    {
        currentPhase++;
        if(currentPhase < phaseInfo.Count)
            currentPhaseHealth = phaseInfo[currentPhase].phaseHealth;
    }
}