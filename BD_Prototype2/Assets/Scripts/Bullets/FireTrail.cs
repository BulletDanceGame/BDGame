using UnityEngine;

public class FireTrail : MonoBehaviour
{
    public float duration;
    public bool isInfinite=false;

    private void OnEnable()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer!=null)spriteRenderer.enabled = false;
        if(isInfinite)
        {
            duration=Mathf.Infinity;
            //Invoke("OnActive", duration);
        }
        Invoke("OnActive",duration);
    }

   
    void OnActive()
    {
        FireBag.Instance.AddToPool(gameObject);
    }
}
