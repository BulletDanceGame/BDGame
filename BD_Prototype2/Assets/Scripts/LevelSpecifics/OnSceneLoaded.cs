using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneLoaded : MonoBehaviour
{
    public GameObject cutscene1trigger;
    public GameObject cutscene2trigger;

    [Tooltip("For debugging, should be false in build")]
    public bool dontMovePlayerOnStart;

    private void Start()
    {
        cutscene1trigger.SetActive(true);
        cutscene1trigger.SetActive(true);


        SaveSystem.Instance.GetData().currentLevel = SceneManager.GetActiveScene().buildIndex;
        print("SAVED THE CURRENT LEVEL: " + SaveSystem.Instance.GetData().currentLevel);
        SaveSystem.Instance.Save();

        if (dontMovePlayerOnStart == false)
        {
            Invoke("PlayerPos", 1.5f);
        }
    }

    private void PlayerPos()
    {

        Vector3 Checkpointposition;
        
        Checkpointposition.x = SaveSystem.Instance.GetData().currentCheckpointX;
        Checkpointposition.y = SaveSystem.Instance.GetData().currentCheckpointY;
        Checkpointposition.z = SaveSystem.Instance.GetData().currentCheckpointZ;


        UnitManager.Instance.GetPlayer().transform.position = Checkpointposition;
    }

    private void Update()
    {
        //print(SaveSystem.Instance.GetData().hasBat);

        if (cutscene1trigger)
        {
            if (SaveSystem.Instance.GetData().hasplayed1stcutscene)
            {
                cutscene1trigger.SetActive(false);
            }
        }
        if (cutscene2trigger)
        {
            if (SaveSystem.Instance.GetData().hasplayed2stcutscene)
            {
                cutscene2trigger.SetActive(false);
            }
        }

        
    }
}
