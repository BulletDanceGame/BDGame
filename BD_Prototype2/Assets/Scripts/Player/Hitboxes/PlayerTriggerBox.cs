using UnityEngine;

public class PlayerTriggerBox : MonoBehaviour
{
    [SerializeField] private HurtEffect _hurtEffect;
    [SerializeField] private float _burnBeat;

    private bool _isImmune = false, _isBurning;
    private float _burnTracker;


    private void Start()
    {
        EventManager.Instance.OnBeat += BurnDamage;
        EventManager.Instance.OnPlayerDamage += NormalHurtFeedback;
        EventManager.Instance.OnCutsceneStart += ImmunityStart;
        EventManager.Instance.OnCutsceneEnd   += ImmunityEnd;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnBeat -= BurnDamage;
        EventManager.Instance.OnPlayerDamage -= NormalHurtFeedback;
        EventManager.Instance.OnCutsceneStart -= ImmunityStart;
        EventManager.Instance.OnCutsceneEnd   -= ImmunityEnd;
    }


    void ImmunityStart(string none)
    {
        _isImmune = true;
    }

    void ImmunityEnd()
    {
        _isImmune = false;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isImmune) return;

        if (collision.gameObject.tag == "Bullet")
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            EventManager.Instance.PlayerDamage(bullet.GetDamage());
            bullet.Deactivate();
            ScoreManager.Instance.GotHit++;
        }
        else if (collision.GetComponent<FireTrail>())
        {            
            _isBurning = true;
            _burnTracker = 0;
        }
    }


    void BurnDamage(int beat)
    {
        if (_isImmune) return;


        if (beat % 2 != 0)
            return;

        if (_isBurning)
        {
            if (Player.currentHealth >= 2)
            {
                EventManager.Instance.PlayerDamage(1);
                //GetComponentInParent<Player>().TakeDamage(1);
            }


            _burnTracker++;
        }

        if (_burnTracker>= _burnBeat) 
            _isBurning = false;

        EventManager.Instance.PlayerBurn(_isBurning);
    }


    public void NormalHurtFeedback(float damage)
    {
        if (_isImmune) return;

        if(damage <= 1) return;
        if(Player.currentHealth <= 0) return;

        _hurtEffect.HurtForPLayer();
    }

}