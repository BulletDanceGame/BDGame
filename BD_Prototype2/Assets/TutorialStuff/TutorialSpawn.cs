using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TutorialSpawn : MonoBehaviour
{
    public GameObject tutorial;
    public Transform spawnPoint;
    public CinemachineTargetGroup targetGroup;

    bool hasSpawned = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawned)
            return;

        tutorial = Instantiate(tutorial, spawnPoint);
        tutorial.transform.parent = null;

        targetGroup.AddMember(tutorial.transform, 1, 1);

        hasSpawned = true;
    }
}
