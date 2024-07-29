using System;
using System.Collections;
using UnityEngine;
using SM = UnityEngine.SceneManagement;


/*
** !! This should be the only script that uses UnityEngine.SceneManagement !!
** ---------------------------------------------------------------------------
**    Contains methods for scene loading
**    Contains preset load handers to avoid string finicking across scripts
*/

public class SceneManager : MonoBehaviour
{
    //Putting this here because no scripts should reference Unity's SceneManager
    public static SM.Scene GetActiveScene()
    {
        return SM.SceneManager.GetActiveScene();
    }

    public static SceneManager Instance { get; private set; }
    
    public enum LoadOptions { Specified = 0, NextLevel = 1, Reload = 2, RespawnPlayer = 3, Continue = 4}


    //ADD ANY SCENES HERE (AND IN THE BUILD SETTINGS)
    public enum Scenes { Menu = 0, Tutorial = 1, BigCritterBoss = 2, YokaiHunterBoss = 3, DashLevel = 4}
    private Scenes _sceneToLoad = 0;
    public Scenes _currentScene { private set; get; } = 0;

    [SerializeField]
    private LoadingScreen screenTransition;

    private AsyncOperation _asyncLoad;



    // -- Init & Events -- //

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            SM.SceneManager.sceneLoaded += SceneLoaded; //Sub On Awake so level settings work
        }
    }

    private void Start()
    {
        //Get name of current scene
        string currentSceneName = GetActiveScene().name;
        if (!DoesSceneExist(currentSceneName)) { return; }
        //turn the string into the enum
        _currentScene = (Scenes)Enum.Parse(typeof(Scenes), currentSceneName, true);
    }

    private void Destroy()
    {
        SM.SceneManager.sceneLoaded -= SceneLoaded;
    }

    void SceneLoaded(SM.Scene scene, SM.LoadSceneMode mode)
    {
        EventManager.Instance?.SceneLoaded();
    }


    // -- Load Handler -- // 
    
    public void LoadScene(Scenes scene, float delay = 0f)
    {
        EventManager.Instance?.SceneLoad();

        if (ConductorManager.Instance) { 
            ConductorManager.Instance.RemoveCurrentController(MusicManager.TransitionType.QUEUE_STOP);
        }

        _sceneToLoad = scene;

        SetAudioState(_sceneToLoad);

        StartCoroutine(AsyncSceneLoad(_sceneToLoad, delay));
    }

    public void LoadNextScene(float delay = 0f)
    {
        if(!DoesSceneExist((int)_currentScene + 1)) { return; }

        _currentScene++;

        LoadScene(_currentScene, delay);
    }

    public void ReloadCurrentScene(float delay = 0f)
    {
        LoadScene(_currentScene, delay);
    }



    bool DoesSceneExist(string sceneName)
    {
        bool exists = Enum.IsDefined(typeof(Scenes), sceneName);

        if (!exists) { Debug.LogWarning("The scene " + sceneName + " could not be found, check if the scene name in the enum is correct & if it is in the build settings"); }
 
        return exists;
    }
    bool DoesSceneExist(int sceneNumber)
    {
        bool exists = Enum.IsDefined(typeof(Scenes), sceneNumber);

        if (!exists) { Debug.LogWarning("The scene of number " + sceneNumber + " could not be found, there are not that many scenes in the enum!"); }

        return exists;
    }



    IEnumerator AsyncSceneLoad(Scenes scene, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        _asyncLoad = SM.SceneManager.LoadSceneAsync(scene.ToString());
        while (!_asyncLoad.isDone)
        {
            yield return null;
        }
        _currentScene = scene;

        //This doesnt happen, prolly cause this script isnt DontDestroyOnLoad :)
        Debug.Log("Loading Completed");
    }



    //states for sound
    public void SetTutorialScene() //i know this is a bit cluttery, buuuuut - dont worry about it :D
    {

        //Yeah it should work just using the SceneLoad now?

        //_currentScene = Scenes.Tutorial;
        //SetAudioState();
    }


    public void SetAudioState(Scenes scene)
    {
        print("SCENE TO LOAD: " + scene);
        switch (scene) //Set "Level State" so sound fades smoothly between scenes
        {
            case Scenes.Menu:
                AkSoundEngine.SetState("Level", "Menu");
                print("STATE: MENU");
                break;
            case Scenes.Tutorial:
                AkSoundEngine.SetState("Level", "Tutorial");
                print("STATE: TUTORIAL");
                break;
            case Scenes.BigCritterBoss:
                AkSoundEngine.SetState("Level", "CritterBoss");
                print("STATE: CRITTER");
                break;
            case Scenes.YokaiHunterBoss:
                AkSoundEngine.SetState("Level", "YokaiHunterBoss");
                print("STATE: YOKAI HUNTER");
                break;
            case Scenes.DashLevel:
                AkSoundEngine.SetState("Level", "Tutorial");
                print("STATE: TUTORIAL");
                break;
            default:
                break;
        }
    }
}