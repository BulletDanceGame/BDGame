using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }

    [SerializeField]
    private Animator anim;
    [SerializeField] private Image img;
    [SerializeField] private Material inverted, notInverted;
    [SerializeField] private Canvas canvas;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;

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
        img.material = value == 0 ? notInverted : inverted;
    }

    public System.Action<int> ShowOnUncover;
    public void ShowWhenUncover(int frame)
    {
        ShowOnUncover?.Invoke(frame);
    }
}