using UnityEngine;

public class BossFightTrigger : MonoBehaviour
{
    private bool _triggered = false;

    public int bossPrefabID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_triggered) return;

        if (collision.gameObject.tag == "Player")
        {
            EventManager.Instance.SetupBossFight(bossPrefabID);
            EventManager.Instance.BossSpawn();

            _triggered = true;
        }        
    }
}
