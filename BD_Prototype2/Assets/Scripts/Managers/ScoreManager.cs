using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

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
        }
    }


    public int SessionScore { get; private set; }

    [SerializeField]
    private int _maxTimeScore = 10000;
    public  int CurrentTimeScore { get; private set; }

    [SerializeField]
    private int  _maxCombo = 10;
    public  int  ComboMulti { get; private set; }
    public  bool isMaxCombo { get { return ComboMulti >= _maxCombo; } }

    [HideInInspector]
    public int GoodHits, PerfectHits, BadHits, GotHit;


    //Put reset score on some events (go to main menu, etc.)
    void Start()
    {
        ResetScore();

        EventManager.Instance.OnUpdateTimer += DecreaseTimeBonus;
        EventManager.Instance.OnAddScore    += AddScore;
        EventManager.Instance.OnPlayerMiss  += ResetCombo;
    }


    void AddScore(int score)
    {
        ComboMulti++;
        ComboMulti = Mathf.Clamp(ComboMulti, 1, _maxCombo);

        SessionScore += score * ComboMulti;
    }

    void ResetCombo()
    {
        ComboMulti = 1;
    }

    void DecreaseTimeBonus()
    {
        CurrentTimeScore -= 50;
        CurrentTimeScore = Mathf.Clamp(CurrentTimeScore, 0, _maxTimeScore);
    }

    void ResetScore()
    {
        CurrentTimeScore = _maxTimeScore;
        ComboMulti = 1;

        SessionScore = 0;

        GoodHits = 0;
        PerfectHits = 0;
        BadHits = 0;
        GotHit = 0;
    }
}
