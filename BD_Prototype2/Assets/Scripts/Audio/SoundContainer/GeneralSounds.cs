
namespace BulletDance.Audio //Ignore indent of this {} bc that's annoying
{


/*
    This class contains all general SFX & rtpc controlls.
        (excluding cutscene-specific controls, that should be separated bc there could be many of them)
        (excluding level-specific controls, because they'd be loaded for every level if put in here)
    There should be NO REFERENCE this class.

    This is Mo's living space.
*/
public class GeneralSounds : SoundContainer
{
    private uint ambienceSoundID;
    private uint fireSoundID;
    AkGameObj gameObj;
        
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

    public void MenuNavigation()
    {
        PlaySFX("MenuNav");
    }

    public void MenuConfirmation()
    {
        PlaySFX("MenuConfirm");
    }

    public void WinMenuConfirmation()
    {
        PlaySFX("WinScreenConfirm");
    }

    public void PauseMenuConfirmation()
    {
        PlaySFX("PauseScreeConfirm");
    }

    public void StartGame()
    {
        RTPCManager.Instance.SetValue("VOLUME____MenuMusic", 0, 0.0000000001f, RTPCManager.CurveTypes.high_curve, 0.00000001f, RTPCManager.CurveTypes.high_curve, 4);
        PlaySFX("StartGame");
    }

    public void SetAmbienceVolume(int volume)
    {
         RTPCManager.Instance.SetValue("VOLUME____AmbientComponents", volume, 2, RTPCManager.CurveTypes.high_curve);
    }

    public void LongerSetAmbienceVolume(int volume)
    {
         RTPCManager.Instance.SetValue("VOLUME____AmbientComponents", volume, 6, RTPCManager.CurveTypes.high_curve);
    }

    public void ResetAmbienceVolume(float sec)
    {
         RTPCManager.Instance.ResetValue("VOLUME____AmbientComponents", sec, RTPCManager.CurveTypes.high_curve);
    }

    public void PlayForestAmbience()
    {
            ambienceSoundID = AkSoundEngine.PostEvent("Play_ForestAmb", gameObject);
    }

    public void StopForestAmbience()
    {
           ambienceSoundID = AkSoundEngine.PostEvent("Stop_ForestAmb", gameObject);
    }

    public void PlayFireAmb()
    {
            fireSoundID = AkSoundEngine.PostEvent("Play_fire", gameObject);
    }

    public void StopFireAmb()
    {
            fireSoundID = AkSoundEngine.PostEvent("Stop_fire", gameObject);
    }

    public void MuteEnvMusic()
    {
           RTPCManager.Instance.SetValue("VOLUME____MusicEnvBattle", 0, 0.0000001f, RTPCManager.CurveTypes.high_curve);
    }

    public void UnmuteEnvMusic()
    {
           RTPCManager.Instance.ResetValue("VOLUME____MusicEnvBattle", 0.0000001f, RTPCManager.CurveTypes.high_curve);
    }


}

}