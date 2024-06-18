using System.Collections;
using UnityEngine;

namespace BulletDance.Animation
{

public class RhythmAnimator : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        //Instance is assigned on awake but awake is delayed for 2 frames bc reasons
        //So just keep trying to add into the thing :D
        StartCoroutine(TryAddToRhythmAnimationManager());
    }

    IEnumerator TryAddToRhythmAnimationManager()
    {
        while(RhythmAnimationManager.Instance == null)
        {
            yield return null;
        }

        RhythmAnimationManager.Instance?.AddToController(this);
    }

    protected virtual void OnDisable()
    {
        RhythmAnimationManager.Instance?.RemoveFromController(this);
    }

    protected virtual void OnDestroy()
    {
        RhythmAnimationManager.Instance?.RemoveFromController(this);
    }


    public enum AnimatorType { Unity, Sprite, Layered }
    [SerializeField] protected AnimatorType _animType = AnimatorType.Unity;
    [SerializeField] protected UnityEngine.Animator  _animator;
    [SerializeField] protected SpriteAnimator        _spriteAnimator;
    [SerializeField] protected LayeredSpriteAnimator _layeredAnimator;
    [SerializeField] protected string _animationName;


    protected enum NoteDuration { Whole, Half, Quarter, Eighth, Sixteenth }
    [SerializeField] 
    protected NoteDuration _animNoteDuration = NoteDuration.Eighth;
    [SerializeField]
    protected float _minAnimDuration = 0.08f;
    protected float _duration, _beatDuration;
    protected float GetDuration(float duration, NoteDuration noteDuration)
    {
        switch(noteDuration)
        {
            case NoteDuration.Whole    : return duration*8;
            case NoteDuration.Half     : return duration*4;
            case NoteDuration.Quarter  : return duration*2;
            case NoteDuration.Eighth   : return duration*1;
            case NoteDuration.Sixteenth: return duration*0.5f;
            default                    : return duration*1;
        }
    }

    protected float _offbeatDuration
    {
        get
        {
            //OffTime is the offset between the current time and the closest beat
            double offTime = PlayerRhythm.Instance.GetHitDelaySwing();

            if(offTime > 100) return 9999f; //There are no beats rn

            //OffTime > 0 means the current time is after the closest beat  => animation duration is the beat time there is left
            if(offTime > 0)
            {
                if(MusicManager.Instance.secondsPerBeat > offTime)
                    return (float)(MusicManager.Instance.secondsPerBeat - offTime);
                
                //Uhh idk how offTime > beatDuration tbh
                else
                    return 9999f;
            }

            //OffTime < 0 means the current time is before the closest beat => animation duration is offTime
            //Making a minimum duration check because it's not worth having an animation at like 10x speed, y'know
            else if(Mathf.Abs((float)offTime) > _minAnimDuration)
                return Mathf.Abs((float)offTime);

            else return 9999f;
        }
    }


    public virtual void PlayAnimation(int anticipation, float duration)
    {
        _beatDuration = duration;
        _duration = GetDuration(_beatDuration, _animNoteDuration);

        //Anticipation = beats in the future
        //This default method will play animations in the current beat (anticipation = 0)
        if(anticipation != 0) return;

        switch(_animType)
        {
            case AnimatorType.Unity:
                if(_animator == null) return;

                _animator.speed = 1f / _duration;
                _animator.Play(_animationName);

                break;

            case AnimatorType.Sprite:
                if(_spriteAnimator == null) return;

                _spriteAnimator.SetSpeed(1f / _duration);
                _spriteAnimator.Anim(_animationName);

                break;

            case AnimatorType.Layered:
                if(_layeredAnimator == null) return;

                _layeredAnimator.SetSpeed(1f / _duration);
                _layeredAnimator.Anim(_animationName);

                break;

            default: break;
        }
    }
}

}
