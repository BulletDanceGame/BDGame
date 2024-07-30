using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    //Input
    public static ControllerType currentController;
   

    private PointerEventData _eventDataCurrentPosition;
    private Vector2 _mousePosition;
    private bool _noRaycast = true;
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();
    private GameObject _currentSelection;

    public static Button currentButton;
    public Button selectedMainButton;
    public Button selectedCalibrationButton;
    public Button selectedOptionsButton;
    public Button selectedScoreButton;

    private bool _updateInput = true;

    //Screens
    public GameObject mainScreen;
    public GameObject calibrationScreen;
    public GameObject optionsScreen;
    public GameObject scoreScreen;


    private void OnEnable()
    {
        EventManager.Instance.OnEnableInput += Activate;
        EventManager.Instance.OnDisableInput += Deactivate;
    }


    private void OnDisable()
    {
        EventManager.Instance.OnEnableInput -= Activate;
        EventManager.Instance.OnDisableInput -= Deactivate;
    }


    // -- Init, Sub & unsub events, Update -- //
    private void Start()
    {
        _updateInput = true;

        _eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        currentButton = selectedMainButton;

        currentController = ControllerType.KEYBOARDANDMOUSE;
        currentController = InputManager.Instance.CurrentController;

        if(currentController == ControllerType.GAMEPAD)
            currentButton.Select();

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

        switch(currentController)
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


    void Activate()
    {
        mainScreen.SetActive(true);
        print("click?");
    }

    void Deactivate()
    {
        mainScreen.SetActive(false);
    }


    // -- Input -- //
    void ChangeToGAMEPAD()
    {
        currentController = ControllerType.GAMEPAD;

        //if(!_noRaycast)
        //    _currentButton = _currentSelection.transform.parent.GetComponent<Button>();
        //else
        //{
            if(mainScreen.activeSelf)
                currentButton = selectedMainButton;
            else if(calibrationScreen.activeSelf)
                currentButton = selectedCalibrationButton;
            else if(scoreScreen.activeSelf)
                currentButton = selectedScoreButton;
        //}

        currentButton.Select();

        Debug.Log("Switch to gamepad");
    }

    void ChangeToKBM()
    {
        currentController = ControllerType.KEYBOARDANDMOUSE;

        currentButton = null;// _currentSelection.transform.parent.GetComponent<Button>();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void PointerHover_FirstRaycastedObject()
    {
        _eventDataCurrentPosition.position = _mousePosition;
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _raycastResults);
        if(_raycastResults.Count > 0)
        {
            _currentSelection = _raycastResults[0].gameObject;
        }

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
        SaveSystem.Instance.GetData().currentCheckpointX = -1;
        SaveSystem.Instance.GetData().currentCheckpointY = -1;
        SaveSystem.Instance.GetData().misscutsceneplayed=false;
        SaveSystem.Instance.GetData().bossdeath = false;
        SaveSystem.Instance.GetData().hasplayed1stcutscene= false;
        SaveSystem.Instance.GetData().hasplayed2stcutscene= false;
        SaveSystem.Instance.GetData().hasBat= false;

        SaveSystem.Instance.Save();
        //SceneLoad happens in the SceneLoadButton on the Button
    }
    public void ContinueGame()
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

        if(currentController == ControllerType.GAMEPAD)
        {
            currentButton = selectedCalibrationButton;
            currentButton.Select();
        }
    }

    public void BackFromCalibration()
    {
        mainScreen.SetActive(true);
        calibrationScreen.SetActive(false);

        if (currentController == ControllerType.GAMEPAD)
        {
            currentButton = selectedMainButton;
            currentButton.Select();
        }
    }


    //Options
    public void StartOptions()
    {
        mainScreen.SetActive(false);
        optionsScreen.SetActive(true);

        if (currentController == ControllerType.GAMEPAD)
        {
            currentButton = selectedOptionsButton;
            currentButton.Select();
        }
    }
    public void BackFromOptions()
    {
        mainScreen.SetActive(true);
        optionsScreen.SetActive(false);

        if (currentController == ControllerType.GAMEPAD)
        {
            currentButton = selectedMainButton;
            currentButton.Select();
        }
    }

    //Score
    public void StartLeaderboard()
    {
        mainScreen.SetActive(false);
        scoreScreen.SetActive(true);

        if(currentController == ControllerType.GAMEPAD)
        {
            currentButton = selectedScoreButton;
            currentButton.Select();
        }
    }

    public void BackFromLeaderboard()
    {
        mainScreen.SetActive(true);
        scoreScreen.SetActive(false);

        if(currentController == ControllerType.GAMEPAD)
        {
            currentButton = selectedMainButton;
            currentButton.Select();
        }
    }



    //Deselect Buttons

    public void DisableButtons(Button[] buttons)
    {
        EventSystem.current.SetSelectedGameObject(null);

        foreach(var button in buttons)
        {
            button.enabled = false;
        }
    }

    public void DeselectButton()
    {
        if (currentController == ControllerType.KEYBOARDANDMOUSE)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}