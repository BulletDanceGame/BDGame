using System.Collections;
using UnityEngine;

namespace BulletDance.Animation
{

public class DitherAnimator : RhythmAnimator
{
    private Material _mat;

    protected override void OnEnable()
    {
        base.OnEnable();
        _mat = GetComponent<SpriteRenderer>()?.material;
    }


    private float _smallAnim = 80f, _bigAnim = 45f;
    public float smallAnimSpeed = 2f, bigAnimSpeed = 2f;

    void Update()
    {
        if(_mat == null) return;

        _bigAnim   += Time.deltaTime * bigAnimSpeed;
        _smallAnim += Time.deltaTime * smallAnimSpeed;

        _mat.SetFloat("_BWAnim", _bigAnim);
        _mat.SetFloat("_SWAnim", _smallAnim);
    }
}

}
