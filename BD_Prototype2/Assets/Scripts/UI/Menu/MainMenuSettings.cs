using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class MainMenuSettings : MonoBehaviour
{

    [SerializeField] private bool _playTrailer;
    bool trailerPlaying = false;

    float seconds = 0;
    public float MaxIdleTimeBeforeTrailer = 10;

    public GameObject videoPlayer;
    public GameObject videoScreen;

    // Update is called once per frame
    void Update()
    {
        if (_playTrailer == false)
            return;

        bool movement;

        if (InputManager.Instance.CurrentController == ControllerType.KEYBOARDANDMOUSE)
        {
            movement = MouseCursor.instance.IsMouseMoving() || Mouse.current.leftButton.IsPressed() ||
                        Keyboard.current.anyKey.IsPressed();
        }
        else
        {
            movement = Gamepad.current.buttonEast.IsPressed() || Gamepad.current.buttonWest.IsPressed() ||
                       Gamepad.current.buttonNorth.IsPressed() || Gamepad.current.buttonSouth.IsPressed() ||
                       Gamepad.current.leftShoulder.IsPressed() || Gamepad.current.rightShoulder.IsPressed() ||
                       Gamepad.current.leftStick.ReadValue() != Vector2.zero || Gamepad.current.rightStick.ReadValue() != Vector2.zero;
        }

        if (!movement)
        {
            seconds += Time.deltaTime;
        }
        else
        {
            if (trailerPlaying)
            {
                trailerPlaying = false;
                MainMenuConductor.Instance.SwitchToMenuMusic();
            }

            seconds = 0;
        }

        if (seconds >= MaxIdleTimeBeforeTrailer)
        {
            if (!trailerPlaying)
            {
                trailerPlaying = true;
                MainMenuConductor.Instance.SwitchToTrailerMusic();
            }
        }

        if (seconds >= MaxIdleTimeBeforeTrailer + videoPlayer.GetComponent<VideoPlayer>().clip.length-1)
        {
            trailerPlaying = false;
            MainMenuConductor.Instance.SwitchToMenuMusic();
            seconds = 0;
        }

    }



    public void PlayTrailer()
    {
        videoScreen.SetActive(true);
        videoPlayer.GetComponent<VideoPlayer>().Play();
    }

    public void StopTrailer()
    {
        videoScreen.SetActive(false);
        videoPlayer.GetComponent<VideoPlayer>().Stop();
    }
}
