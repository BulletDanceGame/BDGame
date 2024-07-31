using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hurtsgx : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            RTPCManager.Instance.ResetValue("VOLUME____PlayerDamage", 0.000000001f, RTPCManager.CurveTypes.linear);
            print("what?");
        }
    }
}
