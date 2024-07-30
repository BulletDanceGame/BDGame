
namespace BulletDance.Audio //Ignore indent of this {} bc that's annoying
{


/*
    This class contains Boss 1 (Yokai Hunter) SFX & rtpc controlls.
    There should be NO REFERENCE this class.

    This is Mo's living space.

    Hey! That's me!
*/
public class Boss1Sounds : SoundContainer
{
    // -- Event Hooks & sound initialization -- //
    void Start()
    {
        //Init
        Initialize(); //So we can get SoundManager.Instance to access PlaySFX & other common helper methods

        //Events
        EventManager.Instance.OnBossEndPhaseHit += EndPhaseHit;
    }

    void OnDestroy()
    {
        DeInitialize();
        EventManager.Instance.OnBossEndPhaseHit -= EndPhaseHit;
    }


    // -- Update -- //


    // -- SFX Implementation -- //
    //Canister Land SFX
    
    void EndPhaseHit(bool isLastPhase)
    {
        //Last hit SFX, Last phase
        if(isLastPhase)
        {
            if (BossController.Instance.currentBoss == BossController.Boss.YokaiHunter)
            {   
                PlaySFX("Final Phase Impact");
                RTPCManager.Instance.ResetAttributeValue("VOLUME", 0.00000000000001f, RTPCManager.CurveTypes.linear);
            }
            else if (BossController.Instance.currentBoss == BossController.Boss.Critter)
            {
                PlaySFX("Critter Final Phase Impact");
                RTPCManager.Instance.ResetAttributeValue("VOLUME", 0.00000000000001f, RTPCManager.CurveTypes.linear);
            }
        }
        else    //other phases
            PlaySFX("End Phase Impact");

        }
}


}