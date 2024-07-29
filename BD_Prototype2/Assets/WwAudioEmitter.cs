using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwAudioEmitter : MonoBehaviour
{
    public string EventName = "default";
    public string StopEvent = "default";
    
    bool IsPlaying = false;

    private void OnDestroy()
    {
        AkSoundEngine.PostEvent(StopEvent, gameObject);
        IsPlaying = false;
    }

    void Start()
    {
        AkSoundEngine.RegisterGameObj(gameObject);
        //AkSoundEngine.PostEvent("Play_Env1_Vegzone_OutroLoop", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        print("is playing status: " + IsPlaying);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!IsPlaying)
            {
                print("starting music amb");
                AkSoundEngine.PostEvent(EventName, gameObject);
                IsPlaying = true;
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (IsPlaying)
            {
                print("stopping music amb");
                AkSoundEngine.PostEvent(StopEvent, gameObject);
                IsPlaying = false;
            }
        }
            
    }
}
