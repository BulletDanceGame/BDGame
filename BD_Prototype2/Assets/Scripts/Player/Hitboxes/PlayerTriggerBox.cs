using UnityEngine;

public class PlayerTriggerBox : MonoBehaviour
{
    [SerializeField] private HurtEffect _hurtEffect;
    [SerializeField] private float _burnBeat;

    private bool _isImmune = false, _isBurning;
    private float _burnTracker;

    private bool _blackFire=false;

    private Player player;

    private void Start()
    {
        player = UnitManager.Instance?.GetPlayer()?.GetComponent<Player>();

        EventManager.Instance.OnBeat += BurnDamage;
        EventManager.Instance.OnPlayerDamage += NormalHurtFeedback;
        EventManager.Instance.OnPlayerLastHit += ImmuneStart;
        EventManager.Instance.OnCutsceneStart += ImmunityStart;
        EventManager.Instance.OnCutsceneEnd   += ImmunityEnd;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnBeat -= BurnDamage;
        EventManager.Instance.OnPlayerDamage -= NormalHurtFeedback;
        EventManager.Instance.OnPlayerLastHit -= ImmuneStart;
        EventManager.Instance.OnCutsceneStart -= ImmunityStart;
        EventManager.Instance.OnCutsceneEnd   -= ImmunityEnd;
    }

    void ImmuneStart(BeatTiming beatTiming)
    {
        if(beatTiming != BeatTiming.BAD)
            _isImmune = true;
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

        if(player == null)
            player = UnitManager.Instance?.GetPlayer()?.GetComponent<Player>();

        if (collision.gameObject.tag == "Bullet")
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            EventManager.Instance.PlayerDamage(bullet.GetDamage());
            bullet.Deactivate();
            ScoreManager.Instance.GotHit++;
        }
        else if (collision.gameObject.tag=="FireTrail")
        {
            _blackFire = false;
            _isBurning = true;
            _burnTracker = 0;
        }
        else if(collision.gameObject.tag == "BlackFireTrail")
        {
            _blackFire = true;
            _isBurning = true;
            _burnTracker = 0;
        }
    }


    void BurnDamage(int beat)
    {
        if (_isImmune)  return;
        if(!_isBurning) return;

        if (beat % 2 != 0)
            return;

        if (!_blackFire)
        {
            if (player.currentHealth >= 2)
                EventManager.Instance.PlayerDamage(1);
        }
        else if(_blackFire)
            EventManager.Instance.PlayerDamage(5);

        _burnTracker++;

        if (_burnTracker >= _burnBeat)
        {
            _blackFire = false;
            _isBurning = false;
        }

        EventManager.Instance.PlayerBurn(_isBurning);
    }


    public void NormalHurtFeedback(float damage)
    {
        if (_isImmune) return;

        if(damage <= 1) return;
        if(player.isDead) return;

        _hurtEffect.HurtForPLayer();
    }

}