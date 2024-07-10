using UnityEngine;

public class Container : MonoBehaviour
{

    private bool _activated;
    private int _beatsUntilDestroyed = 0;

    private bool hit;
    private bool canDamage;
    private float collisionTimer;

    [SerializeField] private GameObject bulletTriggerBox;
    [SerializeField] private BulletDance.Animation.CannisterAnimator _animator;

    void Start()
    {
        SetUp();
    }

    private void SetUp()
    {
        if (_activated) return;
        _activated = true;

        _beatsUntilDestroyed = MusicManager.Instance._currentSequence.duration;

        GetComponent<CircleCollider2D>().enabled = false;

        if (EventManager.Instance)
        {
            EventManager.Instance.OnDeactivateBoss += Deactivate;
            EventManager.Instance.OnBeat += OnBeat;
        }

    }

    private void Update()
    {        
        if (canDamage && collisionTimer > 0 && !hit)
        {
            collisionTimer -= Time.deltaTime;
            if (collisionTimer <= 0)
            {
                GetComponent<CircleCollider2D>().isTrigger = false;
                canDamage = false;
                bulletTriggerBox.SetActive(true);
                print("Cannister Collide");

            }
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


    public void Landed()
    {
        canDamage = true;
        GetComponent<CircleCollider2D>().enabled = true;
        collisionTimer = 0.5f;
        print("Cannister Land");

        EventManager.Instance.PlaySFX("Cannister Land", 10f);
    }



    public void StartSmokeVFX()
    {
        _animator.StartSmokeVFX();
    }


    public void OverrideValues(GameObject g)
    {
        _animator.SetUp(BulletDance.Animation.CannisterAnimator.CanType.FIRE);

        SetUp();
        _beatsUntilDestroyed = 34 + 64;

        Vector3 bossPos = BossController.Instance.boss.transform.position;
        Vector3 spawnPos = new Vector3(
            (bossPos.x > 0) ? -10 : 10,
            (bossPos.y > 0) ? -10 : 10);
        transform.position = bossPos + spawnPos;

        g.transform.position = transform.position;

        transform.parent = null;
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            print(collision.gameObject.name);

            if (!hit && canDamage)
            {
                hit = true;
                EventManager.Instance.PlayerDamage(10);
                EventManager.Instance.PlayerPushBack(transform.position);
                ScoreManager.Instance.GotHit++;

                print("Cannister Hit");
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (hit)
            {
                print(collision.gameObject.name);
                GetComponent<CircleCollider2D>().isTrigger = false;
                canDamage = false;
                bulletTriggerBox.SetActive(true);
                print("Cannister Exit");
            }
        }


    }

}
