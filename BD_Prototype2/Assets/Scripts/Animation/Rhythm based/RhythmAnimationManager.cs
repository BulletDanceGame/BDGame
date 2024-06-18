using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Animation
{

//This handles the animators
public class RhythmAnimationManager : MonoBehaviour
{
    public static RhythmAnimationManager Instance { get; private set; }
    private void Awake() { if(Instance == null) Instance = this; }

    private List<RhythmAnimator> _animators = new List<RhythmAnimator>();
    public void AddToController(RhythmAnimator animator)
    {
        _animators.Add(animator);
    }

    public void RemoveFromController(RhythmAnimator animator)
    {
        _animators.Remove(animator);
    }


    private void Start()
    {
        EventManager.Instance.OnBeatForVisuals += UpdateAnimations;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnBeatForVisuals -= UpdateAnimations;
    }

    private void UpdateAnimations(int anticipation, float duration, int beat)
    {
        foreach(var animator in _animators)
        {
            animator.PlayAnimation(anticipation, duration);
        }
    }
    
}

}
