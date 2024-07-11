using UnityEngine;
using BulletDance.Graphics;
using System.Collections;
using System.Collections.Generic;

public class BulletFX : MonoBehaviour
{
    [SerializeField] SpriteRenderer _bulletSprite;
    [SerializeField] SpriteRenderer[] _pulses;
    Color _currentcolor, _ogcolor, _ogPulseColor;

    [Space]
    [SerializeField] float _hitSize;
    [SerializeField] TrailRenderer  _hitTrail;
    [SerializeField] ParticleSystem _perfectTrail;
    [SerializeField] ParticleSystem _explodePS;
     
    [SerializeField] ParticleSystem _lastHitExplosion;
    [SerializeField] ParticleSystem _lineExplosion;

    [SerializeField] ParticleSystem _missExplosion;

    [Space]
    [SerializeField] private GameObject _bulletIndicatorPrefab;
    private GameObject _bulletIndicator;
    private Bullet     _bulletParent;

    [Space]
    [SerializeField] private GameObject _shockWave;

    // --- Enable & Disable --- //
    public void SetUp(Bullet bulletParent)
    {
        var trailMain = _perfectTrail.main;
        trailMain.startColor = new ParticleSystem.MinMaxGradient(Colors.CreateGradient(Colors.perfectHit, Color.white));
        _hitTrail.enabled = false;

        _ogcolor = _bulletSprite.color;
        _ogPulseColor = _pulses[0].color;

        _bulletParent = bulletParent;
    }

    public void Reset()
    {
        _bulletSprite.color = _ogcolor;
        SetPulseColor(_ogPulseColor);
        transform.localScale = new Vector3(1, 1, 1);
        _hitTrail.enabled = false;

        if (_bulletIndicator)
        {
            Destroy(_bulletIndicator);
            _bulletIndicator = null;
        }
    }


    // --- Helper methods --- //
    void SetPulseColor(Color color)
    {
        for(int i = 0; i < _pulses.Length; i++)
        {
            _pulses[i].color = color;
        }        
    }

    public void CreateParticle(Color startColor, Color endColor)
    {
        var _particle = Instantiate(_explodePS,transform.position, Quaternion.identity);
        var _main = _particle.main;
        _main.startColor = new ParticleSystem.MinMaxGradient(Colors.CreateGradient(startColor, endColor));
    }



    // --- Public FX methods --- //
    public void PerfectFX()
    {
        _bulletSprite.color = Colors.perfectHit;
        SetPulseColor(Colors.perfectHit);

        CreateParticle(Colors.perfectHit, Color.white);
        _perfectTrail.Play();
        _hitTrail.enabled    = true;
        _hitTrail.startColor = Colors.perfectHit;

        transform.localScale += new Vector3(0, _hitSize, 0);

    }


    public void GoodFX()
    {
        _bulletSprite.color = Colors.goodHit;
        SetPulseColor(Colors.goodHit);

        CreateParticle(Colors.goodHit, Color.white);
        _hitTrail.enabled    = true;
        _hitTrail.startColor = Colors.goodHit;
    }

    public void LastHitFX()
    {
        _bulletSprite.color = Color.red;
        SetPulseColor(Color.red);

        CreateParticle(Color.red, Color.white);
        _hitTrail.enabled = true;
        _hitTrail.startColor = Color.red;

        _lastHitExplosion.Play();
        Instantiate(_perfectTrail,transform.position,Quaternion.identity);
        Instantiate(_lineExplosion,transform.position,Quaternion.identity);
    }


    public void MissFX()
    {
        Vector3 shockwavepos=Camera.main.WorldToScreenPoint(transform.position);

        _currentcolor = _bulletSprite.color;
        //Instantiate(_shockWave, shockwavepos, Quaternion.identity);

        var _particle = Instantiate(_missExplosion, transform.position, Quaternion.identity);
        var _main = _particle.main;
        _main.startColor = new ParticleSystem.MinMaxGradient(Colors.CreateGradient(_currentcolor, Color.white));
    }

    public void DestroyFX()
    {
        _currentcolor = _bulletSprite.color;
        CreateParticle(_currentcolor, _currentcolor);
    }


    // --- Out-of-camera bullet indicator --- //
    public void CreateBulletIndicator()
    {
        _bulletIndicator = Instantiate(_bulletIndicatorPrefab, transform.position, Quaternion.identity);

        Vector2 pos = transform.position;
        Vector2 camPos = Camera.main.transform.position;

        for (int i = 0; i < 100; i++)
        {
            pos += _bulletParent.GetDir();
            if (pos.x < camPos.x + Camera.main.orthographicSize * (16f / 9f) - 5 &&
                pos.x > camPos.x - Camera.main.orthographicSize * (16f / 9f) + 5 &&
                pos.y < camPos.y + Camera.main.orthographicSize - 5 &&
                pos.y > camPos.y - Camera.main.orthographicSize + 5)
            {
                break;
            }
        }
        _bulletIndicator.transform.position = pos;
    }

    void Update()
    {
        //indicator!
        //ManageIndicator();
    }

    void ManageIndicator()
    {
        if(!_bulletIndicator) return;

        Vector2 camPos = Camera.main.transform.position;

        if (transform.position.x < camPos.x + Camera.main.orthographicSize * (16f / 9f) &&
            transform.position.x > camPos.x - Camera.main.orthographicSize * (16f / 9f) &&
            transform.position.y < camPos.y + Camera.main.orthographicSize &&
            transform.position.y > camPos.y - Camera.main.orthographicSize)
        {
            Destroy(_bulletIndicator);
            _bulletIndicator = null;
        }

        else
        {
            Vector2 pos = transform.position;
            for (int i = 0; i < 100; i++)
            {
                pos += _bulletParent.GetDir();
                if (pos.x < camPos.x + Camera.main.orthographicSize * (16f / 9f) - 5 &&
                    pos.x > camPos.x - Camera.main.orthographicSize * (16f / 9f) + 5 &&
                    pos.y < camPos.y + Camera.main.orthographicSize - 5 &&
                    pos.y > camPos.y - Camera.main.orthographicSize + 5)
                {
                    break;
                }
            }
            _bulletIndicator.transform.position = Vector2.MoveTowards(_bulletIndicator.transform.position, pos, 5 * Time.deltaTime);
        }
    }
}