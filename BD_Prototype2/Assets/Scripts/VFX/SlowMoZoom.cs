using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace BulletDance.VFX
{

[RequireComponent(typeof(Volume))]
public class SlowMoZoom : VFXObject
{
    // -- Set up -- //
    private Volume _volume;
    private LensDistortion _lensDistortion;
    private ChromaticAberration _chromAberration;

    private float _lensDistortIntensityDefault, _lensDistortScaleDefault, _chromAberrationDefault;
    private Vector2 _lensDistortCenterDefault;
    private Camera _camera;

    protected override void Start()
    {
        _volume = GetComponent<Volume>();
        _volume.profile.TryGet<LensDistortion>(out _lensDistortion);
        _volume.profile.TryGet<ChromaticAberration>(out _chromAberration);

        //Get defaults
        if(_lensDistortion)
        {
            _lensDistortCenterDefault    = _lensDistortion.center.value;
            _lensDistortIntensityDefault = _lensDistortion.intensity.value;
            _lensDistortScaleDefault     = _lensDistortion.scale.value;
        }

        if(_chromAberration)
            _chromAberrationDefault = _chromAberration.intensity.value;

        _camera = Camera.main;
    }

    void OnDestroy() {  Deactivate();  }

    protected override void Deactivate()
    {
        if(_lensDistortion)
        {
            _lensDistortion.center.value    = _lensDistortCenterDefault;
            _lensDistortion.intensity.value = _lensDistortIntensityDefault;
            _lensDistortion.scale.value     = _lensDistortScaleDefault;
        }

        if(_chromAberration)
            _chromAberration.intensity.value = _chromAberrationDefault;

        VFXManager.Instance.RemoveFromUpdateList(this);
    }


    // -- Animation -- //

    [SerializeField]
    private AnimationCurve _chromAberrationAnim, _lensDistortIntensityAnim, _lensDistortScale;

    public void SetAnimation(Transform focusTransform)
    {
        _lensDistortion.center.value = focusTransform == null ? 
                                       _lensDistortCenterDefault :
                                       _camera.WorldToViewportPoint(focusTransform.position);
        IntensityScaleCorrection();
        _enableTime = 0f;
    }

    public void SetAnimation(Vector3 worldSpaceFocusPosition)
    {
        _lensDistortion.center.value = _camera.WorldToViewportPoint(worldSpaceFocusPosition);
        IntensityScaleCorrection();
        _enableTime = 0f;
    }

    public override void UpdateSelf()
    {
        if(!_lensDistortion || !_chromAberration || _enableTime >= _duration)
        {
            Deactivate();
            return;
        }

        _lensDistortion.intensity.value  = _lensDistortIntensityAnim.Evaluate(_enableTime);
        _lensDistortion.scale.value      = _lensDistortScale.Evaluate(_enableTime);
        _chromAberration.intensity.value = _chromAberrationAnim.Evaluate(_enableTime);
        _enableTime += Time.fixedDeltaTime;
    }



    [SerializeField]
    private float _lensDistortIntensityMax, _lensDistortIntensityMin;
    private float CalculateIntensityInRectByPoint(Vector2 point, float intensityMax, float intensityMin)
    {
        /*
            I want the Lens distortion intensity to scale with the center offset of the lens distortion
            The closer to the screen corners the center is, the less intense the lens distortion will be
            => Intensity scales like the height of a triangle that's intersecting a pyramid, 
                    whose corner and slant side is snapped to a side of the said pyramid
            => The calculation formula is trigonometry
        */

        float _intensityRange = intensityMax - intensityMin;

        Vector2 _vectorFromCenter = point - new Vector2(0.5f, 0.5f);
        _vectorFromCenter.x = Mathf.Abs(_vectorFromCenter.x);
        _vectorFromCenter.y = Mathf.Abs(_vectorFromCenter.y);

        float _bossVectorLength   = _vectorFromCenter.magnitude;
        float _parentTriangleAdjacentLength = 0.5f / (_vectorFromCenter.y / _bossVectorLength);

        float _adjacentSideLength = _parentTriangleAdjacentLength - _bossVectorLength;

        float _intensity = (_adjacentSideLength / _parentTriangleAdjacentLength) * _intensityRange + intensityMin;
        return _intensity;
    }

    //Set the correct intensity scale to the Animation curve
    void IntensityScaleCorrection()
    {
        Keyframe[] keyframes = _lensDistortIntensityAnim.keys;
        keyframes[1].value = CalculateIntensityInRectByPoint(_lensDistortion.center.value, _lensDistortIntensityMax, _lensDistortIntensityMin);            
        _lensDistortIntensityAnim.keys = keyframes;
    }

}


}