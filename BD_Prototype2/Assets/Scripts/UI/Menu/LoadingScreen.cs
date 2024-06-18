using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    void Start()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnSceneLoad   += Cover;
        EventManager.Instance.OnSceneLoaded += UnCover;
    }

    void OnDestroy()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnSceneLoad   -= Cover;
        EventManager.Instance.OnSceneLoaded -= UnCover;
    }

    void Cover()
    {
        InvertMask(0);
        anim.SetTrigger("Cover");
    }

    void UnCover()
    {
        InvertMask(1);
        anim.SetTrigger("UnCover");
    }

    void InvertMask(int value)
    {
        var rdr = GetComponentInChildren<UnityEngine.UI.Image>();
        rdr.material.SetInt("_InvertMask", value);
    }
}