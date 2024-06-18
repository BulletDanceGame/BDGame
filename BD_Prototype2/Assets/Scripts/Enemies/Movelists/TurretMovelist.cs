using System.Collections.Generic;
using UnityEngine;

public class TurretMovelist : Movelist
{
    [Header("SHOOTING SETTING")]
    //for SHOOTING
    private List<BulletBag.BulletTypes> bulletPrefabs = new List<BulletBag.BulletTypes>();

    //For ActionOne
    public Transform normalShot;

    //For ActionTwo
    [Space]
    public Transform doubleShot;


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
        }
    }

    private void NormalShot()
    {
        Shooting.ShootAtPlayer(transform, normalShot, bulletPrefabs);
    }

    private void DoubleShot()
    {
        Shooting.ShootAtPlayer(transform, doubleShot, bulletPrefabs);
    }


}

