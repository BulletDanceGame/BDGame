using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public GameObject fireTrailPrefab;
    public bool isBlackfire;
    private float _firetrailSpawnrate;
    public float Spawnrate;

    

    
    private void Update()
    {       
        SpawnFireTrail();
    }

    public void SpawnFireTrail()
    {
        _firetrailSpawnrate -= Time.deltaTime;
        if(isBlackfire)
        {
            if (_firetrailSpawnrate <= 0)
            {
                GameObject fire = BlackFireBag.Instance.GetFromPool();
                if (fire != null)
                {
                    fire.transform.position = transform.position;
                }
                _firetrailSpawnrate = Spawnrate;
            }
        }
        else
        {
            if (_firetrailSpawnrate <= 0)
            {
                GameObject fire = FireBag.Instance.GetFromPool();
                if (fire != null)
                {
                    fire.transform.position = transform.position;
                }
                _firetrailSpawnrate = Spawnrate;
            }
        }
        
    }

    

    
}
