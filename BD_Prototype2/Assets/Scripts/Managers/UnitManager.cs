using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private GameObject Player;
    public GameObject Boss, PlayerGO;

    public List<GameObject> Enemies = new List<GameObject>();
    public List<GameObject> SurvivalRoomEnemies;
    public List<GameObject> ActiveEnemies;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetPlayer();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnEnable()
    {
        SetPlayer();
        InvokeRepeating("updatePlayerStatus", 5, 5f);
        InvokeRepeating("updateEnemyList", 1, 1f);
    }

    void updateEnemyList()
    {
        for(int i = 0; i < Enemies.Count; i++)
        {
            if(Enemies[i] == null)
                Enemies.RemoveAt(i);
            
        }
    }

    public GameObject GetPlayer()
    {
        if (Player == null)
            return null;
        else
            return Player;

    }

    public GameObject GetBoss()
    {
        if (Player == null)
            return null;
        else
            return Boss;
    }

    public void SetPlayer()
    {
        InvokeRepeating("findPlayer", 0, 0.5f);
    }

    void findPlayer()
    {
        Player = GameObject.Find("Player");

        if (Player != null)
            CancelInvoke("findPlayer");
    }

    void updatePlayerStatus()
    {
        if (Player == null)
            return;

        if (!Player.activeSelf)
            Player = null;
    }

    public void RespawnPlayer()
    {
        SceneManager.Instance.ReloadCurrentScene(1);
        Destroy(Player);

        Player = Instantiate(PlayerGO, CheckpointManager.instance.GetCurrentCheckPoint().transform.position, Quaternion.identity);

        CameraManager.Instance.FreeRoamCamera.Follow = Player.transform;
        CameraManager.Instance.SwitchToFollowCamera();
    }
}
