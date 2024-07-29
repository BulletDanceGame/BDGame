using UnityEngine;
using SM = UnityEngine.SceneManagement;

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
                RTPCManager.Instance.SetAttributeValue("VOLUME", 0, 1f, RTPCManager.CurveTypes.linear, "VOLUME____Menu");
                SceneManager.Instance.LoadScene(_sceneToLoadOnPlay, 3.5f);
                break;

            case SceneManager.LoadOptions.NextLevel:
                RTPCManager.Instance.SetAttributeValue("VOLUME", 0, 1f, RTPCManager.CurveTypes.linear, "VOLUME____Menu");
                SceneManager.Instance.LoadNextScene(3.5f);
                break;

            case SceneManager.LoadOptions.Reload:
                RTPCManager.Instance.SetAttributeValue("VOLUME", 0, 1f, RTPCManager.CurveTypes.linear, "VOLUME____Menu");
                SceneManager.Instance.ReloadCurrentScene(3.5f);
                break;

            case SceneManager.LoadOptions.RespawnPlayer:

                if(CheckpointManager.instance.GetCurrentCheckPoint() == null)
                {
                    RTPCManager.Instance.SetAttributeValue("VOLUME", 0, 1f, RTPCManager.CurveTypes.linear, "VOLUME____Menu");
                    SceneManager.Instance.ReloadCurrentScene(3.5f);
                    return;
                }

                UnitManager.Instance.RespawnPlayer();
                //SceneManager.Instance.ReloadCurrentScene(3.5f);
                //RTPCManager.Instance.SetAttributeValue("VOLUME", 0, 1f, RTPCManager.CurveTypes.linear, "VOLUME____Menu");

                break;
            case SceneManager.LoadOptions.Continue:
                SaveData sd = SaveSystem.Instance.GetData();

                switch (sd.currentLevel)
                {
                    case 0:
                        SceneManager.Instance.LoadScene(SceneManager.Scenes.Tutorial, 3.5f);
                        break;
                    case int n when (n > SM.SceneManager.sceneCountInBuildSettings):
                        SceneManager.Instance.LoadScene(SceneManager.Scenes.YokaiHunterBoss, 3.5f);
                        break;
                    default:
                        SceneManager.Instance.LoadScene((SceneManager.Scenes)sd.currentLevel, 3.5f);
                        break;

                }
                break;

            default: break;
        }

        gameObject.SetActive(false);
    }
}
