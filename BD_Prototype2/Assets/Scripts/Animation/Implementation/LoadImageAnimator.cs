using System.Collections;
using UnityEngine;

namespace BulletDance.Animation
{

public class LoadImageAnimator : RhythmAnimator
{
    private enum ImageType { Player = 0, Halftone = 1, Logo = 2 }
    [SerializeField]
    private ImageType loadImageType;

    void Start()
    {
        _animator.SetFloat("ImageType", (float)((int)loadImageType));
    }

    public override void PlayAnimation(int anticipation, float duration)
    {
        _beatDuration = duration;
        _duration = GetDuration(_beatDuration, _animNoteDuration);

        //Anticipation = beats in the future
        //This default method will play animations in the current beat (anticipation = 0)
        if(anticipation != 0) return;

        if(_animator == null) return;

        _animator.speed = 1f / _duration;
        _animator.Play("Enter", -1, 0f);
    }
}

}
