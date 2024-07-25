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

        //UnitManager.Instance.GetPlayer().transform.position = SaveSystem.Instance.GetData().currentCheckpoint.transform.position;
        //print("Checkpoint: " + SaveSystem.Instance.GetData().currentCheckpoint);
    }
}
