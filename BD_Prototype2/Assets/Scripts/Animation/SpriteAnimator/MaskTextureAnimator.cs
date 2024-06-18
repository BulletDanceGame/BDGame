using UnityEngine;
using UnityEngine.U2D.Animation;

using BulletDance.Graphics;


/****
    Seperate class containing animation methods, so that
    - Artists have a seperate workspace than programmers
    - Artists only lose direct implementations (when the animation triggers) in case merging goes wrong
    - More legible implementation names than animator.SetTrigger etc.
***/

namespace BulletDance.Animation
{

/// <summary>
/// Use to shift Mask's UV
/// </summary>
public class MaskTextureAnimator : MonoBehaviour
{
    [SerializeField]
    private SpriteLibraryGroupAsset _spriteGroupAsset;

    [SerializeField]
    private SpriteRenderer _affectedRenderer;
    private Material _mat;

    [SerializeField]
    private SpriteResolver _affectedResolver, _referencedResolver;

    [SerializeField]
    private UnityEngine.Animator _affectedAnimator, _referencedAnimator;

    private float _refAnimState, _affectedAnimState, _refFrame, _affectedFrame;
    private float _offset, _xOffsetInFrames;


    bool debugToggle = false;

    void Start()
    {
        // -- Getting references -- //
        _mat = _affectedRenderer.material;

        if(_mat == null || _spriteGroupAsset == null ||
           _referencedResolver == null || _affectedResolver == null ||
           _referencedAnimator == null || _affectedAnimator == null)
        {
            this.enabled = false;
            return;
        }

        _spriteGroupAsset.CreateAnimationInfoLookup();
    }


    void Update()
    {
        //Get animation State
        _refAnimState      = (float)_referencedAnimator.GetInteger("State");
        _affectedAnimState = (float)_affectedAnimator.GetInteger("State");


        //Set the mask for the Dash anim, otherwise disable mask 
        if(_refAnimState == 2 && _affectedAnimState == 2) 
            _mat.SetFloat("_MaskXOffset", 0);
        else
            _mat.SetFloat("_MaskXOffset", 1);
    }


    void TestTheory()
    {
        //Take advantage of sprite library entry naming convention being frame numbers
        _refFrame      = float.Parse(_referencedResolver.GetLabel());
        _affectedFrame = float.Parse(_affectedResolver.GetLabel()); 

        //Get animation State
        _refAnimState      = (float)_referencedAnimator.GetInteger("State");
        _affectedAnimState = (float)_affectedAnimator.GetInteger("State");

        _xOffsetInFrames = _spriteGroupAsset.GetAnimationInfo(_refAnimState).xOffsetInFrames - _spriteGroupAsset.GetAnimationInfo(_affectedAnimState).xOffsetInFrames;
        _offset          = (_refFrame - _affectedFrame + _xOffsetInFrames) / _spriteGroupAsset.spritesheetSize.x;
        _mat.SetFloat("_MaskXOffset", _offset);

        //test
        if(Input.GetKeyDown(KeyCode.I)) debugToggle = !debugToggle;

        if(debugToggle)
        {
            Debug.Log("Curent state + frame: " + _spriteGroupAsset.GetAnimationInfo(_affectedAnimState).name + " - " + _affectedFrame);
            Debug.Log("Reference state + frame: " + _spriteGroupAsset.GetAnimationInfo(_refAnimState).name + " - " + _refFrame);
            Debug.Log("Offset: " + _offset);
        }
    }
}

}