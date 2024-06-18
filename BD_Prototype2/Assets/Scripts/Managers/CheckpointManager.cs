using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{

    private static CheckpointManager instance;
    public Vector2 LastCheckpointPos;

    private void OnEnable()
    {
        EventManager.Instance.OnPlayerDeath += RespawnPlayer;

    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }
    public void RespawnPlayer()
    {
        UnitManager.Instance.GetPlayer().transform.position = LastCheckpointPos;
    }
}
