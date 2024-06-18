
namespace BulletDance.Audio //Ignore indent of this {} bc that's annoying
{


/*
    This class contains Layer 1 (environment, enemies) SFX & rtpc controlls.
    There should be NO REFERENCE this class.

    This is Mo's living space.
*/
public class Layer1Sounds : SoundContainer
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