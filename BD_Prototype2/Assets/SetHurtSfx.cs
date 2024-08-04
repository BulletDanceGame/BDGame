using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHurtSfx : MonoBehaviour
{
    public int hurtAmount = 40;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            RTPCManager.Instance.SetValue("VOLUME____PlayerDamage", hurtAmount, 0.00000001f, RTPCManager.CurveTypes.linear);
            print("hm?");
        }
    }
}
