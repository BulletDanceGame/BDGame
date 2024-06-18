using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    [SerializeField] private float _slowAmount;
    private float _normalTime = 1.0f;

    public void StartSlowMotion()
    {
        Time.timeScale = _slowAmount;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void StopSlowMotion()
    {
        Time.timeScale = _normalTime;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
