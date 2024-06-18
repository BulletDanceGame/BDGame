
namespace BulletDance.Audio //Ignore indent of this {} bc that's annoying
{


/*
    This class contains general cutscene SFX & rtpc controlls.
        (excluding level-specific cutscene controls, because they'd be loaded for every level if put in here)
    There should be NO REFERENCE this class.

    This is Mo's living space.
*/
[UnityEngine.ExecuteAlways]
public class CutsceneSounds : SoundContainer
{
    // -- Event Hooks & sound initialization -- //
    void Start()
    {
        //Init
        Initialize(); //So we can get SoundManager.Instance to access PlaySFX & other common helper methods

        //Events
    }

    void OnDestroy()
    {
        DeInitialize();
    }


    // -- Update -- //


    // -- SFX Implementation -- //
    public void PlaySpeedUpSFX()
    {
        PlaySFX("Speed Up Cutscene");
    }

    public void DialogueLetter(string talker)
    {
        switch(talker)
        {
            case "YokaiHunter":
                PlaySFX("Yokai Hunter"); break;
            case "Player":
                PlaySFX("Player"); break;
            default: break;
        }
    }

    //This is for dialogue UI's in-editor sound playback
    public AK.Wwise.Event GetDialogueLetterSFX(string talker)
    {
        switch(talker)
        {
            case "YokaiHunter":
                return GetSFX("Yokai Hunter");
            case "Player":
                return GetSFX("Player");
            default: return null;
        }
    }


}

}