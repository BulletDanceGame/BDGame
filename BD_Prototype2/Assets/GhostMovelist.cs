using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostMovelist : Movelist
{

    //for SHOOTING
    private List<BulletBag.BulletTypes> bulletPrefabs = new List<BulletBag.BulletTypes>();



    //For ActionTwo
    [Space]
    [SerializeField] private Transform lineRight;
    [SerializeField] private Transform lineLeft;


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

        if (action.functionName != null)
        {
            bulletPrefabs.Clear();
            bulletPrefabs.AddRange(action.bullets);
            Invoke(action.functionName, 0);
        }
    }



    private void ShootRight()
    {
        Shooting.ShootInDirection(lineRight, bulletPrefabs);
    }

    private void ShootLeft()
    {
        Shooting.ShootInDirection(lineLeft, bulletPrefabs);

    }






    //DEACTIVATION
    public override void Deactivate()
    {
        _isActive = false;

    }

    private void OnDisable()
    {
        EventManager.Instance.OnDeactivateBoss -= Deactivate;
        EventManager.Instance.OnActivateBoss -= Activate;
    }

}
