using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Options : MonoBehaviour
{

    [SerializeField] private MainMenu menu;
    [SerializeField] private PauseScreen pause;


    [Header("FPS")]
    [SerializeField] private TextMeshProUGUI _fpsText;


    private void Start()
    {

        _fpsText.text = MusicManager.Instance.maxFPS.ToString();
    }



    public void BackToMenu()
    {

        SaveSystem.Instance.Save();
        menu.BackFromOptions();
    }


    public void BackToPauseMenu()
    {
        SaveSystem.Instance.Save();
        pause.BackFromOptions();
    }


    public void ChangeFPS(int dir)
    {
        int fps = MusicManager.Instance.maxFPS;
        fps += dir * 10;
        fps = Mathf.Clamp(fps, 30, 300);
        MusicManager.Instance.maxFPS = fps;

        SaveSystem.Instance.GetData().maxFPS = fps;

        _fpsText.text = fps.ToString();

    }

}
