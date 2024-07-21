using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public List<CinemachineVirtualCamera> cameras;

    public CinemachineVirtualCamera bossCamera;
    public CinemachineVirtualCamera EnemyFightCamera;
    public CinemachineVirtualCamera FreeRoamCamera;

    public CinemachineVirtualCamera StartCamera;
    private CinemachineVirtualCamera currentCamera;


    private bool cameraFollowsPlayer = true;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentCamera = StartCamera;

        foreach (var cam in cameras)
        {
            if (cam == currentCamera)
            {
                cam.Priority = 20;
                cameraFollowsPlayer = true;
            }
            else
            {
                cam.Priority = 10;
            }
        }

        InvokeRepeating("findBoss", 0, 0.5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToFollowCamera();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchCamera(bossCamera);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchToFightCamera();
        }
    }

    void findBoss()
    {
        CinemachineTargetGroup targetGroup = bossCamera.GetComponentInChildren<CinemachineTargetGroup>();
        Transform boss = null;

        if (targetGroup == null || UnitManager.Instance.GetBoss() == null)
            return;

        for(int i = 0; i < targetGroup.m_Targets.Length; i++)
        {
            if (targetGroup.m_Targets[i].target != null)
                continue;

            targetGroup.m_Targets[i].target = UnitManager.Instance.GetBoss().transform;
            boss = UnitManager.Instance.GetBoss().transform;
        }

        if (boss != null)
            CancelInvoke();

        //Debug.Log("Finding boss...");

    }

    void findActiveEnemies()
    {
        CinemachineTargetGroup targetGroup = EnemyFightCamera.GetComponentInChildren<CinemachineTargetGroup>();

        if (targetGroup == null)
            return;

        targetGroup.AddMember(UnitManager.Instance.GetPlayer().transform, 0,0);

        foreach (var unit in UnitManager.Instance.ActiveEnemies)
        {
            targetGroup.AddMember(unit.transform, 0, 0);
        }
    }

    public void SwitchCamera(CinemachineVirtualCamera SwappedCamera)
    {
        if(SwappedCamera == FreeRoamCamera)
        {
            SwitchToFollowCamera();
            cameraFollowsPlayer = true;
            return;
        }

        foreach (var cam in cameras)
        {
            if (cam == SwappedCamera)
            {
                cam.Priority = 20;
            }
            else
            {
                cam.Priority = 10;
            }
        }
        cameraFollowsPlayer = false;
    }

    public void SwitchToFightCamera()
    {
        findActiveEnemies();

        foreach (var cam in cameras)
        {
            cam.Priority = 10;
        }

        EnemyFightCamera.Priority = 20;
        cameraFollowsPlayer = false;
    }

    public void SwitchToFollowCamera()
    {
        foreach (var cam in cameras)
        {
            cam.Priority = 10;
        }

        FreeRoamCamera.Priority = 20;
        cameraFollowsPlayer = true;

        if (EnemyFightCamera)
        {
            foreach (var member in EnemyFightCamera.GetComponentInChildren<CinemachineTargetGroup>().m_Targets)
            {
                EnemyFightCamera.GetComponentInChildren<CinemachineTargetGroup>().RemoveMember(member.target.transform);
            }
        }
        
    }

    public bool IsCameraFollowingPlayer()
    {
        return cameraFollowsPlayer;
    }
}
