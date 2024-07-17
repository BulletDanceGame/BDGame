using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }

    [SerializeField]
    private Animator anim;
    [SerializeField] private Image img;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {

        InvertMask(1);

        if (EventManager.Instance == null) return;
        EventManager.Instance.OnSceneLoad   += Cover;
        EventManager.Instance.OnSceneLoaded += UnCover;

    }

    void OnDestroy()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnSceneLoad   -= Cover;
        EventManager.Instance.OnSceneLoaded -= UnCover;
    }

    public void Cover()
    {
        InvertMask(0);
        anim.SetTrigger("Cover");

        //also gets Inverted again from an Event in the Cover-animation
    }

    public void UnCover()
    {
        InvertMask(1);
        anim.SetTrigger("UnCover");
    }

    void InvertMask(int value)
    {
        img.material.SetInt("_InvertMask", value);
    }
}