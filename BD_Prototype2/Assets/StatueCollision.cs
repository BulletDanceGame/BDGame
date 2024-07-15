using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueCollision : MonoBehaviour
{
    private float personalHP;

    [Header("Adjust this to limit how many times the boss can be hit at once")]
    [SerializeField]
    private int _maxHits = 2;
    private int _bulletsHit = 0;

    [Space]
    [SerializeField]
    private int debugDamage = 20;

    //Last hit tracking
    private float _bulletDamage;



    private void Start()
    {
        personalHP = GetComponentInParent<BossHealthController>().phaseInfo[1].phaseHealth / 4f;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        ////Escape if tag is not Deflected nor Perfect
        //if(collision.gameObject.tag != "DeflectedBullet")// && 
        //   //collision.gameObject.tag != "PerfectBullet")
        //    return;


        if (collision.CompareTag("Player"))
        {
            EventManager.Instance.PlayerDamage(10);
            EventManager.Instance.PlayerPushBack(transform.position);
            ScoreManager.Instance.GotHit++;
        }


        if (collision.GetComponent<Bullet>() == null) { return; }

        Bullet bullet = collision.GetComponent<Bullet>(); //Replacing GetComponent() with Bullet var

        if (bullet.type == BulletOwner.BOSSBULLET)
            return;

        //Prevent boss from getting over-hit
        if (_bulletsHit >= _maxHits)
        {
            bullet.Deactivate();
            gameObject.GetComponent<Collider2D>().enabled = false;
            StartCoroutine(TurnOnHitbox());
            return;
        }


        //Boss gets hit by bullet

        //Add score
        if (collision.gameObject.tag == "DeflectedBullet")
            EventManager.Instance.AddScore(100);

        if (collision.gameObject.tag == "PerfectBullet")
            EventManager.Instance.AddScore(200);

        //Add bullet hits
        _bulletsHit++;
        StartCoroutine(ResetHit());

        //Cache bullet damage to calculate if boss is at last hit
        _bulletDamage = bullet.GetDamage();

        //Take damage && do phase change (see TakeDamage method)
        EventManager.Instance.BossDamage(_bulletDamage);
        bullet.Deactivate();



        //statues hp
        if (GetComponentInParent<BossHealthController>().currentPhase > 0)
        {
            personalHP -= _bulletDamage;
            if (personalHP <= 0)
            {
                GetComponent<GhostMovelist>().Deactivate();
                GetComponent<Collider2D>().enabled = false;
                transform.Find("EnemiesGraphics").gameObject.SetActive(false);
                transform.Find("Shadow").gameObject.SetActive(false);

            }
        }


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
}
