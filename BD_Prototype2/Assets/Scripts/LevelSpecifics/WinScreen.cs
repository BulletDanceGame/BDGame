using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class WinScreen : MonoBehaviour
{
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

        int minutes = Mathf.FloorToInt((GameManager.Instance.ElapsedTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(GameManager.Instance.ElapsedTime % 60f);

        TimeText.text = "" + minutes + " : " + seconds;

        int score = ScoreManager.Instance.SessionScore + ScoreManager.Instance.CurrentTimeScore;

        ScoreText.text = "" + score;

        PerfectHits.text = "" + ScoreManager.Instance.PerfectHits;
        GoodHits.text = "" + ScoreManager.Instance.GoodHits;
        BadHits.text = "" + ScoreManager.Instance.BadHits;

        GotHitText.text = "" + ScoreManager.Instance.GotHit;

        if (score >= 25800)
        {
            GradeText.text = "S";
        }else if(score > 20000)
        {
            GradeText.text = "A";
        }else if (score > 17200)
        {
            GradeText.text = "B";
        }
        else if (score >= 10000)
        {
            GradeText.text = "C";
        }
        else if (score >= 7500)
        {
            GradeText.text = "D";
        }
        else if (score >= 6500)
        {
            GradeText.text = "E";
        }
        else{
            GradeText.text = "F";
        }


    }

    void OnDestroy()
    {
        //_playerInput.Player.Move.performed   -= OnHold;
        //_playerInput.Player.Move.canceled    -= OnRelease;
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
        done_button.Select();

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
}