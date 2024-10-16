using System.Collections.Generic;
using UnityEngine;
using BulletDance.Graphics;

namespace BulletDance.VFX
{

public class PlayerDashVFX : MonoBehaviour
{
    void OnEnable()
    {
        ResetEmit();
        EventManager.Instance.OnPlayerDash += DashVFX;
    }

    void OnDisable()
    {
        ResetEmit();
        EventManager.Instance.OnPlayerDash -= DashVFX;
    }

    /* Main function */ 
    void DashVFX(BeatTiming hitTiming, Vector2 direction)
    {
        //Blink
        Instantiate(_blink, _playerMovement.StartDashPosition, Quaternion.identity);

        //Don't do any other effects if bad hit timing
        if(hitTiming == BeatTiming.BAD) return;

        //Dust
        _dustOffsetAngle = Mathf.Sin(Vector2.Angle(direction, Vector2.up) * Mathf.Deg2Rad);
        for(int i = 0; i < _dustAmount; i++) {
            Instantiate(_dust, _playerMovement.StartDashPosition + _dustOffset * _dustOffsetAngle, Quaternion.identity);
        }

        //After Image
        DashAfterImage(direction);

        //Dash trail
        EmitDashTrail(hitTiming);
    }


    [SerializeField] PlayerMovement _playerMovement;

    /* Particles */
    [Space] [Header("Particles")]
    [SerializeField] ParticleSystem _blink;
    [SerializeField] ParticleSystem _dust;
    [SerializeField] int _dustAmount = 3;
    [SerializeField] Vector2 _dustOffset = new Vector2(0, -1.3f);
    float _dustOffsetAngle = 0f;

    /* Dash AfterImage */
    [Space] [Header("After Image")]
    [SerializeField] SpriteRenderer _playerSpriteRenderer;
    [SerializeField] Vector3 _afterImageOffset;
    [SerializeField] private float _distanceBetweenImages;
    [SerializeField] private Gradient _dashAfterImageColors;

    void DashAfterImage(Vector2 direction)
    {
        for (int i = 0; i < _playerMovement.dashDistance; i+=2)
        {
            AfterImage img = AfterImagePool.Instance.GetFromPool(
                transform, _playerSpriteRenderer, _afterImageOffset, 
                _dashAfterImageColors.Evaluate(i/_playerMovement.dashDistance), true, true);
            img.transform.position = (Vector3)_playerMovement.StartDashPosition + ((Vector3)direction * i) + _afterImageOffset;
        }
    }


    /* Dash Trail */

    [Space] [Header("Graffiti Trail")]
    [SerializeField]
    private List<TrailRenderer> _trailRdr;
    private Color _baseColor;

    public void EmitDashTrail(BeatTiming hitTiming)
    {
        _baseColor = hitTiming == BeatTiming.PERFECT ? Colors.perfectHit: Colors.goodHit;

        for(int i = 0; i < _trailRdr.Count; i++)
        {
            _trailRdr[i].emitting = true;

            _trailRdr[i].material.SetColor("_BaseColor", _baseColor);
            _trailRdr[i].material.SetFloat("_Hue", i * -0.6f + Random.Range(-0.1f * (i+1), 0.1f * (i+1)));
            _trailRdr[i].material.SetFloat("_Shape", Random.Range(5, 60));
            _trailRdr[i].material.SetFloat("_Intensity", Random.Range(7f, 30f));
            _trailRdr[i].material.SetFloat("_Dispersity", Random.Range(0.15f, 0.25f) * (Random.Range(0, 2) == 0 ? -1f : 1f));
        }

        _isEmitting = true;
    }

    private bool  _isEmitting = false;
    private float _frames = 0;
    [SerializeField]
    private float _emitFrame = 2;

    void Update()
    {
        //Only update when trail is emitting
        if(!_isEmitting) return;

        //Timer
        _frames++;
        if(_frames < _emitFrame) return;

        ResetEmit();
    }

    void ResetEmit()
    {
        _frames = 0;
        _isEmitting = false;
        foreach(TrailRenderer trRdr in _trailRdr)
        {
            trRdr.emitting = false;
        }
    }

}

}