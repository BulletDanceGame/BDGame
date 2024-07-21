using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField]
    private string cutsceneName;
    private bool triggered = false;

    void OnTriggerStay2D(Collider2D cld)
    {
        if(triggered) return;

        if(cld.tag == "Player")
        {
            EventManager.Instance?.StartCutscene(cutsceneName);
            triggered = true;
            HealPlayer();
        }
    }

    void HealPlayer()
    {
        UnitManager.Instance.GetPlayer().GetComponent<Player>().Heal(150);
    }
}
