using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] _screens;
    [SerializeField] private TextMeshProUGUI[] _buttons;
    private int _currentScreen;

    [SerializeField] private MainMenu menu;

    public bool canHit { get; set; } = true;




    //also called from button
    public void ActivateScreen(int i)
    {
        _screens[_currentScreen].SetActive(false);
        _buttons[_currentScreen].color = new Color(1f,1f,1f,0.5f);
        _currentScreen = i;
        _screens[_currentScreen].SetActive(true);
        _buttons[_currentScreen].color = Color.white;

        //if (MainMenu.currentController == ControllerType.GAMEPAD)
        //{
        //    MainMenu.currentButton = _screens[_currentScreen].GetComponentInChildren<Button>();
        //    MainMenu.currentButton.Select();
        //}
    }

    //also called from button
    public void Done()
    {

        ActivateScreen(0);

        SaveSystem.Instance.Save();

        menu.BackFromCalibration();
    }

    //also called from button
    public void SwitchScreen(int i)
    {
        if (_currentScreen + i < 0)
        {
            SaveSystem.Instance.Save();
            
            menu.BackFromCalibration();
        }
        else if (_currentScreen + i >= _screens.Length)
        {
            Done();
        }
        else
        {
            ActivateScreen(_currentScreen+i);

            if (MainMenu.currentController == ControllerType.GAMEPAD)
            {
                MainMenu.currentButton = _screens[_currentScreen].GetComponentInChildren<Button>();
                MainMenu.currentButton.Select();
            }
        }
    }
}
