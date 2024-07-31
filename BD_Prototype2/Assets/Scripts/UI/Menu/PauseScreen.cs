using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{

    private bool _paused;
    [SerializeField] private CanvasGroup _ui;


    [SerializeField] AK.Wwise.Event _pauseEvent;
    [SerializeField] AK.Wwise.Event _resumeEvent;
    [SerializeField] AK.Wwise.Event _pauseMenuSong;


    [SerializeField] private GameObject main;
    [SerializeField] private GameObject options;
    [SerializeField] private Button mainSelectedButton;
    [SerializeField] private Button optionsSelectedButton;

    private void OnEnable()
    {
        EventManager.Instance.OnPausePressed += PausePressed;
        EventManager.Instance.OnGamePadUsed += ChangeToGAMEPAD;
        EventManager.Instance.OnKeyBoardAndMouseUsed += ChangeToKBM;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnPausePressed -= PausePressed;
        EventManager.Instance.OnGamePadUsed -= ChangeToGAMEPAD;
        EventManager.Instance.OnKeyBoardAndMouseUsed -= ChangeToKBM;
    }




    public void PausePressed()
    {
        if (!_paused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    private void Pause()
    {
        _paused = true;
        Time.timeScale = 0;
        _ui.alpha = 1;
        _ui.interactable = true;
        _ui.blocksRaycasts = true;

        if (currentController == ControllerType.GAMEPAD)
        {
            mainSelectedButton.Select();
        }

        _pauseEvent.Post(gameObject);
        _pauseMenuSong.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncExit, MusicCallbacks);

        EventManager.Instance.Pause(true);
    }

    public void Resume()
    {
        _paused = false;
        Time.timeScale = 1;
        _ui.alpha = 0;
        _ui.interactable = false;
        _ui.blocksRaycasts = false;


        main.SetActive(true);
        options.SetActive(false);

        _resumeEvent.Post(gameObject);
        _pauseMenuSong.Stop(gameObject);

        EventManager.Instance.Pause(false);

    }



    private void MusicCallbacks(object in_cookie, AkCallbackType in_type, object in_info)
    {
        
        if (in_type == AkCallbackType.AK_MusicSyncExit)
        {
            _pauseMenuSong.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncExit, MusicCallbacks);
        }


    }

    public void DisablePauseScreen()
    {
        EventManager.Instance.PausePressed();
    }

    public void Options()
    {
        main.SetActive(false);
        options.SetActive(true);

        optionsSelectedButton.Select();
    }

    public void BackFromOptions()
    {

        main.SetActive(true);
        options.SetActive(false);

        mainSelectedButton.Select();
    }





    // -- Input -- //
    public static ControllerType currentController;

    private PointerEventData _eventDataCurrentPosition;
    private Vector2 _mousePosition;
    private bool _noRaycast = true;
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();
    private GameObject _currentSelection;

    private void Start()
    {

        _eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        currentController = ControllerType.KEYBOARDANDMOUSE;
        currentController = InputManager.Instance.CurrentController;
    }

    private void Update()
    {
        switch (currentController)
        {
            case ControllerType.KEYBOARDANDMOUSE:
                _mousePosition = Input.mousePosition;
                PointerHover_FirstRaycastedObject();
                break;

            case ControllerType.GAMEPAD:
                _currentSelection = EventSystem.current.currentSelectedGameObject;
                break;

            default:
                Debug.Log("Unknown controller");
                break;
        }
    }

    void ChangeToGAMEPAD()
    {
        currentController = ControllerType.GAMEPAD;

        if (main.activeSelf)
        {
            mainSelectedButton.Select();
        }
        else if (options.activeSelf)
        {
            optionsSelectedButton.Select();
        }

        Debug.Log("Switch to gamepad");
    }

    void ChangeToKBM()
    {
        currentController = ControllerType.KEYBOARDANDMOUSE;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void PointerHover_FirstRaycastedObject()
    {
        _eventDataCurrentPosition.position = _mousePosition;
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _raycastResults);
        if (_raycastResults.Count > 0)
        {
            _currentSelection = _raycastResults[0].gameObject;
        }

        _noRaycast = !(_raycastResults.Count > 0);
    }


    public void DeselectButton()
    {
        if (currentController == ControllerType.KEYBOARDANDMOUSE)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
