using System.Collections.Generic;
using UnityEngine;


namespace BulletDance.Audio //Ignore indent of this {} bc that's annoying
{

/*
    This is the base class for containing specific SFX & rtpc controls.
    Use EventManager to execute sounds.
*/
public class SoundContainer : MonoBehaviour
{
    protected SoundManager manager;

    [SerializeField]
    protected List<SFXGroup> _sfxGroups;
    protected List<SFX> _sfxList;


    // -- Event Hooks & sound initialization -- //
    protected void Initialize()
    {
        GetSoundManager();
        MakeSFXList();

        if(EditorCheck.inEditMode) return;
        SubscribeEvents();
    }

    void GetSoundManager()
    {
        manager = SoundManager.Instance;
    }

    void MakeSFXList()
    {
        _sfxList = new List<SFX>();

        foreach(SFXGroup group in _sfxGroups)
        {
            foreach(SFX sfx in group.sfxList)
            {
                _sfxList.Add(sfx);
            }
        }
    }

    void SubscribeEvents()
    {
        EventManager.Instance.OnPlaySFX += PlaySFX;
    }

    protected void DeInitialize()
    {
        if(EditorCheck.inEditMode) return;
        UnsubscribeEvents();
    }

    void UnsubscribeEvents()
    {
        EventManager.Instance.OnPlaySFX -= PlaySFX;
    }

    // -- Helper method -- //
    protected AK.Wwise.Event GetSFX(string sfxName)
    {
        foreach(SFX sfx in _sfxList)
        {
            if(sfx.sfxName == sfxName)
                return sfx.sfxEvent;
        }

            return null;
    }


    // -- SoundManager wrapper -- //
    //For easier method calling

    /// <summary> Play a sound in the SFX list </summary>
    /// <param name="sfxName">Name defined in the SFX list</param>
    /// <param name="duration">(Optional) Time until sfx cuts off, default is 1s</param>
    /// <param name="source">(Optional) Which GameObject is the sound source (for spatial sound), default is MusicManager GameObject</param>
    /// <param name="playOnce">(Optional) Only one of this sound plays at any given moment, which prevents voice starvation, default is false</param>
    protected void PlaySFX(string sfxName, float duration = 1f, GameObject source = null, bool playOnce = false)
    {
        AK.Wwise.Event sound = GetSFX(sfxName);
        if (sound != null)
            manager.PlaySFX(sound, duration, source, playOnce);
    }

    /// <summary> Play a specified Wwise event </summary>
    /// <param name="duration">(Optional) Time until sfx cuts off, default is 1s</param>
    /// <param name="source">(Optional) Which GameObject is the sound source (for spatial sound), default is MusicManager GameObject</param>
    /// <param name="playOnce">(Optional) Only one of this sound plays at any given moment, which prevents voice starvation, default is false</param>
    protected void PlaySFX(AK.Wwise.Event sound, float duration = 1f, GameObject source = null, bool playOnce = false)
    {
        manager.PlaySFX(sound, duration, source, playOnce);
    }
}

}