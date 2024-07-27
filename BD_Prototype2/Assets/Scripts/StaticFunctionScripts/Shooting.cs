using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    public static void ShootInDirection(Transform shootingPattern, List<BulletBag.BulletTypes> bulletPrefabs)
    {
        BulletBag.BulletTypes type = BulletBag.BulletTypes.normal;

        for (int i = 0; i < shootingPattern.childCount; i++)
        {
            if (i < bulletPrefabs.Count)
            {
                type = bulletPrefabs[i];
            }
            //else use the last type


            GameObject bullet = BulletBag.instance.FindBullet(type);
            if (bullet == null)
            {
                continue;
            }

            bullet.GetComponent<Bullet>().ShootBullet(shootingPattern.GetChild(i).up, shootingPattern.GetChild(i).position);

            //indicator!
            //outside camera
            //Vector3 camPos = Camera.main.transform.position;
            //if (bullet.transform.position.x > camPos.x + Camera.main.orthographicSize ||
            //    bullet.transform.position.x < camPos.x - Camera.main.orthographicSize ||
            //    bullet.transform.position.y > camPos.y + Camera.main.orthographicSize * (16f / 9f) ||
            //    bullet.transform.position.y < camPos.y - Camera.main.orthographicSize * (16f / 9f))
            //{
            //    bullet.GetComponent<Bullet>().SpawnedOutsideCamera();
            //}
        }
    }


    public static void ShootAtPlayer(Transform shooter, Transform shootingPattern, List<BulletBag.BulletTypes> bulletPrefabs)
    {
        if (UnitManager.Instance.GetPlayer() != null)
        {
            //aim at player
            shootingPattern.up = UnitManager.Instance.GetPlayer().transform.position - shooter.position;
        }

        ShootInDirection(shootingPattern, bulletPrefabs);
    }

    public static void ShootFromSpecificPoint(Transform shootingPattern, int shootPointIndex, List<BulletBag.BulletTypes> bulletPrefabs)
    {
        GameObject bullet = BulletBag.instance.FindBullet(bulletPrefabs[shootPointIndex]);
        if (bullet == null)
        {
            return;
        }

        bullet.SetActive(true);
        bullet.transform.position = shootingPattern.GetChild(shootPointIndex).position;
        bullet.transform.up = shootingPattern.GetChild(shootPointIndex).up;
        bullet.GetComponent<Bullet>().SetMoveDir(bullet.transform.up);

    }
}
