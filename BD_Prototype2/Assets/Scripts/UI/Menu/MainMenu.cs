using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    //Input
    private ControllerType _currentController;
    private PointerEventData _eventDataCurrentPosition;
    private Vector2 _mousePosition;
    private bool _noRaycast = true;
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();
    private GameObject _currentSelection;

    private Button _currentButton;
    public Button selectedMainButton;
    public Button selectedCalibrationButton;
    public Button selectedScoreButton;

    private bool _updateInput = true;

    //Screens
    public GameObject mainScreen;
    public GameObject calibrationScreen;
    public GameObject scoreScreen;



    // -- Init, Sub & unsub events, Update -- //
    private void Start()
    {
        _updateInput = true;

        _eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        _currentButton = selectedMainButton;

        _currentController = ControllerType.KEYBOARDANDMOUSE;
        _currentController = InputManager.Instance.CurrentController;

        if(_currentController == ControllerType.GAMEPAD)
            _currentButton.Select();

        //To switch controller-specific code
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnGamePadUsed          += ChangeToGAMEPAD;
        EventManager.Instance.OnKeyBoardAndMouseUsed += ChangeToKBM;
    }

    void OnDestroy()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnGamePadUsed          -= ChangeToGAMEPAD;
        EventManager.Instance.OnKeyBoardAndMouseUsed -= ChangeToKBM;
    }

    private void Update()
    {
        if(!_updateInput) return;

        switch(_currentController)
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
    

    // -- Input -- //
    void ChangeToGAMEPAD()
    {
        _currentController = ControllerType.GAMEPAD;

        //if(!_noRaycast)
        //    _currentButton = _currentSelection.transform.parent.GetComponent<Button>();
        //else
        //{
            if(mainScreen.activeSelf)
                _currentButton = selectedMainButton;
            else if(calibrationScreen.activeSelf)
                _currentButton = selectedCalibrationButton;
            else if(scoreScreen.activeSelf)
                _currentButton = selectedScoreButton;
        //}

        _currentButton.Select();

        Debug.Log("Switch to gamepad");
    }

    void ChangeToKBM()
    {
        _currentController = ControllerType.KEYBOARDANDMOUSE;

        _currentButton = _currentSelection.transform.parent.GetComponent<Button>();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void PointerHover_FirstRaycastedObject()
    {
        _eventDataCurrentPosition.position = _mousePosition;
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _raycastResults);
        if(_raycastResults.Count > 0)
            _currentSelection = _raycastResults[0].gameObject;

        _noRaycast = !(_raycastResults.Count > 0);
    }


    // -- Buttons -- //

    public void QuitGame()
    {
        Application.Quit();
    }

    //Game
    public void StartGame()
    {
        _updateInput = false;
        DisableButtons(FindObjectsOfType<Button>());

        MainMenuConductor.Instance.StopMenuMusic();

        GameManager.Instance.ChangeGameState(GameState.NEWGAME);

        //SceneLoad happens in the SceneLoadButton on the Button
    }

    //Calibration
    public void StartCalibrate()
    {
        mainScreen.SetActive(false);
        calibrationScreen.SetActive(true);

        if(_currentController == ControllerType.GAMEPAD)
        {
            _currentButton = selectedCalibrationButton;
            _currentButton.Select();
        }
    }

    public void BackFromCalibration()
    {
        mainScreen.SetActive(true);
        calibrationScreen.SetActive(false);

        if(_currentController == ControllerType.GAMEPAD)
        {
            _currentButton = selectedMainButton;
            _currentButton.Select();
        }
    }

    //Score
    public void StartLeaderboard()
    {
        mainScreen.SetActive(false);
        scoreScreen.SetActive(true);

        if(_currentController == ControllerType.GAMEPAD)
        {
            _currentButton = selectedScoreButton;
            _currentButton.Select();
        }
    }

    public void BackFromLeaderboard()
    {
        mainScreen.SetActive(true);
        scoreScreen.SetActive(false);

        if(_currentController == ControllerType.GAMEPAD)
        {
            _currentButton = selectedMainButton;
            _currentButton.Select();
        }
    }

    public void DisableButtons(Button[] buttons)
    {
        EventSystem.current.SetSelectedGameObject(null);

        foreach(var button in buttons)
        {
            button.enabled = false;
        }
    }
}