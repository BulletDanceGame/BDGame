using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Checkpoint : MonoBehaviour
{
    
    private CheckpointManager _checkpointManager;
    PlayerSwing _playerSwing;


    private void Start()
    {
        _checkpointManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CheckpointManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            _playerSwing = collision.GetComponentInParent<PlayerSwing>();
            _checkpointManager.SetCurrentCheckpoint(this);

            SaveSystem.Instance.GetData().currentCheckpointX = UnitManager.Instance.GetPlayer().transform.position.x;
            SaveSystem.Instance.GetData().currentCheckpointY = UnitManager.Instance.GetPlayer().transform.position.y;
            //SaveSystem.Instance.GetData().hasBat = _playerSwing.SwingActivated;
            SaveSystem.Instance.Save();

        }
    }

}
