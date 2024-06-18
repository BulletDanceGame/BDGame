using UnityEngine;
using BulletDance.Graphics;
using System.Collections.Generic;

public class MusicUI : MonoBehaviour
{
    private bool _shown = true;


    [SerializeField] private SpriteRenderer[] _uiElements;
    [SerializeField] private ParticleSystem _particles;

    [SerializeField] private GameObject _elementPrefab;
    [SerializeField] private string _beatAnimationName;

    private List<GameObject> _rhythmElements = new List<GameObject>();

    [SerializeField] private bool _setAnticipation;

    private void Start()
    {
        if (_setAnticipation)
        {
            EventManager.Instance.OnBeatForVisuals += StartRhythmUIAnimation;

        }
        else
        {
            EventManager.Instance.OnPlayerRhythmBeat += StartRhythmUIAnimation;

        }
        EventManager.Instance.OnCutsceneStart += StopRhythmUIAnimation;
        EventManager.Instance.OnPlayerAttack   += UIFeedback;
        EventManager.Instance.OnPlayerDash     += UIFeedback;

        Create();
    }

    private void OnDestroy()
    {
        if (_setAnticipation)
        {
            EventManager.Instance.OnBeatForVisuals -= StartRhythmUIAnimation;

        }
        else
        {
            EventManager.Instance.OnPlayerRhythmBeat -= StartRhythmUIAnimation;

        }
        EventManager.Instance.OnCutsceneStart -= StopRhythmUIAnimation;
        EventManager.Instance.OnPlayerAttack   -= UIFeedback;
        EventManager.Instance.OnPlayerDash     -= UIFeedback;

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _shown = !_shown;
            if (_shown)
            {
                foreach (SpriteRenderer ui in _uiElements)
                {
                    ui.enabled = true;
                }
                if (_setAnticipation) { EventManager.Instance.OnBeatForVisuals += StartRhythmUIAnimation; }
                else { EventManager.Instance.OnPlayerRhythmBeat += StartRhythmUIAnimation; }
            }
            else
            {
                foreach (SpriteRenderer ui in _uiElements)
                {
                    ui.enabled = false;
                }
                if (_setAnticipation) { EventManager.Instance.OnBeatForVisuals -= StartRhythmUIAnimation; }
                else { EventManager.Instance.OnPlayerRhythmBeat -= StartRhythmUIAnimation; }
            }
        }
    }


    private void Create()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject e = Instantiate(_elementPrefab, transform);
            _rhythmElements.Add(e);
            e.name = "Element " + i;
        }
    }


    public void StartRhythmUIAnimation(int anticipation, float s, int beat)
    {
        if (anticipation != 8) return;

        for (int i = 0; i < _rhythmElements.Count; i++) 
        {            
            if (_rhythmElements[i].GetComponentInChildren<SpriteRenderer>().enabled == false)
            {
                _rhythmElements[i].GetComponent<Animator>().speed = 1f / ((float)MusicManager.Instance.secondsPerBeat * 8); //remove instance
                _rhythmElements[i].GetComponent<Animator>().Play(_beatAnimationName);
                _rhythmElements[i].GetComponentInChildren<SpriteRenderer>().color = (beat % 2 == 1) ? Color.cyan : Color.white;
                break;
            }
        }
    }

    public void StartRhythmUIAnimation(int anticipation)
    {
        for (int i = 0; i < _rhythmElements.Count; i++)
        {
            if (_rhythmElements[i].transform.GetChild(0).gameObject.activeSelf == false)
            {
                _rhythmElements[i].GetComponent<Animator>().speed = 1f / ((float)MusicManager.Instance.secondsPerBeat * anticipation); //remove instance
                _rhythmElements[i].GetComponent<Animator>().Play(_beatAnimationName);
                break;
            }
        }
    }


    public void StopRhythmUIAnimation(string s)
    {
        //this doesnt work - can be removed
        //print("stop");
        for (int i = 0; i < _rhythmElements.Count; i++)
        {
            _rhythmElements[i].GetComponentInChildren<SpriteRenderer>().enabled = false;
            _rhythmElements[i].GetComponent<Animator>().StopPlayback();
        }
    }


    public void UIFeedback(BeatTiming quality, Vector2 none)
    {
        Color color = new Color();
        float psRate = 0f;

        switch(quality)
        {
            case BeatTiming.PERFECT:
                color = Colors.perfectHit;
                psRate = 200;
                break;
            case BeatTiming.GOOD:
                color = Colors.goodHit;
                psRate = 100;
                break;
            case BeatTiming.BAD:
                color = Colors.badHit;
                psRate = 50;
                break;
            default: break;
        }

        SetParticles(color, psRate);
    }

    
    void SetParticles(Color color, float psRate)
    {
        if(!_particles) return;

        var main = _particles.main;
        var emission = _particles.emission;
        main.startColor = color;
        emission.rateOverTime = psRate;
        _particles.Play();
    }
}
