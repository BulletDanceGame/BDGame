using UnityEngine;

public class TowersRotate : MonoBehaviour
{
    [SerializeField]
    private Vector2 _direction;
    private float _angleToPlayer = 0f;
    private Vector3 _towerAngle;

    GameObject player;

    void Start()
    {
        _towerAngle = transform.localEulerAngles;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if(!UnitManager.Instance.GetPlayer()) return;

        _angleToPlayer = Vector2.SignedAngle(
                        player.transform.position - transform.position,
                        _direction);

        _towerAngle.z = - _angleToPlayer;
        transform.localEulerAngles = _towerAngle;
    }
}
