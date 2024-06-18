using UnityEngine;

public class UIBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform _barFillTrf;
    private Vector3 _oriBarPos;
    private Vector3 _displayBarPos;

    [SerializeField]
    private float _fillSpd;
    private float _currentFill = 1f;

    private float _curBarPosX  = 0f;
    private float _currentRatio;
    private float _currentDisplayedValue;
    private float _maxValue;

    [SerializeField]
    private TMPro.TextMeshProUGUI _textDisplay;


    public void SetUpUIBar(float maxValue)
    {
        SetUpUIBar(maxValue, maxValue);
    }
    public void SetUpUIBar(float maxValue, float startValue)
    {
        _maxValue = maxValue;
        _currentDisplayedValue = startValue;
        _oriBarPos = _barFillTrf.anchoredPosition;
        _displayBarPos = _oriBarPos;
        _displayBarPos.x = (_currentDisplayedValue / _maxValue) * _oriBarPos.x;
        _barFillTrf.anchoredPosition = _displayBarPos;

        UpdateBar();
    }


    //Need subscription or referenced method call
    public void SetMaxValue(float maxValue)
    {
        _maxValue = maxValue;
        UpdateBar();
    }

    public void SetValue(float value)
    {
        _currentDisplayedValue = value;
        UpdateBar();
    }

    public void DecreaseValue(float amount)
    {
        _currentDisplayedValue -= amount;
        UpdateBar();
    }

    public void IncreaseValue(float amount)
    {
        _currentDisplayedValue += amount;
        UpdateBar();
    }


    void UpdateBar()
    {
        //Clamp value, min 0 -OR- max value
        _currentDisplayedValue = Mathf.Clamp(_currentDisplayedValue, 0f, _maxValue); 

        _currentRatio = _currentDisplayedValue/_maxValue;
        _currentFill = 0f;
        _curBarPosX = _barFillTrf.anchoredPosition.x;

        //Don't execute text function if there's none referenced
        if(_textDisplay != null)
            _textDisplay.text = _currentDisplayedValue.ToString();
    }


    //Private update, excecute when value changed
    void Update()
    {
        if(_currentFill >= 1f) return;

        _currentFill += _fillSpd * Time.deltaTime;
        _currentFill = Mathf.Clamp(_currentFill, 0f, 1f); 

        _displayBarPos.x = Mathf.Lerp(_curBarPosX, _oriBarPos.x*_currentRatio, _currentFill);

        _barFillTrf.anchoredPosition  = _displayBarPos;
    }
}