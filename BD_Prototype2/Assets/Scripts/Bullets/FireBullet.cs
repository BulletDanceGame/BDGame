using UnityEngine;
using System.Collections.Generic;
public class FireBullet : MonoBehaviour
{
    public GameObject fireTrailPrefab;
    private float _firetrailSpawnrate;
    public float Spawnrate;

    TrailRenderer myTrail;
    EdgeCollider2D myCollider;

    static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();

    void Awake()
    {
        myTrail = this.GetComponent<TrailRenderer>();

        GameObject colliderGameObject=new GameObject("TrailCollider",typeof(EdgeCollider2D));
        myCollider = colliderGameObject.GetComponent<EdgeCollider2D>();
        myCollider.isTrigger = true;
        myCollider.tag = "FireTrail";
        //Invoke("Destroy", 4f);
    }
    private void Update()
    {       
        SpawnFireTrail();
        SetColliderPointsFromTrail(myTrail, myCollider);
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

    

    void SetColliderPointsFromTrail(TrailRenderer trail, EdgeCollider2D collider)
    {
        List<Vector2> points = new List<Vector2>();
        //avoid having default points at (-.5,0),(.5,0)
        if (trail.positionCount == 0)
        {
            points.Add(transform.position);
            points.Add(transform.position);
        }
        else for (int position = 0; position < trail.positionCount; position++)
            {
                //ignores z axis when translating vector3 to vector2
                points.Add(trail.GetPosition(position));
            }
        collider.SetPoints(points);
    }

    void OnDestroy()
    {
        Destroy(myCollider.gameObject);
    }

    void Destroy()
    {
        if (myCollider != null)
        {
            myCollider.enabled = false;
            unusedColliders.Add(myCollider);
        }

    }
}
