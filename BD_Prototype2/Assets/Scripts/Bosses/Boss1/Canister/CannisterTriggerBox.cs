using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannisterTriggerBox : MonoBehaviour
{

    private bool hit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!hit)
            {
                hit = true;
                EventManager.Instance.PlayerDamage(10);
                EventManager.Instance.PlayerPushBack(transform.position);
                ScoreManager.Instance.GotHit++;
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (hit)
            {
                GetComponent<Collider>().isTrigger = false;
                //needs to happen when not hit as well
            }
        }


    }

}
