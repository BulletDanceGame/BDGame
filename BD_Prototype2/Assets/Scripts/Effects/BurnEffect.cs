using UnityEngine;

public class BurnEffect : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem;

    void Start()
    {
        EventManager.Instance.OnPlayerBurn += PlayerBurnFX;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnPlayerBurn -= PlayerBurnFX;
    }

    private void PlayerBurnFX(bool isBurning)
    {
        if(isBurning)
            _particleSystem.Play();
        else
            _particleSystem.Stop();
    }


}
