using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;


namespace BulletDance.Cutscene
{


    // Controlls how cutscenes are operated 
    public class CutsceneController : MonoBehaviour
    {
        private PlayableDirector[] _cutscenes = null;
        private PlayableDirector   _currentDirector;

        private bool _cutscenePlaying = false;
        private bool _musicStarted    = false;
        private bool _paused = false;

        private void Start()
        {
            _cutscenes = GetComponentsInChildren<PlayableDirector>(true);
            foreach(var cutscene in _cutscenes)
            {
                cutscene.enabled = false;
            }

            EventManager.Instance.OnCutsceneStart += Play;
            EventManager.Instance.OnEnableSpeedUp += EnableSpeedUp;
            EventManager.Instance.OnPause += Pause;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnCutsceneStart -= Play;
            EventManager.Instance.OnEnableSpeedUp -= EnableSpeedUp;
            EventManager.Instance.OnPause -= Pause;
        }


        // -- Start & End cutscene -- //

        public void Play(string cutsceneName)
        {
            foreach(var cutscene in _cutscenes)
            {
                if (cutscene.transform.name == cutsceneName) 
                {
                    _currentDirector = cutscene;
                    _currentDirector.enabled = true;
                    _currentDirector.Play();
                    _cutscenePlaying = true;
                    EventManager.Instance.StopTimer();

                    return;
                }
            }

            print("No child of CutsceneController had the name " + cutsceneName);
        }


        public void StartMusicForCutscene()
        {
            if(_musicStarted) return;
            BossConductor.Instance.StartCutsceneSequence();
            _musicStarted = true;
        }

        public void CutsceneEnded()
        {
            DisableSpeedUp();

            _cutscenePlaying = false;
            _musicStarted    = false;
            EventManager.Instance.StartTimer();

            EventManager.Instance.EndCutscene();
        }


        // -- Speed up -- //

        public void EnableSpeedUp()
        {
            InputManager.Instance.PlayerInput.Cutscene.SpeedUp.performed += SpeedUp;
            InputManager.Instance.PlayerInput.Cutscene.SpeedUp.canceled  += StopSpeedUp;
        }

        public void DisableSpeedUp()
        {
            StopSpeedUp(1);

            InputManager.Instance.PlayerInput.Cutscene.SpeedUp.performed -= SpeedUp;
            InputManager.Instance.PlayerInput.Cutscene.SpeedUp.canceled  -= StopSpeedUp;
        }


        public void Pause(bool pause)
        {
            if (!_cutscenePlaying) return;

            if (pause)
            {
                StopSpeedUp(0);
            }
            else
            {
                _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            }
            _paused = pause;

        }



        public void SpeedUp(InputAction.CallbackContext c)
        {
            if(!_cutscenePlaying || _paused) return;

            MusicManager.Instance.speedingUpInCutscene = true;
            _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(4);
            Time.timeScale = 4;

            //RTPC Effects
            //EventManager.Instance.PlaySFX("Speed Up Cutscene"); //temp

            RTPCManager.Instance.SetValue("PLAYBACK_SPEED____CutsceneMusic", 100, 0.0000000001f, 0);
            RTPCManager.Instance.SetValue("HIGH_PASS____CutsceneMusic", 54, 0.0000000001f, 0);
            RTPCManager.Instance.SetValue("VOLUME____CutsceneMusic", 15, 0.0000000001f, 0);

            RTPCManager.Instance.SetValue("PITCH____CutsceneSFX__Voicelines", 71, 0.0000000001f, 0);
            RTPCManager.Instance.SetValue("HIGH_PASS____CutsceneSFX__Voicelines", 54, 0.0000000001f, 0);
            RTPCManager.Instance.SetValue("VOLUME____CutsceneSFX__Voicelines", 10, 0.0000000001f, 0);

            RTPCManager.Instance.SetValue("PITCH____CutsceneSFX__General__AudibleOnSpeedUp", 70, 0.0000000001f, 0);
            RTPCManager.Instance.SetValue("HIGH_PASS____CutsceneSFX__General__AudibleOnSpeedUp", 54, 0.0000000001f, 0);
            RTPCManager.Instance.SetValue("VOLUME____CutsceneSFX__General__InaudibleOnSpeedUp", 0, 0.0000000001f, 0);
            RTPCManager.Instance.SetValue("VOLUME____CutsceneSFX__SpeedUpCutscene", 70, 0.0000000001f, 0);
        }


        public void StopSpeedUp(InputAction.CallbackContext c)
        {
            StopSpeedUp(1);
        }

        public void StopSpeedUp(float t)
        {
            if (!_cutscenePlaying || _paused) return;

            MusicManager.Instance.speedingUpInCutscene = false;
            _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(t);
            Time.timeScale = t;

            //RTPC Effects
            RTPCManager.Instance.ResetValue("PLAYBACK_SPEED____CutsceneMusic", 0.0000000001f, 0);
            RTPCManager.Instance.ResetValue("HIGH_PASS____CutsceneMusic", 0.0000000001f, 0);
            RTPCManager.Instance.ResetValue("VOLUME____CutsceneMusic", 0.0000000001f, 0);

            RTPCManager.Instance.ResetValue("PITCH____CutsceneSFX__Voicelines", 0.0000000001f, 0);
            RTPCManager.Instance.ResetValue("HIGH_PASS____CutsceneSFX__Voicelines", 0.0000000001f, 0);
            RTPCManager.Instance.ResetValue("VOLUME____CutsceneSFX__Voicelines", 0.0000000001f, 0);

            RTPCManager.Instance.ResetValue("VOLUME____CutsceneSFX__General__InaudibleOnSpeedUp", 0.0000000001f, 0);
            RTPCManager.Instance.ResetValue("VOLUME____CutsceneSFX__SpeedUpCutscene", 0.0000000001f, 0);
        }

    }


}