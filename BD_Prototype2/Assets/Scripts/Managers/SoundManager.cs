using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletDance.Audio;
using System;

/*
    This class handles SFX playing and gradual RTPC changes.
        Only contains helper methods and sfx & rtpc play queue
    Non-audio class should NOT REFERENCE this class.
*/
public class SoundManager : MonoBehaviour
{
    public  static SoundManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    SFXQueue  _sfxQueue  = new SFXQueue();
    RTPCQueue _rtpcQueue = new RTPCQueue();

    [Serializable]
    public struct SpecificSoundsContainer { public GameObject prefab; public SceneManager.Scenes scene; }

    [SerializeField]
    List<SpecificSoundsContainer> _bossSpecificSounds, _levelSpecificSounds;
    GameObject _currentBossSound, _currentLevelSound;

    // -- Initialization -- //
    void Start()
    {
        EventManager.Instance.OnStartPlaying += LoadSound;

        //Create sfx player queue
        _sfxQueue.SetDefaultSFXPlayerParent(gameObject);
        _sfxQueue.CreateSFXPlayer(5);
    }

    private void OnDisable()
    {
        EventManager.Instance.OnStartPlaying -= LoadSound;
    }

    // -- Load & Unload Sounds -- //
    // -- EDIT LATER FOR LEVEL LOADING -- //
    void LoadSound()
    {
        //Level sounds
        if (_currentLevelSound == null)
        {
            foreach (SpecificSoundsContainer sounds in _levelSpecificSounds)
            {
                if (sounds.scene == SceneManager.Instance._currentScene)
                {
                    _currentLevelSound = Instantiate(sounds.prefab, this.transform);
                }
            }
        }

        //Boss sounds
        if (_currentBossSound == null)
        {
            foreach (SpecificSoundsContainer sounds in _bossSpecificSounds)
            {
                if (sounds.scene == SceneManager.Instance._currentScene)
                {
                    _currentBossSound = Instantiate(sounds.prefab, this.transform);
                }
            }
        }
    }

    void UnloadSound()
    {
        Destroy(_currentLevelSound);
        Destroy(_currentBossSound);

        _currentLevelSound = null;
        _currentBossSound  = null;
    }


    // -- Update -- //
    void Update()
    {
        _sfxQueue.Update();
    }


    // -- Common helper methods -- //
    public void PlaySFX(AK.Wwise.Event sound, float duration = 1f, GameObject source = null, bool playOnce = false)
    {
        SFXPlayer sfxPlayer = _sfxQueue.GetSFXPlayer(playOnce ? sound : null);

        //Found an existing sfxPlayer
        if(playOnce && sfxPlayer.soundPlaying != null)
        {
            //Sound is currently playing, escape
            if(sfxPlayer.state) return;
            else    sfxPlayer.SetActive(true);
        }

        if(source == null) source = gameObject;
        sfxPlayer.SetParent(source);
        sfxPlayer.SetDuration(duration);
        sfxPlayer.SetSound(sound);

        sound.Post(sfxPlayer.player);
    }


    void StopExistingCoroutine(string[] rtpcNames)
    {
        RTPCCoroutine coroutine = _rtpcQueue.GetCoroutine(rtpcNames);
        if(coroutine == null) return;
        StopCoroutine(coroutine.coroutine);
        _rtpcQueue.Remove(coroutine);
    }

    public IEnumerator Fade(float initValue, float newValue, float fadeSpeed, string[] rtpcNames)
    {
        float rtpcValue = initValue;
        float progess   = 0;

        while (rtpcValue != newValue)
        {
            foreach (string rtpcName in rtpcNames)
            {
                RTPCManager.Instance.SetValue(rtpcName, rtpcValue);
            }

            rtpcValue = Mathf.Lerp(initValue, newValue, progess);
            progess += fadeSpeed/100f * Time.deltaTime;
            yield return null;
        }

        _rtpcQueue.Remove(_rtpcQueue.GetCoroutine(rtpcNames));
    }
}