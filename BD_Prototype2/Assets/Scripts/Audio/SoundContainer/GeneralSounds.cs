
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

}

}