using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private bool _triggeredByButton;
    [SerializeField] private bool _startOpen;
    private bool _doorOpen;
    [SerializeField] private bool _startLocked;
    private bool _doorLocked;

    [SerializeField]
    private Animator _animator;

    private GameObject _collider;

    public void Start()
    {
        _collider = GetComponentInChildren<BoxCollider2D>().gameObject;
        _collider.SetActive(false);

        //Ignore the automatic opening and closing
        if (_triggeredByButton)
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
        }

        if (_startLocked)
        {
            LockDoor();
            CloseDoor();
        }
        else if (_startOpen)
        {
            OpenDoor();
        }
    }

    //OPEN CLOSE
    public void OpenDoor()
    {
        if (!_doorOpen && !_doorLocked)
        {
            _animator.SetTrigger("Open");
            _doorOpen = true;
            EventManager.Instance.PlaySFX("Gate Open", 5f); //should be an event, no?
        }
    }

    public void CloseDoor()
    {
        if (_doorOpen)
        {
            _animator.SetTrigger("Close");
            _doorOpen = false;
            EventManager.Instance.PlaySFX("Gate Close", 5f); //should be an event, no?
        }
    }


    //LOCK
    public void LockDoor()
    {
        if (!_doorLocked)
        {
            _doorLocked = true;
            _collider.SetActive(true);
            //playsfx?
        }
    }

    public void UnlockDoor()
    {
        if (_doorLocked)
        {
            _doorLocked = false;
            _collider.SetActive(false);
        }
    }



    //ON BUTTON PRESS
    public void ButtonActivate()
    {
        if (!_triggeredByButton) { return; }

        UnlockDoor();
        OpenDoor();
    }

    public void ButtonDeactivate()
    {
        if (!_triggeredByButton) { return; }

        LockDoor();
        CloseDoor();
    }



    //AUTOMATIC OPEN
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "PlayerFeet")
        {
            OpenDoor();                
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player"|| collider.gameObject.tag == "PlayerFeet")
        {
            CloseDoor();
        }
    }


}
