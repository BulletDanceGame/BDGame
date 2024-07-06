using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MangaEffect : MonoBehaviour
{
    [SerializeField]
    private float _fxScale = 2000;
    [SerializeField]
    private float _expandSpeed, _shrinkSpeed;
    private bool _isExpand, _isSkrink;

    [SerializeField]
    private float _animationDuration = 5;
    private Material _mat;
    private float _initContrast, _initBrightness;
    [SerializeField] 
    private AnimationCurve _saturationOverTime, _contrastOverTime, _brightnessOverTime;



    private void Start()
    {
        EventManager.Instance.OnPlayerTired += TriggerManga;
        EventManager.Instance.OnPlayerNormal += TurnOffManga;

        transform.localScale = new Vector3(0, 0, 0);
        _mat = GetComponent<SpriteRenderer>().material;
        
        if(_mat == null) return;
        _initContrast   = _mat.GetFloat("_GrayScaleContrast");
        _initBrightness = _mat.GetFloat("_Brightness");

        //Set the beginning key values to initial fx values
        SetInitKeyValue(_contrastOverTime, _initContrast);
        SetInitKeyValue(_brightnessOverTime, _initBrightness);
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
        if(_isExpand)
            transform.localScale += new Vector3(_expandSpeed, _expandSpeed, _expandSpeed);
        if(_isSkrink)
            transform.localScale -= new Vector3(_shrinkSpeed, _shrinkSpeed, _shrinkSpeed);

        //Clamp values so it doesn't grow or shrink forever
        float clampSize = Mathf.Clamp(transform.localScale.x, 0, _fxScale);
        transform.localScale = new Vector3(clampSize, clampSize, clampSize);

        if(clampSize >= _fxScale)
            _isExpand = false;
        if (clampSize <=0)
            _isSkrink = false;

        transform.position = UnitManager.Instance.GetPlayer().transform.position;

    }

    void TriggerManga()
    {
        transform.position = UnitManager.Instance.GetPlayer().transform.position;
        _isExpand = true;

        if(routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeOut());
    }

    void TurnOffManga()
    {
        transform.position = UnitManager.Instance.GetPlayer().transform.position;
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
