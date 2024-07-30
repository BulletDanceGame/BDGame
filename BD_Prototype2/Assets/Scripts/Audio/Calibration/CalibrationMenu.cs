using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] _screens;
    [SerializeField] private TextMeshProUGUI[] _titleButtons;


    private int _currentScreen;

    [SerializeField] private MainMenu menu;

    public bool canHit { get; set; } = true;




    //also called from button
    public void ActivateScreen(int i)
    {
        _screens[_currentScreen].SetActive(false);
        _titleButtons[_currentScreen].color = new Color(1f,1f,1f,0.5f);
        if(_currentScreen == 1)
        {
            RTPCManager.Instance.SetAttributeValue("VOLUME", 50, 1f, RTPCManager.CurveTypes.linear);
        }

        _currentScreen = i;
        _screens[_currentScreen].SetActive(true);
        _titleButtons[_currentScreen].color = Color.white;


        if (MainMenu.currentController == ControllerType.GAMEPAD)
        {
            Button[] buttons = _screens[_currentScreen].GetComponentsInChildren<Button>();

            if (buttons.Length == 0)
            {
                MainMenu.currentButton = _titleButtons[_currentScreen].GetComponentInParent<Button>();
            }
            else
            {
                MainMenu.currentButton = buttons[0];
            }

            print("button " + MainMenu.currentButton.name);
            MainMenu.currentButton.Select();
        }
    }

    //also called from button
    public void Done()
    {

        ActivateScreen(0);


        SaveSystem.Instance.GetData().haveCalibrated = true;
        SaveSystem.Instance.Save();

        menu.BackFromCalibration();
    }

    //also called from button
    public void SwitchScreen(int i)
    {
        if (_currentScreen + i < 0)
        {
            //currently cant happen
            //SaveSystem.Instance.Save();
            
            //menu.BackFromCalibration();
        }
        else if (_currentScreen + i >= _screens.Length)
        {
            Done();
        }
        else
        {
            ActivateScreen(_currentScreen+i);

        }
    }


    public void ActivateTitleButton()
    {
        MainMenu.currentButton = _titleButtons[_currentScreen].GetComponentInParent<Button>();
;
        MainMenu.currentButton.Select();
    }
}
