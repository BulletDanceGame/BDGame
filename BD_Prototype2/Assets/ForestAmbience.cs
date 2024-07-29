using BulletDance.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestAmbience : MonoBehaviour
{

    public GameObject audioManagerSigh;

    bool windPlaying = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !windPlaying)
        {
            RTPCManager.Instance.ResetValue("VOLUME____AmbientComponents", 2.5f, RTPCManager.CurveTypes.high_curve);
            audioManagerSigh.GetComponent<GeneralSounds>().PlayForestAmbience();

            windPlaying = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player" && windPlaying)
        {
            audioManagerSigh.GetComponent<GeneralSounds>().StopForestAmbience();

            windPlaying = false;
        }
    }
}
