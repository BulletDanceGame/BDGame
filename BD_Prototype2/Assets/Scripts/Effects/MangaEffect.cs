using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MangaEffect : MonoBehaviour
{
    public static MangaEffect Instance { get; private set; }


    [SerializeField]
    private float _fxScale = 2000;
    [SerializeField]
    private float _expandSpeed, _shrinkSpeed;
    private float _clampSize;
    private bool _isExpand, _isSkrink;

    [SerializeField]
    private float _animationDuration = 5;
    private Material _mat;
    private float _initContrast, _initBrightness;
    [SerializeField] 
    private AnimationCurve _saturationOverTime, _contrastOverTime, _brightnessOverTime;

    private bool isFlicker = false;
    private float timeDelay;

    public GameObject LineGlitch;
    private void Start()
    {
        EventManager.Instance.OnPlayerTired  += TriggerManga;
        EventManager.Instance.OnPlayerNormal += TurnOffManga;
        EventManager.Instance.OnCutsceneStart += CutsceneStart;
        EventManager.Instance.OnCutsceneEnd   += CutsceneEnd;

        transform.localScale = new Vector3(0, 0, 0);
        _mat = GetComponent<SpriteRenderer>().material;
        
        if(_mat == null) return;
        _initContrast   = _mat.GetFloat("_GrayScaleContrast");
        _initBrightness = _mat.GetFloat("_Brightness");

        //Set the beginning key values to initial fx values
        SetInitKeyValue(_contrastOverTime, _initContrast);
        SetInitKeyValue(_brightnessOverTime, _initBrightness);

        if(Instance==null)
        {
            Instance = this;
        }
    }

    void OnDestroy()
    {
        EventManager.Instance.OnPlayerTired  -= TriggerManga;
        EventManager.Instance.OnPlayerNormal -= TurnOffManga;
        EventManager.Instance.OnCutsceneStart -= CutsceneStart;
        EventManager.Instance.OnCutsceneEnd   -= CutsceneEnd;
    }


    private void SetInitKeyValue(AnimationCurve curve, float value)
    {
        Keyframe[] keys = curve.keys; // Know that this is making a copy of all keys
        Keyframe keyframe = keys[0];
        keyframe.value = value;
        keys[0] = keyframe;
        curve.keys = keys;
    }

    private void Update()
    {
        //DONT DO SHIT IN CUTSCENE
        if (_isCutscene)
        {
            transform.localScale -= (new Vector3(_shrinkSpeed, _shrinkSpeed, _shrinkSpeed) * 5f);
            _clampSize = Mathf.Clamp(transform.localScale.x, 0, _fxScale);
            transform.localScale = new Vector3(_clampSize, _clampSize, _clampSize);

            return;
        }


        if(_isExpand)
            transform.localScale += new Vector3(_expandSpeed, _expandSpeed, _expandSpeed);
        if(_isSkrink)
            transform.localScale -= new Vector3(_shrinkSpeed, _shrinkSpeed, _shrinkSpeed);

        //Clamp values so it doesn't grow or shrink forever
        _clampSize = Mathf.Clamp(transform.localScale.x, 0, _fxScale);
        transform.localScale = new Vector3(_clampSize, _clampSize, _clampSize);

        if(_clampSize >= _fxScale)
            _isExpand = false;
        if (_clampSize <= 0)
            _isSkrink = false;

        if(UnitManager.Instance.GetPlayer() != null)
            transform.position = UnitManager.Instance.GetPlayer().transform.position;
    }



    IEnumerator Flashing()
    {
        yield return new WaitForSeconds(0.1f);
        LineGlitch.SetActive(true);
        transform.localScale = new Vector3(8000, 8000, 8000);
        timeDelay = Random.Range(0.1f, 0.3f);
        yield return new WaitForSeconds(timeDelay);
        transform.localScale = new Vector3(0, 0, 0);
        LineGlitch.SetActive(false);

        //timeDelay = Random.Range(0.1f, 0.3f);
        //yield return new WaitForSeconds(timeDelay);
        //transform.localScale = new Vector3(8000, 8000, 8000);
        //timeDelay = Random.Range(0.1f, 0.3f);
        //yield return new WaitForSeconds(timeDelay);
        //transform.localScale = new Vector3(0, 0, 0);       
        //timeDelay = Random.Range(0.1f, 0.3f);
        //yield return new WaitForSeconds(timeDelay);
        //transform.localScale = new Vector3(8000, 8000, 8000);
        //timeDelay = Random.Range(0.1f, 0.3f);
        //yield return new WaitForSeconds(timeDelay);
        //transform.localScale = new Vector3(0, 0, 0);
    }

    Coroutine flicker;
    public void TriggerFlicker()
    {
        if(flicker != null) 
        {
            StopCoroutine(flicker);
            flicker = null;
        }
        flicker = StartCoroutine(Flashing());
    }

    public void TurnOffFlicker()
    {
        isFlicker = false;
    }



    void TriggerManga()
    {
        if (_isCutscene) return;

        _isExpand = true;
        if(flicker != null) 
        {
            StopCoroutine(flicker);
            flicker = null;
        }
        if(routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeOut());
    }

    void TurnOffManga()
    {
        _isSkrink = true;
    }



    bool _isCutscene = false;
    void CutsceneStart(string none)
    {
        if(routine != null) StopCoroutine(routine);
        if(flicker != null) 
        {
            StopCoroutine(flicker);
            flicker = null;
        }

        _isSkrink = false;
        _isExpand = false;
        _isCutscene = true;
    }

    void CutsceneEnd()
    {
        _isCutscene = false;
    }

    public void TurnOnForCutscene()
    {
        if(routine != null) StopCoroutine(routine);
        if(flicker != null) 
        {
            StopCoroutine(flicker);
            flicker = null;
        }

        _isCutscene = false;
        _isExpand   = true;
    }

    public void TurnOffForCutscene()
    {
        _isSkrink = true;
    }


    Coroutine routine;
    IEnumerator FadeOut()
    {
        if(_mat == null) yield break;

        float time = 0;
        _mat.SetFloat("_Saturation", 0);
        _mat.SetFloat("_GrayScaleContrast", _initContrast);
        _mat.SetFloat("_Brightness", _initBrightness);

        while(time<1)
        {
            _mat.SetFloat("_Saturation", _saturationOverTime.Evaluate(time));
            _mat.SetFloat("_GrayScaleContrast", _contrastOverTime.Evaluate(time));
            _mat.SetFloat("_Brightness", _brightnessOverTime.Evaluate(time));

            time+=Time.deltaTime*(1/_animationDuration);
            yield return null;
        }
    }
}
