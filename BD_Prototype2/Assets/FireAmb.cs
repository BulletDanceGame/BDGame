using BulletDance.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAmb : MonoBehaviour
{
    [Header("Dumbass Directional Sfx")]
    public AK.Wwise.Event playFireSFX;
    public AK.Wwise.Event stopFireSFX;

    bool windPlaying = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !windPlaying)
        {
            playFireSFX.Post(gameObject);
            print("eyo");

            windPlaying = true;
        }
    }
}
