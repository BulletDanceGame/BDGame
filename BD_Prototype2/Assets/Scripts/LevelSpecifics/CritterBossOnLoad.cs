using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterBossOnLoad : MonoBehaviour
{

    public GameObject BossFightTrigger;
    public GameObject CritterRemoverer;

    [Tooltip("For debugging, should be false in build")]
    public bool dontMovePlayerOnStart;

    // Start is called before the first frame update
    void Start()
    {
        if (dontMovePlayerOnStart == false)
        {
            Invoke("PlayerPos", 1.5f);
        }
    }
    private void PlayerPos()
    {

        //Vector3 Checkpointposition;

        //Checkpointposition.x = SaveSystem.Instance.GetData().currentCheckpointX;
        //Checkpointposition.y = SaveSystem.Instance.GetData().currentCheckpointY;
        //Checkpointposition.z = SaveSystem.Instance.GetData().currentCheckpointZ;


        //UnitManager.Instance.GetPlayer().transform.position = Checkpointposition;
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
