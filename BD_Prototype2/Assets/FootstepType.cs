using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepType : MonoBehaviour
{
    public bool facility = false;
    public bool outdoors = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (facility == true)
            {
                AkSoundEngine.SetState("Level", "YokaiHunterBoss");
            }
            if (outdoors == true)
            {
                AkSoundEngine.SetState("Level", "Tutorial");
            }
        }
    }
}
