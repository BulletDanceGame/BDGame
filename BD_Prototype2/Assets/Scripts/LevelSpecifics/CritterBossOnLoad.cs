using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterBossOnLoad : MonoBehaviour
{

    public GameObject BossFightTrigger;
    public GameObject CritterRemoverer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    void Update()
    {
        if (SaveSystem.Instance.GetData().bossdeath)
        {
            BossFightTrigger.SetActive(false);
            CritterRemoverer.SetActive(false);
        }
    }
}
