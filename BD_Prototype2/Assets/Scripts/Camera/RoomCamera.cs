using Cinemachine;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CameraManager.Instance.cameras.Add(gameObject.GetComponent<CinemachineVirtualCamera>());
    }
}
