using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEnemy : MonoBehaviour
{
    int hitByBullet;

    int unhittableShot = 0;

    bool canFire = true;
    int bulletsFired = 0;

    bool fireNormals = true;

    public TextMeshPro bulletDisplayHit;
    public Transform shootPoint;

    enum TutorialStage { FirstStage, SecondStage, ThirdStage};
    TutorialStage stage;

    // Start is called before the first frame update
    IEnumerator Start()
    {

        yield return new WaitForSeconds(5f);
        EventManager.Instance.OnBeat += ShootNormal;
        stage = TutorialStage.FirstStage;
    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        hitByBullet = 0;
        EventManager.Instance.BossSpawn();
    }

    void ShootNormal(int beat)
    {
        if (beat % 4 == 0 && canFire)
        {
            GameObject NormalBullet = BulletBag.instance.FindBullet(BulletBag.BulletTypes.normal);
            NormalBullet.SetActive(true);

            NormalBullet.transform.position = shootPoint.position;
            NormalBullet.transform.up = shootPoint.up;
            NormalBullet.GetComponent<Bullet>().SetMoveDir(NormalBullet.transform.up);

            bulletsFired++;
        }

        if(bulletsFired > 2 && canFire)
        {
            StartCoroutine(PauseShooting());
        }
    }

    IEnumerator PauseShooting()
    {
        canFire = false;
        yield return new WaitForSeconds(3f);
        canFire = true;
        bulletsFired = 0;
    }

    void ShootUnhittable(int beat)
    {
        if (beat % 4 == 0 && unhittableShot <= 3)
        {
            GameObject NormalBullet = BulletBag.instance.FindBullet(BulletBag.BulletTypes.unhittable);
            NormalBullet.SetActive(true);

            NormalBullet.transform.position = shootPoint.position;
            NormalBullet.transform.up = shootPoint.up;
            NormalBullet.GetComponent<Bullet>().SetMoveDir(NormalBullet.transform.up);

            unhittableShot++;
        }

        if(unhittableShot > 3)
        {
            EventManager.Instance.OnBeat -= ShootUnhittable;
            EventManager.Instance.OnBeat += ShootBoth;
            stage = TutorialStage.ThirdStage;
            hitByBullet = 0;
        }
    }

    void ShootBoth(int beat)
    {
        if (beat % 4 == 0 && canFire)
        {
            GameObject bullet;
            if (fireNormals)
            {
                bullet = BulletBag.instance.FindBullet(BulletBag.BulletTypes.normal);
            }
            else
            {
                bullet = BulletBag.instance.FindBullet(BulletBag.BulletTypes.unhittable);
            }

            fireNormals = !fireNormals;

            bullet.SetActive(true);

            bullet.transform.position = shootPoint.position;
            bullet.transform.up = shootPoint.up;
            bullet.GetComponent<Bullet>().SetMoveDir(bullet.transform.up);

            bulletsFired++;
        }

        if (bulletsFired > 2 && canFire)
        {
            StartCoroutine(PauseShooting());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponent<Bullet>())
            return;

        hitByBullet++;
        collision.GetComponent<Bullet>().Deactivate();

        if (hitByBullet >= 3 && stage == TutorialStage.FirstStage)
        {
            EventManager.Instance.OnBeat -= ShootNormal;
            EventManager.Instance.OnBeat += ShootUnhittable;
            stage = TutorialStage.SecondStage;
            canFire = true;
        }
        if(hitByBullet >= 3 && stage == TutorialStage.ThirdStage)
        {
            EventManager.Instance.OnBeat -= ShootBoth;
            Destroy(gameObject);
        }
    }
}
