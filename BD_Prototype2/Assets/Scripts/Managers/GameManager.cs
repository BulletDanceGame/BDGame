using UnityEngine;
using System.Collections;

public enum GameState { 
    MAINMENU,
    GAMEOVER, 
    CUTSCENE, 
    GAME, 
    FIGHT, 
    NEWGAME,
    CONTINUE
};

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State;

    public int ElapsedTime { get; private set; }
    [SerializeField] bool CountTime = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeGameState(GameState.MAINMENU); //ok i dont know why this doesnt affect vertical slice

        //SessionScore = 0;
    }


    public void ChangeGameState(GameState _newState)
    {
        switch (_newState)
        {
            case GameState.MAINMENU:
                State = GameState.MAINMENU;
                MainMenu();
                break;
            case GameState.GAMEOVER:
                State = GameState.GAMEOVER;
                GameOver();
                break;
            case GameState.CUTSCENE:
                State = GameState.CUTSCENE;
                break;
            case GameState.GAME: 
                State = GameState.GAME;
                GamePlay();
                break;
            case GameState.FIGHT:
                State = GameState.FIGHT;
                Fight();
                break;
            case GameState.NEWGAME:
                State = GameState.NEWGAME;
                NewGame();
                break;
            case GameState.CONTINUE:
                State = GameState.CONTINUE;
                //ContinueGame();
                break;
            default:
                Debug.LogError("State is default \n you have reached a default state");
                break;
        }
    }

    //Has to be a coroutine, If you fire off events inside of start() somtimes it can cause a null refrence exception so im QUEUEing 1 frame before i fire the event.
    void NewGame()
    {
        ChangeGameState(GameState.GAME);
    }

    void GamePlay()
    {
        EventManager.Instance.StartGamePlay();

        EventManager.Instance.OnStartTimer += StartTimer;

        EventManager.Instance.OnResetTimer += ResetTimer;
        ResetTimer();

        EventManager.Instance.OnStopTimer += StopTimer;
        EventManager.Instance.OnPlayerDeath += StopTimer;
        EventManager.Instance.OnBossDeath += StopTimer;
    }

    void MainMenu()
    {
        //Put reset Score here (event?)
    }

    void GameOver()
    {

    }

    void Fight()
    {

    }

    void ContinueGame()
    {
    }

    void StartTimer()
    {
        print(name + " " + gameObject.activeSelf);
        if (!CountTime)
            StartCoroutine(UpdateTimer());
    }

    IEnumerator UpdateTimer()
    {
        CountTime = true;

        while (CountTime)
        {
            yield return new WaitForSeconds(1f);

            ElapsedTime += 1;

            EventManager.Instance.UpdateTimer();
        }
    }

    void StopTimer(string nope)
    {
        CountTime = false;
    }

    void StopTimer()
    {
        CountTime = false;
    }

    void ResetTimer()
    {
        ElapsedTime = 0;
    }
}
