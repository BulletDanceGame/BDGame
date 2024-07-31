using UnityEngine;

public class BossEndPhaseHit : MonoBehaviour
{
    [SerializeField]
    private float slowMoDuration, slowMoScale, lastPhaseSlowMoDur, lastPhaseSlowMoScale;

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
        if(!isLastPhase)
            TimeManager.Instance.RequestSlowMo(slowMoDuration, slowMoScale);
        else
            TimeManager.Instance.RequestSlowMo(lastPhaseSlowMoDur, lastPhaseSlowMoScale);

        VFXManager.Instance?.RequestVFX_SlowMoZoom(UnitManager.Instance?.GetBoss()?.transform);
    }
}