using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFightTriggerSMH : MonoBehaviour
{
    public GameObject musicConductor;
    public GameObject enemyController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            musicConductor.SetActive(true);
            enemyController.SetActive(true);
        }
    }
}