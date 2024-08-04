using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class WinScreen : MonoBehaviour
{

    public static ControllerType currentController;
    private PointerEventData _eventDataCurrentPosition;

    [SerializeField] TextMeshProUGUI TimeText;
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TMP_InputField InputField;
    [SerializeField] TextMeshProUGUI GradeText;
    [SerializeField] TextMeshProUGUI PerfectHits;
    [SerializeField] TextMeshProUGUI GoodHits;
    [SerializeField] TextMeshProUGUI BadHits;
    [SerializeField] TextMeshProUGUI GotHitText;
    [SerializeField] Button done_button;
    [SerializeField] Transform[] arrows;
    [SerializeField] TextMeshProUGUI[] inputTexts;
    [SerializeField] Animator arrowAnim;
    PlayerInputActions _playerInput;
    [SerializeField] float inputThreshold = 0.3f;
    [SerializeField] float scrollCooldown = 1f;
    [SerializeField] float scrollSpdMin = 1f, scrollSpdMax = 5f, scrollSpdIncrease = 0.5f;

    public static Button currentButton;
    public Button selectedMainMenu;
    public Button selectedContinue;
    private GameObject _currentSelection;


    float scrollSpd = 1f;
    float scrollTime = 0f;
    bool isHolding = false;
    Vector2 scrollVector = new Vector2();

    float[] inputTextPositions = new float[3];
    Vector3 arrowPosition = new Vector3();
    string posibleText = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
    char[] inputOption = new char[27];

    int currentInputText = 0;
    int currentInputOption = 0;

    [Header("Other Menus Whos Buttons Fuck With These Ones When Active :)")]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;


    // Start is called before the first frame update
    void Start()
    {

        /*        _playerInput = InputManager.Instance.PlayerInput;

                for(int i = 0; i < inputTexts.Length; i++)
                {
                    inputTextPositions[i] = inputTexts[i].transform.position.x;
                }

                SetArrowsAtCurrentInputText();

                inputOption = posibleText.ToCharArray();

                //Immediate response when starting to scroll inputs
                scrollTime = scrollCooldown;
                scrollSpd  = scrollSpdMin;


                _playerInput.Player.Move.performed   += OnHold;
                _playerInput.Player.Move.canceled    += OnRelease;
        */




    }

    void OnEnable()
    {
        Debug.Log("DISABLING STUFF");

        GameObject.Find("Player").GetComponent<PlayerMovement>().canMove = false;
        //GameObject.Find("Player").GetComponent<PlayerMovement>().canDash = false;
        GameObject.Find("DirectionController").GetComponent<PlayerSwing>().SwingActivated = false;

        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);

        UnitManager.Instance.GetPlayer().GetComponent<Player>().pauseActions = true;



        //DOING EVERYTHING

        _eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        currentButton = selectedContinue;

        currentController = ControllerType.KEYBOARDANDMOUSE;
        currentController = InputManager.Instance.CurrentController;

        if (currentController == ControllerType.GAMEPAD)
            currentButton.Select();

        //To switch controller-specific code
        if (EventManager.Instance == null) return;
        EventManager.Instance.OnGamePadUsed += ChangeToGAMEPAD;
        EventManager.Instance.OnKeyBoardAndMouseUsed += ChangeToKBM;

        int minutes = (int)((GameManager.Instance.ElapsedTime % 3600f) / 60f);
        int seconds = (int)(GameManager.Instance.ElapsedTime % 60f);

        string zero = ((seconds < 10) ? "0" : "");
        TimeText.text = "" + minutes + " : " + zero + seconds;

        int score = ScoreManager.Instance.SessionScore + ScoreManager.Instance.CurrentTimeScore;

        ScoreText.text = "" + score;

        PerfectHits.text = "" + ScoreManager.Instance.PerfectHits;
        GoodHits.text = "" + ScoreManager.Instance.GoodHits;
        BadHits.text = "" + ScoreManager.Instance.BadHits;

        GotHitText.text = "" + ScoreManager.Instance.GotHit;

        if (score >= 100000)
        {
            GradeText.text = "S";
        }
        else if (score > 80000)
        {
            GradeText.text = "A";
        }
        else if (score > 60000)
        {
            GradeText.text = "B";
        }
        else if (score >= 50000)
        {
            GradeText.text = "C";
        }
        else if (score >= 40000)
        {
            GradeText.text = "D";
        }
        else if (score >= 20000)
        {
            GradeText.text = "E";
        }
        else
        {
            GradeText.text = "F";
        }

    }

    private void OnDisable()
    {
        UnitManager.Instance.GetPlayer().GetComponent<Player>().pauseActions = false;

        pauseMenu.SetActive(true);
        gameOverMenu.SetActive(true);
    }

    void OnDestroy()
    {
        //_playerInput.Player.Move.performed   -= OnHold;
        //_playerInput.Player.Move.canceled    -= OnRelease;

        if (EventManager.Instance == null) return;
        EventManager.Instance.OnGamePadUsed -= ChangeToGAMEPAD;
        EventManager.Instance.OnKeyBoardAndMouseUsed -= ChangeToKBM;
    }


    void OnHold(InputAction.CallbackContext context)
    {
        Vector2 input = _playerInput.Player.Move.ReadValue<Vector2>();

        if(input == Vector2.zero)
        {
            //Immediate response when starting to scroll inputs
            scrollTime = scrollCooldown;
            scrollSpd = scrollSpdMin;
            isHolding = false;
            return;
        }

        //Prioritize selecting input texts, inputThreshold determines flick/hold
        if(Mathf.Abs(input.x) > inputThreshold)
        {
            if(input.x < 0) scrollVector = Vector2.left;
            if(input.x > 0) scrollVector = Vector2.right;

            isHolding = true;
        }

        else if(Mathf.Abs(input.y) > inputThreshold)
        {
            if(input.y < 0) scrollVector = Vector2.down;
            if(input.y > 0) scrollVector = Vector2.up;

            isHolding = true;
        }

        else
        {
            //Immediate response when starting to scroll inputs
            scrollTime = scrollCooldown;
            scrollSpd = scrollSpdMin;
            isHolding = false;
        }
    }

    void OnRelease(InputAction.CallbackContext context)
    {
        isHolding = false;

        //Immediate response when starting to scroll inputs
        scrollTime = scrollCooldown;
        scrollSpd  = scrollSpdMin;
    }

    void Update()
    {
        DeselectButton();
/*
        if(!isHolding) return;

        if(scrollTime >= scrollCooldown)
        {
            InputSwitchState();
            scrollTime = 0f;
        }

        scrollSpd = Mathf.Clamp(scrollSpd + scrollSpdIncrease, scrollSpdMin, scrollSpdMax);
        scrollTime += Time.deltaTime * scrollSpd;
*/
    }

    void ChangeToGAMEPAD()
    {
        currentController = ControllerType.GAMEPAD;

        //if(!_noRaycast)
        //    _currentButton = _currentSelection.transform.parent.GetComponent<Button>();
        //else
        //{
        
            currentButton = selectedContinue;        

        currentButton.Select();

        Debug.Log("Switch to gamepad");
    }

    void ChangeToKBM()
    {
        currentController = ControllerType.KEYBOARDANDMOUSE;

        //currentButton = _currentSelection.transform.parent.GetComponent<Button>();
        EventSystem.current.SetSelectedGameObject(null);

        print("MKBBB");
    }

    void InputSwitchState()
    {
        if(scrollVector == Vector2.left || scrollVector == Vector2.right)
        {
            currentInputText = Mathf.Clamp(currentInputText + Mathf.RoundToInt(scrollVector.x), 0, inputTexts.Length - 1);

            //Get currently displayed character
            //Get current char, if not valid, return -1
            var currentChar = inputTexts[currentInputText].text.ToCharArray()[0];
            var foundIndex = Array.FindIndex(inputOption, chara => chara == currentChar);

            //Update to correct option index, set to 0 when invalid
            currentInputOption = foundIndex < 0 ? 0 : foundIndex;

            SetArrowsAtCurrentInputText();
        }

        else if(scrollVector == Vector2.up || scrollVector == Vector2.down)
        {
            currentInputOption = currentInputOption + Mathf.RoundToInt(scrollVector.y);

            //Wrap currentInputOption
            if(currentInputOption > inputOption.Length - 1)
                currentInputOption = 0;
            if(currentInputOption < 0)
                currentInputOption = inputOption.Length - 1;

            //Display changed text
            inputTexts[currentInputText].text = inputOption[currentInputOption].ToString();
        }


        //Play arrow animation
        if(scrollVector == Vector2.left)  arrowAnim.Play("Left");
        if(scrollVector == Vector2.right) arrowAnim.Play("Right");
        if(scrollVector == Vector2.up)    arrowAnim.Play("Up");
        if(scrollVector == Vector2.down)  arrowAnim.Play("Down");
    }


    void SetArrowsAtCurrentInputText()
    {
        for(int i = 0; i<arrows.Length; i++)
        {
            arrowPosition = arrows[i].position;
            arrowPosition.x = inputTextPositions[currentInputText];
            arrows[i].position = arrowPosition;
        }
    }



    string CreateName()
    {
        string name = "";
        for(int i = 0; i < inputTexts.Length; i++)
        {
            name += inputTexts[i].text;
        }
        print(name);
        return name;
    }


    public void EnterScore()
    {
/*
        //I swear if this is the reason why it didn't work last time
        //YES IT IS OMFG
        string name = CreateName();

        EventManager.Instance.AddHighScore(ScoreManager.Instance.SessionScore + ScoreManager.Instance.CurrentTimeScore, name);
*/

        MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.FADE_STOP);
    }


    public void HideScreen()
    {
        gameObject.SetActive(false);
    }

    public void DeselectButton()
    {
        if (currentController == ControllerType.KEYBOARDANDMOUSE)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
