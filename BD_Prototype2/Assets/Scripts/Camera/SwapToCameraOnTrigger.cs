using Cinemachine;
using UnityEngine;

public class SwapToCameraOnTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera CameraToSwapTo;

    bool SwappedCamera = false;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (SwappedCamera)
                return;

            if (!CameraManager.Instance.IsCameraFollowingPlayer())
            {
                CameraManager.Instance.SwitchToFollowCamera();
            }
            else
            {
                CameraManager.Instance.SwitchCamera(CameraToSwapTo);
            }
            SwappedToNewCamera();

            Invoke("SwappedToNewCamera", 1);
        }
    }

    void SwappedToNewCamera()
    {
        if(!SwappedCamera)
            SwappedCamera = true;
        else
            SwappedCamera = false;
    }
}
