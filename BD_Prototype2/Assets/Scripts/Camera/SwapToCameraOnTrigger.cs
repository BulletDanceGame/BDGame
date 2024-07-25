using Cinemachine;
using UnityEngine;

public class SwapToCameraOnTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera CameraToSwapTo;

    bool SwappedCamera = false;

    [Header("Toggle on if you want this trigger to be only for exiting from rooms")]
    public bool exitTrigger = false;

    [Header("If you want to use the old system where you only have to touch the trigger")]
    public bool oldSystem = false;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (SwappedCamera)
                return;

            if (exitTrigger)
            {
                CameraManager.Instance.SwitchToFollowCamera();
            }
            else
            {
                CameraManager.Instance.SwitchCamera(CameraToSwapTo);
            }

            if (oldSystem)
            {
                if (!CameraManager.Instance.IsCameraFollowingPlayer())
                {
                    CameraManager.Instance.SwitchToFollowCamera();
                }
                else
                {
                    CameraManager.Instance.SwitchCamera(CameraToSwapTo);
                }
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
