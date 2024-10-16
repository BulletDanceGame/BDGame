using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField]
    private SceneManager.LoadOptions loadOptions = 0;

    [Space]
    [Header("Choose Scene here if load option is Specified")]
    [Tooltip("If scene is missing, add it in enum of the SceneManager-Script. Don't worry about the space between words shown here")]
    [SerializeField]
    private SceneManager.Scenes _sceneToLoad;

    private bool _wasTriggered = false;

    public List<GameObject> gameObjectsToDeactivateOnTrigger = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") return;
        if (_wasTriggered) return;

        switch (loadOptions)
        {
            case SceneManager.LoadOptions.Specified:
                RTPCManager.Instance.SetAttributeValue("VOLUME", 0, 1f, RTPCManager.CurveTypes.linear, "VOLUME____Menu");
                MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.FADE_STOP);
                SceneManager.Instance.LoadScene(_sceneToLoad, 3.5f);
                break;

            case SceneManager.LoadOptions.NextLevel:
                RTPCManager.Instance.SetAttributeValue("VOLUME", 0, 1f, RTPCManager.CurveTypes.linear, "VOLUME____Menu");
                MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.FADE_STOP);
                SaveSystem.Instance.GetData().currentLevel = SceneManager.GetActiveScene().buildIndex + 1;
                SaveSystem.Instance.Save();
                SceneManager.Instance.LoadNextScene(3.5f);
                break;

            case SceneManager.LoadOptions.Reload:
                RTPCManager.Instance.SetAttributeValue("VOLUME", 0, 1f, RTPCManager.CurveTypes.linear, "VOLUME____Menu");
                MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.FADE_STOP);
                SceneManager.Instance.ReloadCurrentScene(3.5f);
                break;

            default: break;
        }

        _wasTriggered = true;
        Invoke("ResetTrigger", 1f); //Prevent double trigger bc the player has 2 coliders

        for (int i = 0; i < gameObjectsToDeactivateOnTrigger.Capacity; i++)
        {
            gameObjectsToDeactivateOnTrigger[i].SetActive(false);
        }
    }

    void ResetTrigger() { _wasTriggered = false; }
}
