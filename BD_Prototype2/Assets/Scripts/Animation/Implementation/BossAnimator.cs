using System.Collections.Generic;
using UnityEngine;

namespace BulletDance.Animation
{


/****
    Base Class for animating any boss
    this is so i can yoink generic boss-related cutscene functions
***/


public class BossAnimator : UnitAnimator
{
    // -- For cutscenes -- //
    // Excecute always for editor preview

    protected enum CutsceneAnimAction
    {
        Intro = 30, PhaseDefeat = 31, PhaseChange = 32
    }

    [ExecuteAlways]
    protected override void CutsceneAnimationLogic()
    {
        PhaseChange();
    }


    //Phase change
    [ExecuteAlways]
    protected int _state 
    { 
        get 
        {
            return _animType == AnimatorType.Layered ? 
                    _layeredAnimator.GetState() : _spriteAnimator.GetState();
        } 
    }
    protected int _prevState = 0;

    [ExecuteAlways]
    protected void PhaseChange()
    {
        if(_state == (int)CutsceneAnimAction.PhaseChange &&
           _prevState != (int)CutsceneAnimAction.PhaseChange)
            PhaseChangeStart(_phase);

        if(_state != (int)CutsceneAnimAction.PhaseChange && 
           _prevState == (int)CutsceneAnimAction.PhaseChange)
            //_phase instead of _phase + 1 bc the new phase is already set by SetPhaseGraphics
            PhaseChangeFinished(_phase); 

        _prevState = _state;
    }

    [ExecuteAlways]
    protected override void PhaseChangeStart(int phase){}

    [ExecuteAlways]
    protected override void PhaseChangeFinished(int phase)
    {
        _spriteAnimator?.SetFloat("Phase", phase);
        _layeredAnimator?.SetFloat("Phase", phase);
    }
}

}