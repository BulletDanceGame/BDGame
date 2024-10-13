using UnityEngine;

public class AfterImage : MonoBehaviour
{
    [SerializeField]
    private float _activeTime;
    private float _timeActivated;
    private float _alpha;

    [SerializeField]
    private float _alphaInit = 1f;

    [SerializeField]
    private float _alphaMiltiplier;

    [SerializeField]
    private Color _defaultColor;
    private Color _color;


    private Transform _target;
    private Vector3 _offset;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _targetRenderer;

    private bool _isUpdate = false;


    public void SetTargetRenderer(SpriteRenderer renderer)
    {
        _targetRenderer = renderer;
    }
    public void SetTargetTransform(Transform target)
    {
        _target = target;
    }
    public void SetPositionOffset(Vector3 offset)
    {
        _offset = offset;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    [SerializeField]
    private Material _blockMaterial;
    private bool _useBlockMaterial = false;
    public void SetBlockMaterial(bool useBlockMaterial)
    {
        _useBlockMaterial = useBlockMaterial;
    }

    public void SetColor(bool customColor, Color color)
    {
        _color = customColor ? color : _defaultColor;
    }


    public void Enable()
    {
        _isUpdate = true;
        this._spriteRenderer.enabled = true;

        _alpha = _alphaInit;
        _spriteRenderer.sprite = _targetRenderer.sprite;

        if(!_useBlockMaterial)
            _spriteRenderer.material = _targetRenderer.material;
        else
            _spriteRenderer.material = _blockMaterial;

        transform.position = _target.position + _offset;
        transform.rotation = _target.rotation;

        _timeActivated = Time.time;

    }
    public void Disable()
    {
        _isUpdate = false;
        this._spriteRenderer.enabled = false;
    }


    private void Update()
    {
        if (!_isUpdate)
            return;

        _alpha *= _alphaMiltiplier;
        _color.a = _alpha;
        _spriteRenderer.color = _color;

        if(Time.time>=(_timeActivated+_activeTime))
        {
            AfterImagePool.Instance.AddIntoPool(this);
        }
    }

    public void DetachFromParent()
    {
        transform.parent = null;
    }
}
