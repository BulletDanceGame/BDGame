using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBag : MonoBehaviour
{
    public static BulletBag instance;


    public GameObject normalBulletPrefab, unhittableBulletPrefab, fireBulletPrefab, groovyBulletPrefab, explosiveBulletPrefab, unhittableExplosiveBulletPrefab, fireGroovyBulletPrefab, fireExplodyBulletPrefab, groovyExplodyBulletPrefab, groovyExplodyFireBulletPrefab;

    public enum BulletTypes {normal, unhittable, fire, groovy, explosive, firegroovy, fireexplody, groovyexplody, groovyexplodyfire, random, none, unhittableexplody }

    [Header("Bullet Lists")]
    public List<GameObject> allBullets;
    public List<GameObject> normalBulletList;
    public List<GameObject> unhittableBulletList;
    public List<GameObject> unhittableExplodyList;
    public List<GameObject> fireBulletList;
    public List<GameObject> fireexplodyBulletList;
    public List<GameObject> groovyBulletList;
    public List<GameObject> groovyexplodyBulletList;
    public List<GameObject> groovyexplodyfireBulletList;
    public List<GameObject> firegroovyBulletList;
    public List<GameObject> explosiveBulletList;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnBullets(BulletTypes.normal, 15);
        SpawnBullets(BulletTypes.unhittable, 15);
        SpawnBullets(BulletTypes.fire, 10);
        SpawnBullets(BulletTypes.fireexplody, 10); 
        SpawnBullets(BulletTypes.groovy, 10);
        SpawnBullets(BulletTypes.groovyexplody, 10);
        SpawnBullets(BulletTypes.groovyexplodyfire, 10);
        SpawnBullets(BulletTypes.firegroovy, 10);
        SpawnBullets(BulletTypes.explosive, 10);
        SpawnBullets(BulletTypes.unhittableexplody, 10);


        EventManager.Instance.OnDeactivateBoss += DeactivateAllBullets;
    }


    /// <summary> Finds an unused or creates a Bullet from the BulletBag </summary> 
    public GameObject FindBullet(BulletTypes type)
    {
        if (type == BulletTypes.none)
        {
            return null;
        }
        else if (type == BulletTypes.random)
        {
            type = (BulletTypes)Random.Range(0,5);
        }


        List<GameObject> bulletList = GetBulletList(type);
        for (int b = 0; b < bulletList.Count; b++)
        {
            if (bulletList[b].activeSelf == false)
            {
                return bulletList[b];
            }

            if (b == bulletList.Count - 1)
            {
                SpawnBullets(type, 10);
            }
        }

        print("Couldn't Find a Bullet");
        return null;
    }

    void SpawnBullets(BulletTypes type, int amount)
    {
        GameObject prefab = GetBulletPrefab(type);

        for (int i = 0; i < amount; i++)
        {
            GameObject bullet = Instantiate(prefab, transform);
            GetBulletList(type).Add(bullet);

            bullet.name = type.ToString() + "Bullet" + GetBulletList(type).Count;
            bullet.GetComponent<Bullet>().SetUp((type != BulletTypes.unhittable)); //Set up variables & fx before disable

            bullet.SetActive(false);
            allBullets.Add(bullet);
        }
    }

    private GameObject GetBulletPrefab(BulletTypes type)
    {
        if (type == BulletTypes.normal)
        {
            return normalBulletPrefab;
        }
        else if (type == BulletTypes.unhittable)
        {
            return unhittableBulletPrefab;
        }
        else if (type == BulletTypes.fire)
        {
            return fireBulletPrefab;
        }
        else if (type == BulletTypes.fireexplody)
        {
            return fireExplodyBulletPrefab;
        }
        else if (type == BulletTypes.groovy)
        {
            return groovyBulletPrefab;
        }
        else if (type == BulletTypes.groovyexplody)
        {
            return groovyExplodyBulletPrefab;
        }
        else if (type == BulletTypes.groovyexplodyfire)
        {
            return groovyExplodyFireBulletPrefab;
        }
        else if (type == BulletTypes.firegroovy)
        {
            return fireGroovyBulletPrefab;
        }
        else if (type == BulletTypes.unhittableexplody)
        {
            return unhittableExplosiveBulletPrefab;
        }
        else //if (type == BulletTypes.explosive)
        {
            return explosiveBulletPrefab;
        }
    }

    private List<GameObject> GetBulletList(BulletTypes type)
    {
        if (type == BulletTypes.normal)
        {
            return normalBulletList;
        }
        else if (type == BulletTypes.unhittable)
        {
            return unhittableBulletList;
        }
        else if (type == BulletTypes.fire)
        {
            return fireBulletList;
        }
        else if (type == BulletTypes.fireexplody)
        {
            return fireexplodyBulletList;
        }
        else if (type == BulletTypes.groovy)
        {
            return groovyBulletList;
        }
        else if (type == BulletTypes.groovyexplody)
        {
            return groovyexplodyBulletList;
        }
        else if (type == BulletTypes.groovyexplodyfire)
        {
            return groovyexplodyfireBulletList;
        }
        else if (type == BulletTypes.firegroovy)
        {
            return firegroovyBulletList;
        }
        else if (type == BulletTypes.unhittableexplody)
        {
            return unhittableExplodyList;
        }
        else //if (type == BulletTypes.explosive)
        {
            return explosiveBulletList;
        }
    }

    public void DeactivateAllBullets()
    {
        for (int i = 0; i < allBullets.Count; i++) 
        {
            if (allBullets[i] == null)
            {
                allBullets.RemoveAt(i);
                i--;
            }
            else if (allBullets[i].activeSelf)
            {
                allBullets[i].GetComponent<Bullet>().Deactivate();
            }
        }
    }
}
