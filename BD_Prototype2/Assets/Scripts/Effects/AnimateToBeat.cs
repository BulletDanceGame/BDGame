using UnityEngine;

public class AnimateToBeat : MonoBehaviour
{

    [SerializeField] private Animator _animator;
    [SerializeField] private string _animationName;


    private void Start()
    {
        EventManager.Instance.OnBeatForVisuals += VisualsOnBeat;
    }

    private void OnEnable()
    {
        if (EventManager.Instance)
        {
            EventManager.Instance.OnBeatForVisuals += VisualsOnBeat;
        }

    }

    private void OnDisable()
    {
        EventManager.Instance.OnBeatForVisuals -= VisualsOnBeat;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnBeatForVisuals -= VisualsOnBeat;
    }

    private void VisualsOnBeat(int anticipation, float duration, int beat)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (anticipation == 0)
        {
            _animator.speed = 1f / (duration*2); //times 2 cause it doesnt care about 8th notes
            _animator.Play(_animationName, -1, 0);

        }
    }
}
