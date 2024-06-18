using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class GlitchEffect : MonoBehaviour
{
    private Image GlitchVFX;
    private Material _material;
    private float Color;

    public BeatTiming hitTiming { get; private set; }

    private void Start()
    {
        EventManager.Instance.OnPlayerTired += ResetColorStrength;

        EventManager.Instance.OnPlayerAttack += OnGlicthVFX;
        EventManager.Instance.OnPlayerDash   += OnGlicthVFX;
        GlitchVFX = GetComponent<Image>();
        _material = GetComponent<Image>().material;
        GlitchVFX.enabled = false;
        if (_material == null) return;
        Color = _material.GetFloat("_ColorStrength");

        _material.SetFloat("_ColorStrength", Color);

    }

    void ResetColorStrength()
    {
        Color = 0;
        _material.SetFloat("_ColorStrength", Color);
    }

    void OnGlicthVFX(BeatTiming hitTiming, Vector2 none)
    {
        switch (hitTiming)
        {
            case BeatTiming.PERFECT:
                GlitchVFX.enabled = false;
                Color = 0;
                break;

            case BeatTiming.GOOD:
                GlitchVFX.enabled = false;
                Color = 0;
                break;

            case BeatTiming.BAD:
                StartCoroutine(OnGlitchDo());
                
                break;
            case BeatTiming.NONE:
                GlitchVFX.enabled = false;
                Color = 0;
                break;
        }
    }

    IEnumerator OnGlitchDo()
    {
        Color = Color + 5;
        _material.SetFloat("_ColorStrength", Color);
        GlitchVFX.enabled = true;
        yield return new WaitForSeconds(0.1f);
        GlitchVFX.enabled = false;
    }

}
