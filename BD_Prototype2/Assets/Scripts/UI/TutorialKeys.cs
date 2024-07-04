using System;
using UnityEngine;
using BulletDance.Animation;

public class TutorialKeys : RhythmAnimator
{
    private enum TutorialGuide { Move, Attack, Dash, Aim };

    [SerializeField]
    private TutorialGuide _guide;

    protected override void OnEnable()
    {
        base.OnEnable();

        EventManager.Instance.OnKeyBoardAndMouseUsed += ChangeTutorialImage;
        EventManager.Instance.OnGamePadUsed += ChangeTutorialImage;

        _animationName = _guide.ToString();
        ChangeTutorialImage();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.Instance.OnKeyBoardAndMouseUsed -= ChangeTutorialImage;
        EventManager.Instance.OnGamePadUsed -= ChangeTutorialImage;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventManager.Instance.OnKeyBoardAndMouseUsed -= ChangeTutorialImage;
        EventManager.Instance.OnGamePadUsed -= ChangeTutorialImage;
    }


    private void ChangeTutorialImage()
    {
        _animator.SetFloat("Input", 
            InputManager.Instance.CurrentController == ControllerType.KEYBOARDANDMOUSE ?
                0f : 1f );
    }
}
