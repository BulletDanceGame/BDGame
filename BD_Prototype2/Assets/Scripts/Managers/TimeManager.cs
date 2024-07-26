using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    public  float slowMoTime     { get; private set; }
    public  float slowMoDuration { get; private set; }
    private IEnumerator slowMotion = null;
    public  bool  isCurrentlySlowMo { get { return slowMotion != null; } }

    // -- Slow motion -- //
    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; //???????? why 0.02f of time scale wtffff

        if(timeScale == 1f)
            RTPCManager.Instance.ResetValue("PLAYBACK_SPEED____CutsceneMusic", 0.0000000001f, 0);
        else
            RTPCManager.Instance.SetValue("PLAYBACK_SPEED____CutsceneMusic", timeScale, 0.0000000001f, 0);
    }


    //Request Slow mo
    public void RequestSlowMo(float duration, float slowScale)
    {
        slowMoDuration = duration;
        slowMoTime     = slowMoDuration;

        Time.timeScale = slowScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; //???????? why 0.02f of time scale wtffff

        //Start a new coroutine when none existed, else just refresh the current one 
        if(slowMotion == null)
        {
            slowMotion = SlowMotion();
            StartCoroutine(slowMotion);
        }
    }

    //End SlowMo Prematurely, hopefully will never do this, since it will fuck with event calls
    void StopSlowMo()
    {
        StopCoroutine(slowMotion);
        slowMotion = null;

        slowMoDuration = 0f;
        slowMoTime     = 0f;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; //???????? why 0.02f of time scale wtffff

        //reset slo mo rtpc?
    }

    //SlowMo update
    IEnumerator SlowMotion()
    {
        while (slowMoTime > 0f)
        {
            slowMoTime -= Time.unscaledDeltaTime;

            //SlowMo sound
            float value = Mathf.Lerp(0, 100, 1 - (slowMoTime/slowMoDuration));
            //set slo mo rtpc with "value"

            yield return null;
        }

        slowMotion = null;

        slowMoDuration = 0f;
        slowMoTime     = 0f;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; //???????? why 0.02f of time scale wtffff


        //reset slo mo rtpc

        /****
            Enable Cutscene Speed up is called here
            Putting it in cutscene Timeline can cause a bug
                where you can speed up during the slow motion effect
                (bc the timeline does not scale on deltaTime)
        ****/
        EventManager.Instance.EnableSpeedUp();
    }
}