using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoriaEnemyMoveList : Movelist
{
    //for SHOOTING
    private List<BulletBag.BulletTypes> bulletPrefabs = new List<BulletBag.BulletTypes>();
    public Transform singleShot;

    //Animation
    private BulletDance.Animation.UnitAnimationHandler _animHandler;

    public override void Start()
    {
        base.Start();
        _animHandler = GetComponentInChildren<BulletDance.Animation.UnitAnimationHandler>();
    }


    private void SingleShot()
    {
        Shoot(singleShot);
        _animHandler?.AttackStart();
    }

    void Shoot(Transform shootingPattern)
    {
        shootingPattern.up = UnitManager.Instance.GetPlayer().transform.position - transform.position;

        for (int i = 0; i < shootingPattern.childCount; i++)
        {
            GameObject bullet = BulletBag.instance.FindBullet(bulletPrefabs[i]);

            if (bullet == null)
            {
                continue;
            }

            bullet.SetActive(true);
            bullet.transform.position = shootingPattern.GetChild(i).position;
            bullet.transform.up = shootingPattern.GetChild(i).up;
            bullet.GetComponent<Bullet>().SetMoveDir(bullet.transform.up);
        }
    }
}
