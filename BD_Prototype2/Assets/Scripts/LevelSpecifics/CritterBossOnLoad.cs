using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterBossOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void PlayerPos()
    {
        Vector3 Checkpointposition;

        Checkpointposition.x = SaveSystem.Instance.GetData().currentCheckpointX;
        Checkpointposition.y = SaveSystem.Instance.GetData().currentCheckpointY;
        Checkpointposition.z = SaveSystem.Instance.GetData().currentCheckpointZ;


        UnitManager.Instance.GetPlayer().transform.position = Checkpointposition;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
