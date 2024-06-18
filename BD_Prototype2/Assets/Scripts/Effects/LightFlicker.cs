using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightFlicker : MonoBehaviour
{
    private UnityEngine.Rendering.Universal.Light2D _light;
    [SerializeField]
    private float _flickerSpd = 1;
    private float _initBrightness, _time = 0;

    private void Start()
    {
        _light = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        if(_light == null)
        {
            this.enabled = false;
            return;
        }

        //Set the beginning key values to initial fx values
        _initBrightness = _light.intensity;
        _time = Random.Range(0, Mathf.PI*0.5f); //Starting time is random so the lights don't look uniformed
    }

    private void Update()
    {
        _light.intensity = Mathf.Clamp(_initBrightness*(Mathf.Sin(_time)), 0, _initBrightness);

        if(_time > Mathf.PI) _time = 0;
        _time += Time.deltaTime*_flickerSpd;
    }
}
