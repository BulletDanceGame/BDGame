using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialTurret : MonoBehaviour
{
    public Transform shootPoint;

    private void Start()
    {
        EventManager.Instance.OnBeat += Shoot;

        GameObject NormalBullet = BulletBag.instance.FindBullet(BulletBag.BulletTypes.fire);
        NormalBullet.SetActive(true);

        NormalBullet.transform.position = shootPoint.position;
        NormalBullet.transform.up = shootPoint.up;
        NormalBullet.GetComponent<Bullet>().SetMoveDir(NormalBullet.transform.up);
    }

    void Shoot(int beat)
    {
        if (beat % 10 == 0)
        {
            GameObject NormalBullet = BulletBag.instance.FindBullet(BulletBag.BulletTypes.fire);
            NormalBullet.SetActive(true);

            NormalBullet.transform.position = shootPoint.position;
            NormalBullet.transform.up = shootPoint.up;
            NormalBullet.GetComponent<Bullet>().SetMoveDir(NormalBullet.transform.up);
        }
    }
}
