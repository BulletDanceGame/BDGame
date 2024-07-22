using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrailCollider : MonoBehaviour
{

    TrailRenderer myTrail;
    EdgeCollider2D myCollider;

    static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();

    void Awake()
    {
        myTrail = this.GetComponent<TrailRenderer>();
        myTrail.emitting = true;
        GameObject colliderGameObject = new GameObject("TrailCollider", typeof(EdgeCollider2D));
        myCollider = colliderGameObject.GetComponent<EdgeCollider2D>();
        myCollider.isTrigger = true;
        myCollider.tag = "FireTrail";
        //Invoke("Destroy", 4f);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetColliderPointsFromTrail(myTrail, myCollider);

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

    void OnDisable()
    {
        transform.parent = null;
        myTrail.emitting = false;
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
