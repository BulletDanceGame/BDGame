using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class GlitchEffect : MonoBehaviour
{
    private Image GlitchVFX;
    private Material _material;
    private float Color;
    private float CircleSize;


    public BeatTiming hitTiming { get; private set; }

    private void Start()
    {
        EventManager.Instance.OnPlayerNormal += ResetColorStrength;

        EventManager.Instance.OnPlayerAttack += OnGlicthVFX;
        EventManager.Instance.OnPlayerDash   += OnGlicthVFX;
        GlitchVFX = GetComponent<Image>();
        _material = GetComponent<Image>().material;
        GlitchVFX.enabled = false;
        if (_material == null) return;
        Color = _material.GetFloat("_ColorStrength");
        CircleSize = 1.4f;
        _material.SetFloat("_CircleSize", CircleSize);
    }

    void ResetColorStrength()
    {
        Color = 0;
        CircleSize = 1.4f;
        _material.SetFloat("_ColorStrength", Color);
        _material.SetFloat("_CircleSize", CircleSize);
        GlitchVFX.enabled = false;

    }

    void OnGlicthVFX(BeatTiming hitTiming, Vector2 none)
    {
        switch (hitTiming)
        {
            case BeatTiming.PERFECT:
                GlitchVFX.enabled = false;
                CircleSize = 1.4f;
                MangaEffect.Instance.TurnOffFlicker();
                break;

            case BeatTiming.GOOD:
                GlitchVFX.enabled = false;
                CircleSize = 1.4f;
                MangaEffect.Instance.TurnOffFlicker();

                break;

            case BeatTiming.BAD:
                OnGlitchDo();
                
                break;
            case BeatTiming.NONE:
                GlitchVFX.enabled = false;
                CircleSize = 1.4f;
                MangaEffect.Instance.TurnOffFlicker();

                break;
        }
    }

    void OnGlitchDo()
    {
        //1st = 1
        if (UnitManager.Instance.GetPlayer().GetComponentInChildren<Player>().playerFailState == Player.PlayerFailState.FAILED)
        {
            ResetColorStrength();
            return;
        }
        CircleSize = CircleSize-0.25f;
        MangaEffect.Instance.TriggerFlicker();
        _material.SetFloat("_CircleSize", CircleSize);

        GlitchVFX.enabled = true;
        //GlitchVFX.enabled = false;
    }

    private void Update()
    {
        if(GlitchVFX.enabled)
        {
            //MangaEffect.Instance.TriggerFlicker();
        }
    }

}
