using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BulletDance.VFX
{

[ExecuteInEditMode]
public class FightVFX : MonoBehaviour
{
    /* Editor shit */

    private TextMeshProUGUI _text;
    private Material _material;
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _material = _text.fontSharedMaterial;
    }

    private void OnValidate()
    {
        UpdateMaterial();
    }

    public float _bGlow = 0.65f, _rDist = 0.025f;

    private void UpdateMaterial()
    {
        _material.SetFloat("_BGlow", _bGlow);
        _material.SetFloat("_RDist", _rDist);
    }

    private void Update()
    {
        UpdateMaterial();
    }


    /* Animation trigger */
    void OnEnable()
    {
        if(EventManager.Instance)
            EventManager.Instance.OnRoomStart += AnimateVFX;
    }

    void OnDisable()
    {
        if(EventManager.Instance)
            EventManager.Instance.OnRoomStart -= AnimateVFX;
    }

    [SerializeField] private UnityEngine.Animation _animator;
    void AnimateVFX(RoomController.RoomType roomType, bool switchMusic)
    {
        if(roomType != RoomController.RoomType.KillRoom) return;
        if(!switchMusic) return;

        _animator.Play();
    }
}

}