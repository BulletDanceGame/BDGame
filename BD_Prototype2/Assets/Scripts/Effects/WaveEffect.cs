using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEffect : MonoBehaviour
{

    [SerializeField] private float _shockWaveTime = 0.75f;
    private Material _material;
    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

    // Start is called before the first frame update
    void Start()
    {
        _material=GetComponent<SpriteRenderer>().material;

    }

    private void Update()
    {
        ShockWave();        
    }


    private void Awake()
    {
        ShockWave();
        Destroy(gameObject, 2f);
    }

    public void ShockWave()
    {
        StartCoroutine(ShockWave(-0.1f, 1f));
    }

    IEnumerator ShockWave(float startPos,float endPos)
    {
        _material.SetFloat(_waveDistanceFromCenter, startPos);

        float lerpedAmount = 0f;
        float elapsedTime = 0f;
        while(elapsedTime < _shockWaveTime)
        {
            elapsedTime+=Time.deltaTime;
            lerpedAmount=Mathf.Lerp(startPos, endPos, (elapsedTime/_shockWaveTime));
            _material.SetFloat(_waveDistanceFromCenter, lerpedAmount);

            yield return null;
        }
    }

}
