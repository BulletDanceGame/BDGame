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

    public void Done()
    {

        _screens[_currentScreen].SetActive(false);
        _buttons[_currentScreen].color = new Color(1f, 1f, 1f, 0.5f);
        _screens[0].SetActive(true);
        _buttons[0].color = Color.white;
        _currentScreen = 0;


        SaveSystem.Instance.Save();

        menu.BackFromCalibration();
    }

    public void SwitchScreen(int i)
    {
        if (_currentScreen + i < 0)
        {
            SaveSystem.Instance.Save();
            
            menu.BackFromCalibration();
        }
        else if (_currentScreen + i >= _screens.Length)
        {
            SaveSystem.Instance.Save();

            _screens[_currentScreen].SetActive(false);
            _screens[0].SetActive(true);
            _currentScreen = 0;
            menu.BackFromCalibration();
        }
        else
        {
            _screens[_currentScreen].SetActive(false);
            _currentScreen += i;
            _screens[_currentScreen].SetActive(true);
            
            if (MainMenu.currentController == ControllerType.GAMEPAD)
            {
                MainMenu.currentButton = _screens[_currentScreen].GetComponentInChildren<Button>();
                MainMenu.currentButton.Select();
            }
        }
    }
}
