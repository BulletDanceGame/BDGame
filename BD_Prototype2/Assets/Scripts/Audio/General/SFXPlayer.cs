using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Audio
{
    public class SFXPlayer
    {
        public  GameObject player { get; private set; }
        public  bool  state { get; private set; }
        public  AK.Wwise.Event soundPlaying { get; private set; }
        private float duration = 1f;
        private float disableTimer = 0f;

        public SFXPlayer(GameObject parent)
        {
            player = new GameObject("SFX Player");
            player.transform.parent = parent.transform;
            duration = 1f;
            this.SetActive(false);
        }

        public void SetParent(GameObject parent)
        {
            if(parent != null) player.transform.parent = parent.transform;
        }

        public void SetDuration(float playDuration)
        {
            if(playDuration > 0) duration = playDuration;
        }

        public void SetSound(AK.Wwise.Event sound)
        {
            soundPlaying = sound;
        }

        public void UpdateTimer()
        {
            if(disableTimer < duration)
                disableTimer += Time.deltaTime;
            else
                this.SetActive(false);
        }

        public void SetActive(bool isActive)
        {
            player.SetActive(isActive);
            state = isActive;
            disableTimer = 0;
            soundPlaying = null;
        }    
    }

    public class SFXQueue
    {
        private List<SFXPlayer> _sfxQueue;
        private GameObject _defaultSfxPlayerParent;

        public SFXQueue()
        {
            _sfxQueue = new List<SFXPlayer>();
        }

        public void SetDefaultSFXPlayerParent(GameObject parent)
        {
            _defaultSfxPlayerParent = parent;
        }

        public void CreateSFXPlayer(int amount)
        {
            for(int i = 0; i < amount; i++)
            {
                var sfxPlayer = new SFXPlayer(_defaultSfxPlayerParent);
                _sfxQueue.Add(sfxPlayer);
            }
        }

        SFXPlayer GetSFXPlayerFromQueue(AK.Wwise.Event soundQuery = null)
        {
            //Find inactive player
            foreach(SFXPlayer sfxPlayer in _sfxQueue)
            {
                if(soundQuery != null && soundQuery == sfxPlayer.soundPlaying)
                    return sfxPlayer;

                if(!sfxPlayer.state)
                {
                    sfxPlayer.SetActive(true);
                    return sfxPlayer;
                }
            }

            return null;
        }

        public SFXPlayer GetSFXPlayer(AK.Wwise.Event soundQuery = null)
        {
            SFXPlayer sfxPlayer = GetSFXPlayerFromQueue(soundQuery);
            if(sfxPlayer != null) return sfxPlayer;
    
            //Failed to get player, create more
            CreateSFXPlayer(3);
            return GetSFXPlayerFromQueue();
        }

        public void Update()
        {
            foreach(SFXPlayer sfxPlayer in _sfxQueue)
            {
                sfxPlayer.UpdateTimer();
            }
        }
    }
}