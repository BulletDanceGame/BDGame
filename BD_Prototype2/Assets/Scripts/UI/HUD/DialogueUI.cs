using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class DialogueUI : MonoBehaviour
{
    public enum Speaker{ LEFT, RIGHT };

    private CanvasGroup _canvas;

    [SerializeField]
    private Image _leftImg, _rightImg, _dialBox;

    [SerializeField]
    private TMPro.TextMeshProUGUI _dialText;

    [SerializeField]
    private GameObject _voiceline, _soundclip, _typewritter;
    private BulletDance.Audio.CutsceneSounds _cutsceneSounds;


    public static DialogueUI Instance;
    void Awake() { Instance = this; }  //We're not carrying UI over every scene so this is fine

    void Start()
    {
        _canvas = GetComponent<CanvasGroup>();
        _canvas.alpha = 0;

        _cutsceneSounds = FindObjectOfType<BulletDance.Audio.CutsceneSounds>();
    }

    public void SetVisible(bool enable)
    {
        _canvas.alpha = enable ? 1f : 0f;
    }


    // -- Set the character image -- //
    public void SetImage(Speaker speakerPosition, Sprite characterSprite)
    {
        if(speakerPosition == Speaker.LEFT)
        {
            _leftImg.gameObject.SetActive(true);
            _leftImg.sprite = characterSprite;
            _rightImg.gameObject.SetActive(false);
        }
        else
        {
            _rightImg.gameObject.SetActive(true);
            _rightImg.sprite = characterSprite;
            _leftImg.gameObject.SetActive(false);
        }
    }

    public void DeactivateImage()
    {
        _leftImg.gameObject.SetActive(false);
        _rightImg.gameObject.SetActive(false);
    }


    // -- Set box -- //
    public void SetDialBoxActive(bool enable)
    {
        _dialBox.gameObject.SetActive(enable);
        _dialText.gameObject.SetActive(enable);
    }


    // -- Set text -- //
    public void SetText(string displayText)
    {
        _dialText.text = displayText;
    }


    // -- Play sound -- //
    public void PlayVoiceline(AK.Wwise.Event voiceline)
    {
        voiceline.Post(_voiceline);
    }

    public void PlaySoundclip(AK.Wwise.Event soundclip)
    {
        soundclip.Post(_soundclip);
    }

    public void PlayTextletter(string speaker)
    {
        if(!EditorCheck.inEditMode)
            _cutsceneSounds.DialogueLetter(speaker);
        else
            _cutsceneSounds.GetDialogueLetterSFX(speaker).Post(_typewritter);
    }

}