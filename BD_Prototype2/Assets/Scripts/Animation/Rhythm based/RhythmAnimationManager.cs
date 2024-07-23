using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Animation
{

    //This handles the animators
    public class RhythmAnimationManager : MonoBehaviour
    {
        public static RhythmAnimationManager Instance { get; private set; }
        private void Awake() { if(Instance == null) Instance = this; }

        //Need separate lists so update enumarator don't stop from unexpected add/remove
        private List<RhythmAnimator> _animators = new List<RhythmAnimator>();
        private List<RhythmAnimator> _animatorsAdded   = new List<RhythmAnimator>();
        private List<RhythmAnimator> _animatorsRemoved = new List<RhythmAnimator>();
        public void AddToController(RhythmAnimator animator)
        {
            _animatorsAdded.Add(animator);
        }

        public void RemoveFromController(RhythmAnimator animator)
        {
            _animatorsRemoved.Remove(animator);
        }


        private void Start()
        {
            EventManager.Instance.OnBeat += UpdateAnimations;
        }

        private void OnDestroy()
        {
            EventManager.Instance.OnBeat -= UpdateAnimations;
        }

        private void UpdateAnimations(int beat)
        {
            //Add to updates & flush queue
            if(_animatorsAdded.Count > 0)
            {
                foreach(var animator in _animatorsAdded)
                {
                    _animators.Add(animator);
                }
                _animatorsAdded.Clear();
            }


            float duration = (float)MusicManager.Instance.secondsPerBeat;

            //Update
            foreach(var animator in _animators)
            {
                if (!animator) //animators that had been deleted was still called and caused errors
                {
                    _animatorsRemoved.Add(animator);
                    continue;
                }

                animator.PlayAnimation(0, duration);
            }


            //Remove from update & flush queue
            if(_animatorsRemoved.Count > 0)
            {
                foreach(var animator in _animatorsRemoved)
                {
                    _animators.Remove(animator);
                }
                _animatorsRemoved.Clear();
            }
        }
    
    }

}
