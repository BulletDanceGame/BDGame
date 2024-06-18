using BulletDance.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nhi
{

public class CannisterGraphicsTest : MonoBehaviour
{
    [SerializeField]
    private CannisterAnimator _animator;

    [SerializeField]
    private CannisterAnimator.CanType _cannisterType;

    IEnumerator Start()
    {
        yield return null;
        yield return null;
        _animator.SetColor(_cannisterType);
    }

}


}