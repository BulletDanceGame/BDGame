using System.Collections;
using UnityEngine;

namespace BulletDance.Animation
{

public class HitboxVFX : RhythmAnimator
{
    protected override void OnEnable()
    {
        base.OnEnable();
        HideHitbox();

        EventManager.Instance.OnPlayerTired  += ShowHitbox;
        EventManager.Instance.OnPlayerNormal += HideHitbox;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        HideHitbox();

        EventManager.Instance.OnPlayerTired  -= ShowHitbox;
        EventManager.Instance.OnPlayerNormal -= HideHitbox;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        HideHitbox();

        EventManager.Instance.OnPlayerTired  -= ShowHitbox;
        EventManager.Instance.OnPlayerNormal -= HideHitbox;
    }


    public override void PlayAnimation(int anticipation, float duration)
    {
        if(!showHitbox) return;

        base.PlayAnimation(anticipation, duration);
    }

    private bool showHitbox;
    [SerializeField] private GameObject[] vfxs;
    [SerializeField] private SpriteRenderer sprite;

    void ShowHitbox()
    {
        showHitbox = true;
        foreach(var vfx in vfxs){ vfx.SetActive(true); }
        sprite.enabled = true;
    }

    void HideHitbox()
    {
        showHitbox = false;
        foreach(var vfx in vfxs){ vfx.SetActive(false); }
        sprite.enabled = false;
    }
}

}
