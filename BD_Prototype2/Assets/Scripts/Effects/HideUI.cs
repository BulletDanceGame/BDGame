using UnityEngine;

public class HideUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform _UITrf;
    [SerializeField]
    private CanvasGroup _canvasGroup;

    private float _alpha = 1f;
    [SerializeField]
    private float _hideAlpha = 0.2f;
    [SerializeField]
    private float _hideAlphaSpeed = 0.2f;

    [SerializeField]
    private Vector3 _playerSize;
    private Vector3 _playerSizeDefault;
    private Transform _playerTrf;

    private Vector2 _playerScreenPos;
    private Bounds _boundary, _playerUIHitbox;

    private Camera _camera;
    private float _cameraSizeDefault = 21f;
    private float _cameraOffsetMagnitude = 1f;

    bool foundPlayer = false;

    void Start()
    {
        _camera = Camera.main;

        Vector3 _boundarySize = new Vector3(_UITrf.sizeDelta.x * _UITrf.localScale.x, _UITrf.sizeDelta.y * _UITrf.localScale.y, 0);
        _boundary = new Bounds((Vector3)_UITrf.position, _boundarySize);
        
        _playerUIHitbox = new Bounds(Vector3.zero, _playerSize);
        _playerSizeDefault = _playerSize;
    }

    void Update()
    {
        if (!foundPlayer)
        {
            if (UnitManager.Instance.GetPlayer() == null)
                return;

            _playerTrf = UnitManager.Instance.GetPlayer().transform;

            foundPlayer = true;
        }

        _playerScreenPos = _camera.WorldToScreenPoint(_playerTrf.position);
        _playerUIHitbox.center = (Vector3)_playerScreenPos;

        _cameraOffsetMagnitude =  _camera.orthographicSize / _cameraSizeDefault;
        _playerSize = _playerSizeDefault / _cameraOffsetMagnitude;

        if(_boundary.Intersects(_playerUIHitbox))
            _alpha = _alpha > _hideAlpha ? _alpha - (Time.deltaTime * _hideAlphaSpeed) : _alpha;
        else
            _alpha = _alpha < 1f ? _alpha + (Time.deltaTime * _hideAlphaSpeed) : _alpha;

        _canvasGroup.alpha = _alpha;
        _alpha = _canvasGroup.alpha;
    }
}