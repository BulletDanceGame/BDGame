using System.Collections;
using UnityEngine;

namespace BulletDance.VFX
{


public class PlayerLowHealthVFX : MonoBehaviour
{
    private bool _isVignetteActive = false;
    private CanvasGroup _lowHealthVignette;
    private Player      _playerRef = null;

    private void Start()
    {
        _lowHealthVignette = GetComponent<CanvasGroup>();
        _playerRef = UnitManager.Instance.GetPlayer()?.GetComponent<Player>();

        if(EventManager.Instance == null) return;
        EventManager.Instance.OnPlayerSpawned += Disable;
        EventManager.Instance.OnPlayerDamage  += Activate;
        EventManager.Instance.OnPlayerHeal    += Deactivate;
    }

    private void OnDestroy()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnPlayerSpawned -= Disable;
        EventManager.Instance.OnPlayerDamage  -= Activate;
        EventManager.Instance.OnPlayerHeal    -= Deactivate;
    }


    void Activate(float none)
    {
        if(_playerRef == null)
            _playerRef = UnitManager.Instance.GetPlayer().GetComponent<Player>();

        if(_playerRef.healthRatio <= 0.25f)
        {
            if(!_isVignetteActive)
                StartCoroutine("ActivateVignette");
        }
    }

    IEnumerator ActivateVignette()
    {
        _isVignetteActive = true;

        float time = 0f;
        float duration = 1f;

        while(time < duration)
        {
            time += Time.deltaTime;
            _lowHealthVignette.alpha = time;

            yield return null;
        }

        _lowHealthVignette.alpha = 1f;
    }


    void Disable()
    {
        if(_isVignetteActive)
            StartCoroutine("DeactivateVignette");
    }

    void Deactivate(float none)
    {
        if(_isVignetteActive)
            StartCoroutine("DeactivateVignette");
    }

    IEnumerator DeactivateVignette()
    {
        _isVignetteActive = false;

        float time = 1f;

        while(time > 0)
        {
            time -= Time.deltaTime;
            _lowHealthVignette.alpha = time;

            yield return null;
        }

        _lowHealthVignette.alpha = 0f;
    }
}


}