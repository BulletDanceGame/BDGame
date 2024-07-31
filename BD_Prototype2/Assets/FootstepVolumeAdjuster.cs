using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepVolumeAdjuster : MonoBehaviour
{
    [Range(0, 100)]
    public int footstepVolume;
    public float footstepAdjustmentSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RTPCManager.Instance.SetValue("VOLUME____PlayerMovement__Footsteps", footstepVolume, footstepAdjustmentSpeed, RTPCManager.CurveTypes.linear);
    }
}
