using UnityEngine;

public class EnemyDetection : MonoBehaviour
{

    private RoomController _enemyController;

    [SerializeField] private bool _inCover;
    [SerializeField] private AK.Wwise.Event _detectionSFX;

    public void SetEnemyController(RoomController controller)
    {
        _enemyController = controller;
    }


    private void Detection()
    {
        //if it's been deactivated but triggered twice
        if (gameObject.activeSelf == false) { return; }


        if (_enemyController)
        {
            //Tell Controller Player has been detected
            _enemyController.PlayerBeenDetected(transform.parent.gameObject);
        }


        //activate player
        GetComponentInParent<Movelist>().Activate();

        //activate detection alert for other enemies
        transform.GetChild(0).gameObject.SetActive(true);

        //Play alert animation if UnitAnimationHandler exists
        transform.parent.GetComponentInChildren<BulletDance.Animation.UnitAnimationHandler>()?.Alerted();

        SoundManager.Instance.PlaySFX(_detectionSFX); //should come from one of the sound container scripts as an event

        if (_inCover)
        {
            print("BU!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.name == "DetectionAlert")
        {
            Detection();
            GetComponent<PolygonCollider2D>().enabled = false;
        }
    }
}
