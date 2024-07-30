using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindVolumeAdjuster : MonoBehaviour
{
    [Range(0, 100)]
    public int windVolume;
    public float windAdjustmentSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RTPCManager.Instance.SetValue("VOLUME____AmbientComponents", windVolume, windAdjustmentSpeed, RTPCManager.CurveTypes.linear);
    }
}
