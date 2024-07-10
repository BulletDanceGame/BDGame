using System.Collections.Generic;
using UnityEngine;
using BulletDance.Animation;

namespace BulletDance.VFX
{

public class PlayerPerfectVFX : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.Animator _animator;
    [SerializeField]
    private SpriteRenderer _atkAfterImgRdr, _hitSplashRdr, _smallSplashRdr, _bgSplashRdr;
    [SerializeField]
    private Transform _splatterDir;

    public void AttackAfterImage(Sprite sprite, Vector2 position)
    {
        _animator.SetTrigger("Perfect");
        _atkAfterImgRdr.sprite     = sprite;
        _splatterDir.eulerAngles   = new Vector3(0f, 0f, Vector2.SignedAngle(position, Vector2.up));
        _splatterDir.localPosition = Vector3.zero;

        RandomizeSplash(_hitSplashRdr);
        RandomizeSplash(_smallSplashRdr);
        RandomizeSplash(_bgSplashRdr);

        Color color = _baseColor[Random.Range(0, _baseColor.Count)];
        SetSplashColor(_atkAfterImgRdr, color, 0f);
        SetSplashColor(_hitSplashRdr, color, 0.9f);
        SetSplashColor(_smallSplashRdr, color, 0.9f);
        SetSplashColor(_bgSplashRdr, color, 0.5f);
    }

    [SerializeField]
    private Vector2 _positionRandomRange;
    void RandomizeSplash(SpriteRenderer spRdr)
    {
        spRdr.transform.localPosition += new Vector3(Random.Range(_positionRandomRange.x, _positionRandomRange.y), 
                                                Random.Range(_positionRandomRange.x, _positionRandomRange.y), 0f);
        spRdr.transform.localEulerAngles += new Vector3(0f, 0f, Random.Range(-180f, 180f));
    }

    [SerializeField]
    private List<Color> _baseColor;
    void SetSplashColor(SpriteRenderer spRdr, Color color, float hue)
    {
        spRdr.material.SetColor("_BaseColor", color);
        spRdr.material.SetFloat("_Hue", hue);
    }

    float _timer = 0f;
    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > 5f)
            Destroy(gameObject);
    }
}

}