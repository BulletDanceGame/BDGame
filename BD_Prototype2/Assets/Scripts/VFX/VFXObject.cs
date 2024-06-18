using UnityEngine;

namespace BulletDance.VFX
{

/*
    Base class for VFXObjects
    VFXObjects are managed by VFXManager for optimization purposes
    VFXObjects are for effects that can be recycled,
        so that you don't have to instantiate/spawn it multiple times
*/
public class VFXObject : MonoBehaviour
{
    [HideInInspector]
    public int id = 0;

    [SerializeField]
    protected float _duration = 1f;
    protected float _enableTime;


    protected virtual void Start()
    {
        _enableTime = _duration;
    }

    protected virtual void OnEnable()
    {
        _enableTime = _duration;
    }

    protected virtual void OnDisable()
    {
        Deactivate();
    }

    public virtual void UpdateSelf()
    {
        _enableTime -= Time.deltaTime;

        if(_enableTime < 0f)
            Deactivate();
    }

    protected virtual void Deactivate()
    {
        _enableTime = _duration;

        transform.parent = VFXManager.Instance.transform;
        VFXManager.Instance.RemoveFromUpdateList(this);
        gameObject.SetActive(false);
    }
}


}