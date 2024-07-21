using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{

    public static CheckpointManager instance;

    [SerializeField]
    Checkpoint CurrentCheckpoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    public void SetCurrentCheckpoint(Checkpoint checkPoint)
    {
        CurrentCheckpoint = checkPoint;
    }

    public Checkpoint GetCurrentCheckPoint()
    {
        return CurrentCheckpoint;
    }
    
    public void RespawnPlayer()
    {
        UnitManager.Instance.GetPlayer().transform.position = GetCurrentCheckPoint().transform.position;
    }
}
