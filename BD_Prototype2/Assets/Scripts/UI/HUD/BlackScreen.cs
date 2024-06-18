using System.Collections;
using UnityEngine;


namespace BulletDance.Misc
{

public class BlackScreen : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvas;

    [SerializeField]
    private UnityEngine.Animation _anim;    

    public void FadeOut()
    {
        _anim.Play();
    }

    public void SetAlpha(float alpha)
    {
        _canvas.alpha = alpha;
    }
}

}