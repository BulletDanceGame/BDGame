using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelSettings : MonoBehaviour
{
    [SerializeField] GameObject _winScreen;

    void OnEnable()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnSceneLoaded += SceneLoaded;
    }


    void OnDisable()
    {
        if(EventManager.Instance == null) return;
        EventManager.Instance.OnSceneLoaded -= SceneLoaded;
        EventManager.Instance.OnWin -= EnableWinScreen;
    }

    void SceneLoaded()
    {
        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {
        yield return new WaitForEndOfFrame();
        EventManager.Instance.OnWin += EnableWinScreen;

        GameManager.Instance.ChangeGameState(GameState.GAME);
    }


    void EnableWinScreen()
    {
        _winScreen.SetActive(true);
        //for now, this hardcoding should be changed tho 
        if (_winScreen.transform.Find("Input") == null) { Debug.LogWarning("The name of the parent to the button is no longer Input!"); }
        _winScreen.transform.Find("Input").GetComponentInChildren<Button>().Select();
    }
}
