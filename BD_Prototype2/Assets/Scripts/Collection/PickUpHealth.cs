using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpHealth : MonoBehaviour
{
    [SerializeField]
    private int _healthValue;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            EventManager.Instance.PlayerHeal(_healthValue);
            EventManager.Instance.PlaySFX("Pick Up Health"); //i am lazy.... maybe we should add this to an event?
            Destroy(gameObject);
        }
    }
}
