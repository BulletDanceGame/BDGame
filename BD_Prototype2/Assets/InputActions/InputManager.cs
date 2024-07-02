using UnityEngine;
using UnityEngine.InputSystem;

public enum ControllerType { GAMEPAD, KEYBOARDANDMOUSE };

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }


    public ControllerType CurrentController;

    public PlayerInputActions PlayerInput;
    public PlayerInput input;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        PlayerInput = new PlayerInputActions();
        PlayerInput.Player.Enable();
        PlayerInput.Cutscene.Enable();
    }

    

    private void Start()
    {

        //MusicManager.Instance.GetInput();

        CurrentController = ControllerType.KEYBOARDANDMOUSE;
    }

    private void Update()
    {
        if (!input)
        {
            if (!UnitManager.Instance.GetPlayer())
                return;

            input = UnitManager.Instance.GetPlayer().GetComponent<PlayerInput>();
        }

        if (input.currentControlScheme == "KeyBoard" && CurrentController == ControllerType.GAMEPAD)
        {
            CurrentController = ControllerType.KEYBOARDANDMOUSE;
            EventManager.Instance.KeyboardAndMouseUsed();
        }

        if(input.currentControlScheme == "GamePad" && CurrentController == ControllerType.KEYBOARDANDMOUSE)
        {
            CurrentController = ControllerType.GAMEPAD;
            EventManager.Instance.GamepadUsed();
        }
    }
}
