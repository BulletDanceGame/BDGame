using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    private CinemachineVirtualCamera camera;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;
    private float shakeTimerDuration;
    private float startingIntensity;

    private void Awake()
    {
        Instance = this;
        camera = GetComponent<CinemachineVirtualCamera>();
        noise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float duration)
    {
        if(noise == null)
            noise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        startingIntensity = intensity;
        shakeTimer = duration;
        shakeTimerDuration = duration;
    }

    private void Update()
    {
        if (shakeTimer <= 0) return;

        shakeTimer -= Time.deltaTime;
        noise.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - shakeTimer / shakeTimerDuration);
        Camera.main.transform.localEulerAngles = new Vector3(0, 0, Camera.main.transform.localEulerAngles.z);
    }

}
