using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueCollision : MonoBehaviour
{

    [Header("Adjust this to limit how many times the boss can be hit at once")]
    [SerializeField]
    private int _maxHits = 2;
    private int _bulletsHit = 0;

    [Space]
    [SerializeField]
    private int debugDamage = 20;

    //Last hit tracking
    private float _bulletDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ////Escape if tag is not Deflected nor Perfect
        //if(collision.gameObject.tag != "DeflectedBullet")// && 
        //   //collision.gameObject.tag != "PerfectBullet")
        //    return;

        if (collision.GetComponent<Bullet>() == null) { return; }

        Bullet bullet = collision.GetComponent<Bullet>(); //Replacing GetComponent() with Bullet var

        if (bullet.type == BulletOwner.BOSSBULLET)
            return;

        //Prevent boss from getting over-hit
        if (_bulletsHit > _maxHits)
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
