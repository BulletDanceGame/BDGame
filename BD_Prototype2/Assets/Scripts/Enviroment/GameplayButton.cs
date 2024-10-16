using System.Collections;
using UnityEngine;

public class GameplayButton : MonoBehaviour 
{
    [Header("Dumbass Directional Sfx")]
    public AK.Wwise.Event walkButtonSFX;
    public AK.Wwise.Event deflectButtonSFX;

    [SerializeField] private bool _triggeredByOtherButton;
    [SerializeField] private bool _startPressable;
    private bool _isPressable;
    //[SerializeField] private SpriteRenderer _buttonRenderer;

    public enum TriggerType { Walkable, Deflection}
    [SerializeField] private TriggerType type;


    public enum AfterPress { Disabled, Reactivate, Toggle }
    [SerializeField] private AfterPress _afterPress;
    private bool _isToggled;

    [Space]
    [SerializeField] private MonoBehaviour[] _activatedObjects;

    [Space]
    [SerializeField] private MonoBehaviour[] _deactivatedObjects;

    private void Start()
    {
        _isPressable = _startPressable;
        //_buttonRenderer.color = (_isPressable) ? Color.red : Color.gray;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == TriggerType.Walkable)
        {
            if (collision.tag == "Player")
            {
                StartPressWalk();
            }
        }
        else if (type == TriggerType.Deflection)
        {
            if (!collision.GetComponent<Bullet>()) return;
            if (collision.GetComponent<Bullet>().type == BulletOwner.PLAYERBULLET)
            {
                StartPressDeflect();
            }
        }
        
    }


    private void StartPressWalk()
    {
        if (!_isPressable) return;

        //walkButtonSFX.Post(gameObject);
        GetComponentInChildren<Animator>().Play("Activate");
        _isPressable = false;

        StartCoroutine(ButtonPress());
    }

    private void StartPressDeflect()
    {
        if (!_isPressable) return;

        deflectButtonSFX.Post(gameObject);
        GetComponentInChildren<Animator>().Play("Activate");
        _isPressable = false;

        StartCoroutine(ButtonPress());
    }

    private void Effect()
    {
        MonoBehaviour[] toActivate = (!_isToggled) ? _activatedObjects : _deactivatedObjects;
        MonoBehaviour[] toDeactivate = (_isToggled) ? _activatedObjects : _deactivatedObjects;

        foreach (MonoBehaviour o in toActivate)
        {
            o.Invoke("ButtonActivate", 0);
        }

        foreach (MonoBehaviour o in toDeactivate)
        {
            o.Invoke("ButtonDeactivate", 0);
        }
    }

    IEnumerator ButtonPress()
    {
        yield return new WaitForSeconds(0.5f);
        Effect();

        if (_afterPress == AfterPress.Toggle || _afterPress == AfterPress.Reactivate)
        {
            yield return new WaitForSeconds(0.5f);

            GetComponentInChildren<Animator>().Play("Deactivate");
            yield return new WaitForSeconds(0.5f);
            _isPressable = true;

            if (_afterPress == AfterPress.Toggle)
            {
                _isToggled = !_isToggled;
            }
        }
    }


    //ON OTHER BUTTON PRESS
    public void ButtonActivate()
    {
        if (!_triggeredByOtherButton) { return; }

        _isPressable = true;
        //_buttonRenderer.color = Color.red;

    }

    public void ButtonDeactivate()
    {
        if (!_triggeredByOtherButton) { return; }

        _isPressable = false;
        //_buttonRenderer.color = Color.gray;
    }


}
