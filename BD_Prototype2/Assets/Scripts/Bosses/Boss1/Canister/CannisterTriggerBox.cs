using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannisterTriggerBox : MonoBehaviour
{

    //Collider2D saved;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    print("hjello" + collision.tag);
    //    if (collision.tag == "Player")
    //    {
    //        if (collision.GetComponent<PlayerTriggerBox>())
    //        {
    //            saved = collision;
    //        }
    //    }
    //}

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    print("hjello" + collision.tag);
    //    if (collision.tag == "Player")
    //    {
    //        if (collision.GetComponent<PlayerTriggerBox>())
    //        {
    //            saved = collision;
    //        }
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    print("jello" + collision.tag);
    //    if (collision.tag == "Player")
    //    {
    //        if (collision.GetComponent<PlayerTriggerBox>())
    //        {
    //            saved = null;
    //            GetComponentInParent<Cannister>().ApplyCollision();
    //        }
    //    }
    //}


    //public bool DidHit()
    //{
    //    if (saved != null)
    //    {
    //        EventManager.Instance.PlayerDamage(10f);
    //        EventManager.Instance.PlayerPushBack(transform.position);
    //        saved.GetComponent<PlayerTriggerBox>().NormalHurtFeedback();
    //        return true;
    //    }

    //    gameObject.SetActive(false);
    //    return false;
    //}
   
}
