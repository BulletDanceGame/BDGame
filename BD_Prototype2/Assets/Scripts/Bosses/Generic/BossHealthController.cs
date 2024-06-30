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


    void Update()
    {
        //testing
        if (Input.GetKeyDown(KeyCode.U))
            EventManager.Instance.BossDamage(debugDamage);
    }


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


    // -- Boss gets hit -- //
    void OnTriggerEnter2D(Collider2D collision)
    {
        ////Escape if tag is not Deflected nor Perfect
        //if(collision.gameObject.tag != "DeflectedBullet")// && 
        //   //collision.gameObject.tag != "PerfectBullet")
        //    return;

        if (collision.GetComponent<Bullet>() == null) { return; }

        Bullet bullet = collision.GetComponent<Bullet>(); //Replacing GetComponent() with Bullet var

        if (bullet.type == BulletType.BOSSBULLET)
            return;

        //Prevent boss from getting over-hit
        if (!_isActive) return;
        if (_bulletsHit > _maxHits)
        {
            bullet.Deactivate();
            gameObject.GetComponent<Collider2D>().enabled = false;
            StartCoroutine(TurnOnHitbox());
            return;
        }


        //Boss gets hit by bullet

        //Add score
        if(collision.gameObject.tag == "DeflectedBullet")
            EventManager.Instance.AddScore(100);

        if(collision.gameObject.tag == "PerfectBullet")
            EventManager.Instance.AddScore(200);

        //Add bullet hits
        _bulletsHit++;
        StartCoroutine(ResetHit());

        //Cache bullet damage to calculate if boss is at last hit
        _bulletDamage = bullet.GetDamage();

        //Take damage && do phase change (see TakeDamage method)
        EventManager.Instance.BossDamage(_bulletDamage);
        bullet.Deactivate();
    }


    public void TakeDamage(float damage)
    {
        // -- Take Damage-- //
        currentPhaseHealth -= damage;
        _animHandler?.Hurt();

        // -- Phase change -- //
        if(currentPhaseHealth > 0f) return;   //Escape if health > 0

        EventManager.Instance.BossEndPhaseHit(isLastPhase);
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