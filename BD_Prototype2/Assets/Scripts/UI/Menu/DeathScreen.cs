using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeathScreen : MonoBehaviour
{

    public static ControllerType currentController;

    [System.Serializable]
    class ColorSet
    {
        public Color gameOverColor, textColor, normalColor, highlightColor, pressColor;
    }

    [System.Serializable]
    class ImgInfo
    {
        public Sprite image;
        public Vector2 imageSize, pivot;
    }

    [System.Serializable]
    class DeathScreenInfo
    {
        public List<ImgInfo> images;
        public List<ColorSet> phaseColor;
    }


    private CanvasGroup _canvas;

    [SerializeField]
    private List<DeathScreenInfo> _bosses;

    [SerializeField]
    private RectTransform _imageTrf;
    [SerializeField]
    private Image _image;

    [SerializeField]
    private Button _menuButton;
    [SerializeField]
    private TMPro.TextMeshProUGUI _restartTxt, _gameOverTxt;

    private int _currentBoss  = 0;
    private int _currentPhase = 1;


    void Start()
    {
        _canvas = GetComponent<CanvasGroup>();

        if(EventManager.Instance == null) return;
        EventManager.Instance.OnPlayerDeath += ShowScreen;        
    }

    void OnDestroy()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnPlayerDeath -= ShowScreen;
    }


    void ShowScreen()
    {
        BossController bossInfo = BossController.Instance;
        if(bossInfo != null)
        {
            _currentBoss  = (int)bossInfo.currentBoss;
            _currentPhase = bossInfo.currentBossPhase;
        }
        else
        {
            _currentBoss  = 0;
            _currentPhase = 1;
        }

        //The boss' image is not in the list
        if(_currentBoss > _bosses.Count - 1)
        {
            _currentBoss  = 0;
            _currentPhase = 1;
        }

        SetUIImage();
        SetUIColorSet();

        _canvas.alpha = 1f;
        _canvas.blocksRaycasts = true;
        _canvas.interactable = true;

    }

    void SetUIImage()
    {
        var _imageSet = _bosses[_currentBoss].images[_currentPhase - 1];
        _image.sprite = _imageSet.image;
        _imageTrf.pivot = _imageSet.pivot;
        _imageTrf.sizeDelta = _imageSet.imageSize;
        _imageTrf.anchoredPosition = new Vector2(0, _imageTrf.anchoredPosition.y);
    }

    void SetUIColorSet()
    {
        var _colorSet = _bosses[_currentBoss].phaseColor[_currentPhase - 1];
        _gameOverTxt.color = _colorSet.gameOverColor;
        _restartTxt.color  = _colorSet.textColor;  //SHIT DOESNT WORK FUCKING BITCH

        ColorBlock _colors = _menuButton.colors;
        _colors.normalColor = _colorSet.normalColor;
        _colors.highlightedColor = _colorSet.highlightColor;
        _colors.pressedColor = _colorSet.pressColor;
        _menuButton.colors = _colors;
    }


    public void DisableDeathScreen()  // For button event
    {
        MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.FADE_STOP);
    }
    
    public void HideScreen()  // For button event
    {
        _canvas.alpha = 0f;
        _canvas.blocksRaycasts = false;
        _canvas.interactable = false;
    }

    private void Update()
    {
        DeselectButton();
    }
    public void DeselectButton()
    {
        if (currentController == ControllerType.KEYBOARDANDMOUSE)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        //lol why? this makes it impossible to select any other button than the first one?
        //else if(currentController == ControllerType.GAMEPAD)
        //{
        //    GetComponentInChildren<Button>().Select();
        //}
    }
}