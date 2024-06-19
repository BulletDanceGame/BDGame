using UnityEngine;
using UnityEngine.EventSystems;

public class BossController : MonoBehaviour
{
    public static BossController Instance { get; private set; }

    [SerializeField]
    private GameObject _spawnPoint;

    [SerializeField] GameObject _bossGO;
    public BossConductor bossConductor { get; private set; }

    [SerializeField]
    private GameObject[] _bossPrefabs;

    public GameObject boss { get; private set; }
    public BossHealthController bossHealth { get; private set; }

    public GameObject graphics { get; private set;}

    public int currentBoss      = 0; // 0 is nothing
    public int currentBossPhase = 1;
    

    private void Awake()
    {
        //Set instance if null
        if(Instance == null) Instance = this;
    }

    private void Start()
    {
        //Don't do anything if is not Instance (dupe)
        if(Instance != null && Instance != this) return;
        EventManager.Instance.OnSetupBossFight += SetCurrentBoss;
        EventManager.Instance.OnBossPhaseChange += OnPhaseChange;
    }

    private void OnDestroy()
    {
        //Don't do anything if is not Instance (dupe)
        if(Instance != null && Instance != this) return;

        //Set instance null so next scene's boss controller can be Instance
        Instance = null;
        EventManager.Instance.OnSetupBossFight -= SetCurrentBoss;
        EventManager.Instance.OnBossPhaseChange -= OnPhaseChange;
    }


    void OnPhaseChange(int phaseNum)
    {
        currentBossPhase = phaseNum;
    }

    void OnPhaseChange()
    {
        currentBossPhase++;
    }


    void SetCurrentBoss(int index)
    {
        Vector2 spawnPoint = _spawnPoint == null ? Vector2.zero :  _spawnPoint.transform.position;

        if (_bossGO == null)
        {
            _bossGO = Instantiate(_bossPrefabs[index], spawnPoint, transform.rotation);
            _bossGO.transform.parent = transform;
            
        }
        else //already existing
        { 
            _bossGO.SetActive(true);
            _bossGO.transform.position = spawnPoint;
        }

        //music controller
        bossConductor = _bossGO.GetComponent<BossConductor>();
        bossConductor.SetUp();

        boss = _bossGO;

        graphics = _bossGO.transform.Find("Graphics").gameObject;
        bossHealth = boss.GetComponentInChildren<BossHealthController>();

        print("boss spawn");
    }


    public void Spawn()
    {
        print("spawn " + Time.timeAsDouble);
        graphics.SetActive(true);
    }
}
