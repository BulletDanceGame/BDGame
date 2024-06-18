using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    private bool _wasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") return;
        if (_wasTriggered) return;

        EventManager.Instance.Win();

        _wasTriggered = true;
        Invoke("ResetTrigger", 1f); //Prevent double trigger bc the player has 2 coliders
    }

    void ResetTrigger() { _wasTriggered = false; }
}
