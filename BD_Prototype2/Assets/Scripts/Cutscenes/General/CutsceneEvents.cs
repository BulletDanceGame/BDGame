using UnityEngine;
using UnityEngine.InputSystem;


namespace BulletDance.Cutscene
{

    //Put general/common events to be put in cutscene signals here 
    public class CutsceneEvents : MonoBehaviour
    {
        // -- Private events -- //

        // -- Public events (for signals) -- //

        //EventManager events - Put ev initials to find in methods list faster
        public void EV_ActivateBoss()
        {
            EventManager.Instance.ActivateBoss();
        }


        //General Cutscene events - Putting cs initials to find in methods list faster

        //Input
        public void CS_InputToggle(bool enable)
        {
            if(EditorCheck.inEditMode) return;

            InputActionAsset playerInputActions = UnitManager.Instance.GetPlayer().GetComponent<PlayerInput>().actions;



            if(enable)
            {
                playerInputActions.FindActionMap("Player").Enable();
                playerInputActions.FindActionMap("Cutscene").Disable();
            }
            else
            {
                playerInputActions.FindActionMap("Cutscene").Enable();
                playerInputActions.FindActionMap("Player").Disable();
            }
        }


        public void CS_ShowScore()
        {
            var winScreen = FindObjectsOfType<WinScreen>(true);     
            if(winScreen.Length > 0)
                winScreen[0].gameObject.SetActive(true);
        }



        //Boss fight related
        public void CS_Fight(bool isInFight) //set
        {
            if(isInFight)
            {
                //RTPCManager.Instance.SetValue("VOLUME____MusicEnvRoaming", 0, 0.0000001f, RTPCManager.CurveTypes.high_curve); >>>>>> should stop, but should also have an echo left of it
                RTPCManager.Instance.SetValue("VOLUME____PlayerMovement__Footsteps", 0, 0.0000001f, RTPCManager.CurveTypes.high_curve);
                //RTPCManager.Instance.SetValue("VOLUME____AmbientComponents", 0, 0.0000001f, RTPCManager.CurveTypes.high_curve);
                RTPCManager.Instance.SetValue("VOLUME_SPECIAL____Dash__DynamicMixing", 100, 0.0000001f, RTPCManager.CurveTypes.high_curve);
            }

            else
            {
                RTPCManager.Instance.ResetValue("VOLUME____MusicEnvRoaming", 0.0000001f, RTPCManager.CurveTypes.high_curve);
                RTPCManager.Instance.ResetValue("VOLUME____PlayerMovement__Footsteps", 0.0000001f, RTPCManager.CurveTypes.high_curve);
                //RTPCManager.Instance.ResetValue("VOLUME____AmbientComponents", 0.0000001f, RTPCManager.CurveTypes.high_curve);
                RTPCManager.Instance.ResetValue("VOLUME_SPECIAL____Dash__DynamicMixing", 0.0000001f, RTPCManager.CurveTypes.high_curve);
            }
        }

        public void InFight()
        {
             AkSoundEngine.SetState("PlayerStatus", "Fighting");
        }

        public void OutOfFight()
        {
             AkSoundEngine.SetState("PlayerStatus", "Roaming");
        }

        public void LowGameplaySFX()
        {
            AkSoundEngine.SetState("DynamicGameplaySFX", "Low");
        }

        public void MidGameplaySFX()
        {
            AkSoundEngine.SetState("DynamicGameplaySFX", "Mid");
        }

        public void HighGameplaySFX()
        {
            AkSoundEngine.SetState("DynamicGameplaySFX", "High");
        }



    }

}