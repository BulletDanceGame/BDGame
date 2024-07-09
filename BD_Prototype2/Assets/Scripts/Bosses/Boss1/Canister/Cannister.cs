using BulletDance.Graphics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Cannister : MonoBehaviour
{
    [SerializeField]
    GameObject collisionBox;

    private bool ugh;
    private int _beatsUntilDestroyed = 0;

    [SerializeField]
    private BulletDance.Animation.CannisterAnimator _animator;


    // Start is called before the first frame update
    void Start()
    {
        SetUp();

    }


    //private void Update()
    //{
    //    if (QueueToActivate)
    //    {
    //        hitTimer -= Time.deltaTime;
    //        if (hitTimer <= 0)
    //        {
    //            ApplyCollision();
    //        }
    //        QueueToActivate = false;
    //    }

    //}
    //bool once = true;

    private void SetUp()
    {
        if (ugh) return;
        ugh = true;

        _beatsUntilDestroyed = MusicManager.Instance._currentSequence.duration;
        

        if (EventManager.Instance)
        {
            EventManager.Instance.PlaySFX("Cannister Land", 10f);
            EventManager.Instance.OnDeactivateBoss += Deactivate;
                EventManager.Instance.OnBeat += OnBeat;
            }

    }

    private void OnBeat(int b)
    {
        _beatsUntilDestroyed--;

        if (_beatsUntilDestroyed <= 0)
        {
            Deactivate();
            
        }
    }

    private void Deactivate()
    {
        _animator.Deactivate();

        if(EventManager.Instance != null)
        {
            EventManager.Instance.OnDeactivateBoss -= Deactivate;
            EventManager.Instance.OnBeat -= OnBeat;
        }

        Destroy(gameObject);
    }


    //float hitTimer;
    //bool QueueToActivate;

    public void Landed()
    {
        //if (Mathf.Abs((UnitManager.Instance.GetPlayer().transform.position - transform.position).magnitude) < GetComponentInChildren<CircleCollider2D>().radius)
        //{
        //    EventManager.Instance.PlayerDamage(10);
        //    EventManager.Instance.PlayerPushBack(transform.position);
        //    ScoreManager.Instance.GotHit++;


        //    //QueueToActivate = true;
        //    //hitTimer = 1;
        //    collisionBox.SetActive(true);
        //}
        //else
        //{
        //    collisionBox.SetActive(true);
        //}
    }



    public void StartSmokeVFX()
    {
        _animator.StartSmokeVFX();
    }

    public void OverrideValues(GameObject g)
    {
        _animator.SetTypeToFire();

        SetUp();
        _beatsUntilDestroyed = MusicManager.Instance._currentSequence.duration + 22;

        Vector3 bossPos = BossController.Instance.boss.transform.position;
        Vector3 spawnPos = new Vector3(
            (bossPos.x > 0) ? -10 : 10,
            (bossPos.y > 0) ? -10 : 10);
        transform.position = bossPos + spawnPos;

        g.transform.position = transform.position;
    }
}
