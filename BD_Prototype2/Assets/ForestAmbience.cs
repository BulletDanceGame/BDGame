using BulletDance.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestAmbience : MonoBehaviour
{

    public GameObject audioManagerSigh;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            audioManagerSigh.GetComponent<GeneralSounds>().PlayForestAmbience();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            audioManagerSigh.GetComponent<GeneralSounds>().StopForestAmbience();

        }
    }
}
