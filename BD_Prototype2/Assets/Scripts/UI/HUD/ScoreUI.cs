using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _comboBreakFX;
    private Vector3        _oriParticlePos;
    private Camera         _camera;

    [SerializeField]
    private TextMeshProUGUI _txtCurrency, _txtScore, _txtMulti;
    private RectTransform   _txtMultiRTrf;
    private Vector2 multiplerSize;

    [SerializeField]
    private Animator currencyAnim, scoreAnim;


    private void Start()
    {
        _camera         = Camera.main;
        _oriParticlePos = _comboBreakFX.transform.position;

        _txtMultiRTrf = _txtMulti.GetComponent<RectTransform>();
        multiplerSize = _txtMultiRTrf.sizeDelta;

        _txtCurrency.text = "00000";
        _txtScore.text    = "00000";
        _txtMulti.text    = "1x";

        EventManager.Instance.OnAddScore    += AddScore;
        EventManager.Instance.OnPlayerMiss  += LoseCombo;
        EventManager.Instance.OnAddCurrency += AddCurrency;
    }


    //Need to QUEUE 1 frame bc it depends on the ScoreManager score being added first
    void AddScore(int none)
    {
        StartCoroutine(UpdateScoreUI());
    }

    void AddCurrency(int none)
    {
        StartCoroutine(UpdateCurrencyUI());
    }

    IEnumerator UpdateScoreUI()
    {
        yield return null;

        _txtScore.text = Mathf.Clamp(ScoreManager.Instance.SessionScore, 0f, 99999f).ToString().PadLeft(5, '0');
        _txtMulti.text = ScoreManager.Instance.ComboMulti + "x";
        _txtMultiRTrf.sizeDelta = new Vector2(multiplerSize.x, multiplerSize.y * (1 + (ScoreManager.Instance.ComboMulti-1)*0.125f));
        
        _txtCurrency.text = Mathf.Clamp(CurrencyManager.Instance.Currency, 0f, 99999f).ToString().PadLeft(5, '0'); //not sure if this should be here

        scoreAnim.Play("FX.Score_AddScore");
    }

    IEnumerator UpdateCurrencyUI()
    {
        yield return null;

        _txtCurrency.text = Mathf.Clamp(CurrencyManager.Instance.Currency, 0f, 99999f).ToString().PadLeft(5, '0'); //not sure if this should be here

        //currencyAnim.Play("humu humu"); <--- needs an actual animation
    }


    //Need to QUEUE 1 frame bc it depends on the ScoreManager score being added first
    void LoseCombo()
    {
        StartCoroutine(MissFeedback());
    }

    IEnumerator MissFeedback()
    {
        yield return null;

        _txtMulti.text = ScoreManager.Instance.ComboMulti + "x";
        _txtMultiRTrf.sizeDelta = new Vector2(multiplerSize.x, multiplerSize.y * (1 + (ScoreManager.Instance.ComboMulti-1)*0.125f));

        scoreAnim.Play("FX.Score_LoseCombo");

        Vector3 particlePos = _camera.ScreenToWorldPoint(_oriParticlePos);
        particlePos.z = 0;
        _comboBreakFX.transform.position = particlePos;

        _comboBreakFX.Clear();
        _comboBreakFX.Play();
    }


    private void OnDestroy()
    {
        EventManager.Instance.OnAddScore    -= AddScore;
        EventManager.Instance.OnPlayerMiss  -= LoseCombo;
        EventManager.Instance.OnAddCurrency -= AddCurrency;
    }
}