using System.Collections.Generic;
using UnityEngine;
using BulletDance.Animation;

namespace BulletDance.VFX
{

public class PlayerPerfectVFX : MonoBehaviour
{
    private static readonly Dictionary<Direction, Vector3> DirectionAngleLookup = new Dictionary<Direction, Vector3>
    {
        {Direction.Front, Vector3.zero},
        {Direction.Back, new Vector3(0, 0, 180f) },
        {Direction.Left, new Vector3(0, 0, -90f) },
        {Direction.Right, new Vector3(0, 0, 90f) }
    };

    [SerializeField]
    private UnityEngine.Animator _animator;
    [SerializeField]
    private SpriteRenderer _atkAfterImgRdr;
    [SerializeField]
    private Transform _splatterDir;

    public void AttackAfterImage(SpriteRenderer _spRdr, Direction _attackDir)
    {
        _animator.SetTrigger("Perfect");
        _atkAfterImgRdr.sprite     = _spRdr.sprite;
        _splatterDir.eulerAngles   = DirectionAngleLookup[_attackDir];
        _splatterDir.localPosition = Vector3.zero;
    }


    float _timer = 0f;
    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > 5f)
            Destroy(gameObject);
    }
}

}