using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParticle : MonoBehaviour
{
    void Start()
    {
        transform.parent = Camera.main.transform;        
    }

    void LateUpdate()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth/2, 0, 0));
    }
}
