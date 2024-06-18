using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCurrency : MonoBehaviour
{
    [SerializeField]
    private int _currencyValue;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player") 
        {
            EventManager.Instance.AddCurrency(_currencyValue);
            //magnet man!!!
            Destroy(gameObject);
        }
    }
}
