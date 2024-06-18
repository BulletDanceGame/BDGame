using UnityEngine;

public class CalibrationMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] _screens;
    private int _currentScreen;

    [SerializeField] private MainMenu menu;

    public void SwitchScreen(int i)
    {
        if (_currentScreen + i < 0)
        {
            menu.BackFromCalibration();
        }
        else if (_currentScreen + i >= _screens.Length)
        {
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
        }
    }
}
