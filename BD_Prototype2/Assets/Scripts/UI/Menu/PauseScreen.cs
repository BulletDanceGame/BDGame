using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseScreen : MonoBehaviour
{

    private bool _paused;
    [SerializeField] private CanvasGroup _ui;


    [SerializeField] AK.Wwise.Event _pauseEvent;
    [SerializeField] AK.Wwise.Event _resumeEvent;
    [SerializeField] AK.Wwise.Event _pauseMenuSong;


    [SerializeField] private GameObject main;
    [SerializeField] private GameObject options;

    private void OnEnable()
    {
        EventManager.Instance.OnPausePressed += PausePressed;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnPausePressed -= PausePressed;
    }


    public void PausePressed()
    {
        if (!_paused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    private void Pause()
    {
        _paused = true;
        Time.timeScale = 0;
        _ui.alpha = 1;
        _ui.interactable = true;
        _ui.blocksRaycasts = true;
        GetComponentInChildren<UnityEngine.UI.Button>().Select();

        _pauseEvent.Post(gameObject);
        _pauseMenuSong.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncExit, MusicCallbacks);

        EventManager.Instance.Pause(true);
    }

    public void Resume()
    {
        _paused = false;
        Time.timeScale = 1;
        _ui.alpha = 0;
        _ui.interactable = false;
        _ui.blocksRaycasts = false;

        _resumeEvent.Post(gameObject);
        _pauseMenuSong.Stop(gameObject);

        EventManager.Instance.Pause(false);
    }



    private void MusicCallbacks(object in_cookie, AkCallbackType in_type, object in_info)
    {
        
        if (in_type == AkCallbackType.AK_MusicSyncExit)
        {
            _pauseMenuSong.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncExit, MusicCallbacks);
        }


    }

    public void DisablePauseScreen()
    {
        EventManager.Instance.PausePressed();
    }

    public void Options()
    {
        main.SetActive(false);
        options.SetActive(true);
    }

    public void BackFromOptions()
    {

        main.SetActive(true);
        options.SetActive(false);
    }

    public void Menu()
    {
        Resume();
    }




}
