using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwAudioEmitter : MonoBehaviour
{
    public string EventName = "default";
    public string StopEvent = "default";
    private bool IsInCollider = false;
    void Start()
    {
        AkSoundEngine.RegisterGameObj(gameObject);
        //AkSoundEngine.PostEvent("Play_Env1_Vegzone_OutroLoop", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!IsInCollider)
            {
                AkSoundEngine.PostEvent(EventName, gameObject);
                IsInCollider = true;
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (IsInCollider)
            {
                AkSoundEngine.PostEvent(StopEvent, gameObject);
                IsInCollider = false;
            }
        }
            
    }
}
