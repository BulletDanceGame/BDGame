using UnityEngine;

public class BossEndPhaseHit : MonoBehaviour
{
    [SerializeField]
    private float slowMoDuration, slowMoScale;

    void Start()
    {
        EventManager.Instance.OnBossEndPhaseHit += EndPhaseHit;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnBossEndPhaseHit -= EndPhaseHit;
    }

    void EndPhaseHit(bool isLastPhase)
    {
        EventManager.Instance.DeactivateBoss();

        ScreenShake.Instance.ShakeCamera(20f, 2f);

        //SlowMoFX
        TimeManager.Instance.RequestSlowMo(slowMoDuration, slowMoScale);
        VFXManager.Instance?.RequestVFX_SlowMoZoom(UnitManager.Instance?.GetBoss()?.transform);
    }
}