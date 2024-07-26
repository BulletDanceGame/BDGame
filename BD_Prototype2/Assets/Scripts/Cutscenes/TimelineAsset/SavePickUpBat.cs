using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePickUpBat : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D cld)
    {     
        if (cld.tag == "Player")
        {
            SaveSystem.Instance.GetData().hasBat=true;
            SaveSystem.Instance.GetData().hasplayed2stcutscene=true;
            SaveSystem.Instance.Save();
        }
    }

    
}
