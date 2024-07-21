using UnityEngine;

public class SceneLoadButton : MonoBehaviour
{

    [SerializeField]
    private SceneManager.LoadOptions loadOptions = 0;

    [Space]
    [Header("Choose Scene here if load option is Specified")]
    [Tooltip("If scene is missing, add it in enum of the SceneManager-Script. Don't worry about the space between words shown here")]
    [SerializeField]
    private SceneManager.Scenes _sceneToLoadOnPlay;

    public void LoadScene()
    {
        switch (loadOptions)
        {
            case SceneManager.LoadOptions.Specified:
                SceneManager.Instance.LoadScene(_sceneToLoadOnPlay, 3.5f);
                break;

            case SceneManager.LoadOptions.NextLevel:
                SceneManager.Instance.LoadNextScene(3.5f);
                break;

            case SceneManager.LoadOptions.Reload:
                SceneManager.Instance.ReloadCurrentScene(3.5f);
                break;

            case SceneManager.LoadOptions.RespawnPlayer:

                if(CheckpointManager.instance.GetCurrentCheckPoint() == null)
                {
                    SceneManager.Instance.ReloadCurrentScene(3.5f);
                    return;
                }

                UnitManager.Instance.RespawnPlayer();

                break;

            default: break;
        }
    }
}
