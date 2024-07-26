using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public GameObject fireTrailPrefab;
    private float _firetrailSpawnrate;
    public float Spawnrate;

    

    
    private void Update()
    {       
        SpawnFireTrail();
    }

    public void SpawnFireTrail()
    {
        _firetrailSpawnrate -= Time.deltaTime;

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
