using System.Collections.Generic;
using UnityEngine;
using BulletDance.Animation;
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
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] ParticleSystem _blink, _dust;
    [SerializeField] int _dustAmount = 3;
    void DashVFX(BeatTiming hitTiming, Vector2 direction)
    {
        //Blink
        Instantiate(_blink, _playerMovement.StartDashPosition, Quaternion.identity);

        //Don't do any other effects if bad hit timing
        if(hitTiming == BeatTiming.BAD) return;

        //Dust
        for(int i = 0; i < _dustAmount; i++) {
            Instantiate(_dust, _playerMovement.StartDashPosition, Quaternion.identity);
        }

        //Dash trail
        EmitDashTrail(hitTiming);

        //After Image
        DashAfterImage(direction);
    }


    /* Dash Trail */

    [Space] [SerializeField]
    private List<TrailRenderer> _trailRdr;
    private Color _baseColor;

    public void EmitDashTrail(BeatTiming hitTiming)
    {
        _baseColor = hitTiming == BeatTiming.PERFECT ? Colors.perfectHit: Colors.goodHit;

        for(int i = 0; i < _trailRdr.Count; i++)
        {
            _trailRdr[i].emitting = true;

            _trailRdr[i].material.SetColor("_BaseColor", _baseColor);
            _trailRdr[i].material.SetFloat("_Hue", i * -0.6f + Random.Range(-0.07f, 0.07f));
            _trailRdr[i].material.SetFloat("_Shape", Random.Range(0, 60));
            _trailRdr[i].material.SetFloat("_Intensity", Random.Range(7f, 30f));
            _trailRdr[i].material.SetFloat("_Dispersity", Random.Range(-0.25f, 0.25f));
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


    /* Dash AfterImage */
    [Space]
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
}

}