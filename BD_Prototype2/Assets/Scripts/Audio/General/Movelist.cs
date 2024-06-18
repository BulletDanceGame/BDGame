using UnityEngine;

public class Movelist : MonoBehaviour
{
    [Header("Movelist Specifics")]
    [SerializeField] protected bool _isActiveOnStart;
    protected bool _isActive;


    public virtual void Start()
    {
        if (_isActiveOnStart)
        {
            Activate();
        }
    }


    public virtual void Action(Note action)
    {

    }

    public virtual void Activate()
    {
        _isActive = true;
    }

    public virtual void Deactivate()
    {
        _isActive = false;
    }
}
