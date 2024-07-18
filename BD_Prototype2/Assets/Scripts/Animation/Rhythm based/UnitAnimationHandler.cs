using System;
using UnityEngine;

namespace BulletDance.Animation
{

public class UnitAnimationHandler : MonoBehaviour
{
    public event Action OnAlerted;

    public void Alerted()
    {
        OnAlerted?.Invoke();
    }


    public event Action OnWalkStart;
    public event Action OnWalkStop;

    public void WalkStart()
    {
        OnWalkStart?.Invoke();
    }

    public void WalkStop()
    {
        OnWalkStop?.Invoke();
    }


    public event Action<Vector2> OnDashStart;
    public event Action OnDashStop;

    public void DashStart(Vector2 dashDirection)
    {
        OnDashStart?.Invoke(dashDirection);
    }

    public void DashStop()
    {
        OnDashStop?.Invoke();
    }


    public event Action OnAttackStart;

    public void AttackStart()
    {
        OnAttackStart?.Invoke();
    }


    public event Action<int> OnPhaseChange;
    public event Action<int> OnPhaseChangeFinished;

    public void PhaseChangeStart(int phase)
    {
        OnPhaseChange?.Invoke(phase);
    }

    public void PhaseChangeFinished(int phase)
    {
        OnPhaseChangeFinished?.Invoke(phase);
    }


    public event Action OnHurt;
    public event Action OnDefeat;

    public void Hurt()
    {
        OnHurt?.Invoke();
    }

    public void Defeat()
    {
        OnDefeat?.Invoke();
    }

    public event Action <int> OnSpecialStart;
    public event Action OnSpecialStop;

    public void SpecialStart(int actionState)
    {
        OnSpecialStart?.Invoke(actionState);
    }

    public void SpecialStop()
    {
        OnSpecialStop?.Invoke();
    }


}

}
