using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionLogger : MonoBehaviour
{

    public static float ScoreDelay = 3.0f;

    public int score = 0;

    [Header("Score")]
    public int DashScore = 0, HitScore = 0, BuntScore = 0, DamageScore = 0, RehitScore = 0, RehitWallBounceScore = 0, WallBounceScore = 0, ReHitWallBounceDamageScore = 0;

    enum Actions { DASH,HIT,BUNT,DAMAGE,REHIT,REHITWALLBOUNCE, WALLBOUNCE, REHITWALLBOUNCEDAMAGE}
    List<Actions> actionsMade = new List<Actions>();

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.OnDash += Dash;
        EventManager.Instance.OnHit += Hit;
        EventManager.Instance.OnWallBounce += WallBounce;
        EventManager.Instance.OnReHit += ReHit;
        EventManager.Instance.OnRehitWallBounce += ReHitWallBounce;
        EventManager.Instance.OnRehitWallBounceDamage += ReHitWallBounceDamage;
        EventManager.Instance.OnDamage += Damage;
    }

    void Dash()
    {
        AddAction(Actions.DASH);
    }

    void Hit()
    {
        AddAction(Actions.HIT);
    }

    void Bunt()
    {
        AddAction(Actions.BUNT);
    }

    void WallBounce()
    {
        AddAction(Actions.WALLBOUNCE);
    }

    void ReHit()
    {
        AddAction(Actions.REHIT);
    }

    void ReHitWallBounce()
    {
        AddAction(Actions.REHITWALLBOUNCE);
    }

    void ReHitWallBounceDamage()
    {
        AddAction(Actions.REHITWALLBOUNCEDAMAGE);
    }

    void Damage()
    {
        AddAction(Actions.DAMAGE);
    }

    Coroutine actionRoutine;

    void AddAction(Actions action)
    {
        if (actionRoutine != null)
        {
            StopCoroutine(actionRoutine);
            actionRoutine = null;
        }
        actionRoutine = StartCoroutine(ResetActions());

        actionsMade.Add(action);
    }

    IEnumerator ResetActions()
    {
        yield return new WaitForSeconds(ScoreDelay);

        foreach (Actions action in actionsMade)
        {
            switch (action)
            {
                case Actions.DAMAGE:
                    score += DamageScore;
                    break;
                case Actions.WALLBOUNCE:
                    score += WallBounceScore;
                    break;
                case Actions.REHIT:
                    score += RehitScore;
                    break;
                case Actions.REHITWALLBOUNCE:
                    score += RehitWallBounceScore;
                    break;
                case Actions.DASH:
                    score += DashScore;
                    break;
                case Actions.BUNT:
                    score += BuntScore;
                    break;
                case Actions.HIT: 
                    score += HitScore;
                    break;
                case Actions.REHITWALLBOUNCEDAMAGE: 
                    score += ReHitWallBounceDamageScore;
                    break;
            }
        }

        EventManager.Instance.AddScore(score);

        print("Added score: " + score);

        score = 0;
        actionsMade.Clear();
    }
}
