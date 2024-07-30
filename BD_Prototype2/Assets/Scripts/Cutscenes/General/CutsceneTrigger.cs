using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField]
    private bool useTrigger = true, firstMiss = false;
    [SerializeField]
    private string cutsceneName;
    private bool triggered = false;

    void Start()
    {
        EventManager.Instance.OnPlayerAttack += StartMissCutsceen;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnPlayerAttack -= StartMissCutsceen;
    }


    void OnTriggerStay2D(Collider2D cld)
    {
        if(triggered) return;
        if(!useTrigger) return;

        if(cld.tag == "Player")
            StartCutsceen();
    }

    void StartCutsceen()
    {
        EventManager.Instance?.StartCutscene(cutsceneName);
        triggered = true;
        HealPlayer();
    }

    void HealPlayer()
    {
        UnitManager.Instance.GetPlayer().GetComponent<Player>().Heal(150);
    }


    void StartMissCutsceen(BeatTiming hitTiming, Vector2 none)
    {
        if (SaveSystem.Instance.GetData().misscutsceneplayed) return;
        if(hitTiming != BeatTiming.BAD) return;
        if(triggered) return;
        if(!useTrigger && firstMiss)
        {
            SaveSystem.Instance.GetData().misscutsceneplayed = true;
            SaveSystem.Instance.Save();
            StartCutsceen();
        }

    }

}
