using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneLoaded : MonoBehaviour
{
    private void Start()
    {
        SaveSystem.Instance.GetData().currentLevel = SceneManager.GetActiveScene().buildIndex;
        print("SAVED THE CURRENT LEVEL: " + SaveSystem.Instance.GetData().currentLevel);
        SaveSystem.Instance.Save();

        Invoke("PlayerPos", 1f);
    }

    private void PlayerPos()
    {
        Vector3 Checkpointposition;

        Checkpointposition.x = SaveSystem.Instance.GetData().currentCheckpointX;
        Checkpointposition.y = SaveSystem.Instance.GetData().currentCheckpointY;
        Checkpointposition.z = SaveSystem.Instance.GetData().currentCheckpointZ;


        UnitManager.Instance.GetPlayer().transform.position = Checkpointposition;

    }
}
