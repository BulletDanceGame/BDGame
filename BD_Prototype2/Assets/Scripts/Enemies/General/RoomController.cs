using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;


public class RoomController : MonoBehaviour
{
    //general
    public enum RoomType { RoamingArea, KillRoom, SurvivalRoom}
    [SerializeField] private RoomType _roomType;


    [SerializeField] private List<EnvironmentConductor> _environmentConductors = new List<EnvironmentConductor>();


    [SerializeField] private AK.Wwise.Event _startBattleSFX;
    [SerializeField] private AK.Wwise.Event _endBattleSFX;

    //killroom
    [SerializeField] private KillRoomWave[] _waves;
    private int _currentWave = 0;
    [Serializable]
    public struct KillRoomWave
    {
        public List<GameObject> _enemies;
        public int _enemiesAliveToStartNextWave; //inspector idea, hide this when its the last one?, make it a range between 0 and enemies count?
    }

    //survival
    [SerializeField] private int _durationInBeats;
    private bool _startSurvival = false;
    private int _beatTimer = 0;
    [SerializeField] private List<GameObject> _initialEnemies;
    [SerializeField] private List<GameObject> _spawnableEnemies;
    private List<GameObject> _enemiesSpawnedDuringFight = new List<GameObject>();
    private int _spawnableEnemeisIndex = 0;
    [Tooltip("If deactivated, spawning will follow the order of the list")]
    [SerializeField] private bool _randomizeSpawning;
    [SerializeField] private int _enemiesKilledBeforeSpawn;
    private int _enemiesKilled = 0;
    [SerializeField] private int _enemiesSpawnedEachTime;
    [SerializeField] private GameObject _timerUIPrefab;
    private UIBar _timerUI;

    //both
    [SerializeField] private List<Gate> _gatesToLock = new List<Gate>();
    private List<GameObject> _enemiesToSpawn = new List<GameObject>();
    public static List<GameObject> _currentActiveEnemies { get; private set; } = new List<GameObject>();
    [SerializeField] private GameObject _spawnMarkerPrefab;
    [SerializeField] private AK.Wwise.Event _enemySpawnSFX;
    [SerializeField] private GameObject _vfxSmokePrefab;


    //roaming
    [SerializeField] private List<GameObject> _enemies = new List<GameObject>();





    //ZONE TOOLS
    public void SetConductors(List<EnvironmentConductor> conductors)
    {
        _environmentConductors = conductors;
    }

    public void AddConductor(EnvironmentConductor conductor)
    {
        if(!_environmentConductors.Contains(conductor))
            _environmentConductors.Add(conductor);
    }

    public void RemoveNullConductors()
    {
        _environmentConductors.RemoveAll(item => item == null);
    }

    public void SetEnemies(List<GameObject> enemies)
    {
        _enemies = enemies;
    }

    public void AddEnemies(List<GameObject> enemies)
    {
        foreach(var enemy in enemies)
        {
            if(!_enemies.Contains(enemy))
                _enemies.Add(enemy);
        }
    }

    public void RemoveNullEnemies()
    {
        _enemies.RemoveAll(item => item == null);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnBeatForVisuals += InitiateEnemySpawning;
        EventManager.Instance.OnBeat += Beat;
    }
    private void OnDisable()
    {
        EventManager.Instance.OnBeatForVisuals -= InitiateEnemySpawning;
        EventManager.Instance.OnBeat -= Beat;

    }

    //GENERAL
    void Start()
    {
        if (_roomType == RoomType.RoamingArea)
        {
            //Give Enemies their Controller
            for (int i = 0; i < _enemies.Count; i++)
            {
               // _enemies[i].GetComponentInChildren<EnemyDetection>().SetEnemyController(this);
               // _enemies[i].GetComponentInChildren<EnemyHealthController>().SetEnemyController(this);
            }
        }


        foreach (GameObject enemy in _enemies)
        {
            UnitManager.Instance.Enemies.Add(enemy);
        }
    }


    public void PlayerBeenDetected(GameObject enemy)
    {
        //activate enemies depending on what type of room it is
        if (_roomType == RoomType.RoamingArea)
        {
            if (enemy != null)
            {
                _currentActiveEnemies.Add(enemy);
            }
        }
        else if (_roomType == RoomType.KillRoom)
        {
            StartKillRoom();
        }
        else if (_roomType == RoomType.SurvivalRoom)
        {
            StartSurvivalRoom();
        }
    }

    public void StartKillRoom()
    {        
        //give enemies their controller and deactivate them
        if (GetComponent<MusicFollower>())
        {
            GetComponent<MusicFollower>().SetUp();
        }
        

        foreach (KillRoomWave wave in _waves)
        {
            foreach (GameObject enemy in wave._enemies)
            {
                enemy.GetComponentInChildren<EnemyDetection>().gameObject.SetActive(false);
                enemy.GetComponentInChildren<EnemyHealthController>().SetEnemyController(this);
                if (enemy.GetComponent<MusicFollower>())
                {
                    enemy.GetComponent<MusicFollower>().SetUp(); //because fuck this
                }
            }
        }
        _enemiesToSpawn = _waves[0]._enemies;
        _currentActiveEnemies.Clear();

        //Change Music
        EnvironmentConductor.SwitchSequenceType(EnvironmentConductor.SequenceType.Battle, MusicManager.TransitionType.INSTANT_SWITCH);
        SoundManager.Instance.PlaySFX(_startBattleSFX); //should come from one of the sound container scripts as an event

        //lock all doors
        foreach (Gate gate in _gatesToLock)
        {
            gate.LockDoor();
            gate.CloseDoor();
        }
    }

    public void StartSurvivalRoom()
    {
        _startSurvival = true;
        _timerUI = Instantiate(_timerUIPrefab, GameObject.Find("UI").transform).GetComponent<UIBar>();
        _timerUI.SetUpUIBar(_durationInBeats, 0);


        //Activate Enemies
        foreach (GameObject enemy in _initialEnemies)
        {
            enemy.GetComponentInChildren<EnemyDetection>().gameObject.SetActive(false);
            enemy.GetComponentInChildren<EnemyHealthController>().SetEnemyController(this);
            enemy.GetComponent<MusicFollower>().SetUp();//because fuck this
        }
        foreach (GameObject enemy in _spawnableEnemies)
        {
            enemy.GetComponentInChildren<EnemyDetection>().gameObject.SetActive(false);
            enemy.GetComponentInChildren<EnemyHealthController>().SetEnemyController(this);
            enemy.GetComponent<MusicFollower>().SetUp();//because fuck this
        }


        _enemiesToSpawn = _initialEnemies;


        //Change Music
        EnvironmentConductor.SwitchSequenceType(EnvironmentConductor.SequenceType.Battle, MusicManager.TransitionType.INSTANT_SWITCH);
        SoundManager.Instance.PlaySFX(_startBattleSFX); //should come from one of the sound container scripts as an event

        

        //lock all doors
        foreach (Gate gate in _gatesToLock)
        {
            gate.LockDoor();
            gate.CloseDoor();
        }
    }

    private void Beat(int beat)
    {
        if (_startSurvival)
        {
            _beatTimer++;
            _timerUI.IncreaseValue(1);
            if (_beatTimer == _durationInBeats)
            {
                EndBattle();
            }
        }
    }

    private void InitiateEnemySpawning(int anticipation, float s, int beat)
    {
        if (anticipation != 4) { return; }
        if (_enemiesToSpawn.Count == 0) { return; }

        //choose corrrect enemy
        GameObject enemy = _enemiesToSpawn[0];
        _enemiesToSpawn.RemoveAt(0);
        _currentActiveEnemies.Add(enemy);
        print("active " + _currentActiveEnemies.Count);

        UnitManager.Instance.ActiveEnemies.Add(enemy);

        //set marker
        GameObject marker = Instantiate(_spawnMarkerPrefab, enemy.transform.position, transform.rotation);
        marker.GetComponent<Animator>().speed = 1f / ((float)MusicManager.Instance.secondsPerBeat * 4);
        Destroy(marker, 2);

        StartCoroutine(EnemySpawnTimer(enemy));

    }

    IEnumerator EnemySpawnTimer(GameObject enemy)
    {
        yield return new WaitForSeconds((float)MusicManager.Instance.secondsPerBeat * 4);

        //activate enemy
        enemy.SetActive(true);
        enemy.GetComponent<Movelist>().Activate();

        //spawn effects
        SoundManager.Instance.PlaySFX(_enemySpawnSFX); //should come from one of the sound container scripts as an event
        GameObject smoke = Instantiate(_vfxSmokePrefab, enemy.transform.position, transform.rotation);
        smoke.GetComponent<ParticleSystem>().Play();
        Destroy(smoke, 2);

        print("spawn?");

        //Added this for ease of access - Daniel
        UnitManager.Instance.ActiveEnemies.Add(enemy);
    }


    public void EnemyDeath(GameObject enemy)
    {
        //check if all enemies are dead, redo this based on how we kill off enemies
        if (_roomType == RoomType.KillRoom)
        {
            _currentActiveEnemies.Remove(enemy);

            print("currently " + _currentActiveEnemies.Count);

            if (_currentActiveEnemies.Count == _waves[_currentWave]._enemiesAliveToStartNextWave)
            {
                _currentWave++;

                if (_currentWave == _waves.Length) 
                {
                    EndBattle();
                }
                else
                {
                    print("kill spawn more " + _waves[_currentWave]._enemies.Count);
                    _enemiesToSpawn = _waves[_currentWave]._enemies;
                }                
            }
        }
        else if (_roomType == RoomType.SurvivalRoom)
        {
            if (!_startSurvival) return;//battle over

            _enemiesKilled++;
            
            //add enemies back to be able to be respwan
            _currentActiveEnemies.Remove(enemy);
            if (_enemiesSpawnedDuringFight.Contains(enemy))
            {
                _enemiesSpawnedDuringFight.Remove(enemy);
                _spawnableEnemies.Add(enemy);
                enemy.GetComponent<EnemyHealthController>().CancelInvoke("DestroyObject");
                enemy.GetComponent<EnemyHealthController>().SetUp();
            }

            if (_enemiesKilled == _enemiesKilledBeforeSpawn)
            {
                _enemiesKilled = 0; //fix

                for (int i = 0; i < _enemiesSpawnedEachTime; i++)
                {
                    //Random
                    if (_randomizeSpawning)
                    {
                        if (_spawnableEnemies.Count == 0) return;

                        int rand = UnityEngine.Random.Range(0, _spawnableEnemies.Count);

                        _enemiesToSpawn.Add(_spawnableEnemies[rand]);
                        _enemiesSpawnedDuringFight.Add(_spawnableEnemies[rand]);
                        _spawnableEnemies.RemoveAt(rand);

                    }
                    //Follow List
                    else if (!_randomizeSpawning)
                    {
                        if (!_currentActiveEnemies.Contains(_spawnableEnemies[_spawnableEnemeisIndex]))
                        {
                            _enemiesToSpawn.Add(_spawnableEnemies[_spawnableEnemeisIndex]);
                        }

                        _spawnableEnemeisIndex++;
                        if (_spawnableEnemeisIndex == _spawnableEnemies.Count)
                        {
                            _spawnableEnemeisIndex = 0;
                        }
                    }

                }
            }

        }
        else if (_roomType == RoomType.RoamingArea)
        { 
            _currentActiveEnemies.Remove(enemy);
        }
        
    }



    public void EndBattle()
    {
        //music
        for (int i = 0; i < _environmentConductors.Count; i++)
        {
            //EnvironmentConductor.SwitchSequenceType(EnvironmentConductor.SequenceType.BattleOutro, MusicManager.TransitionType.INSTANT_SWITCH);
            //EnvironmentConductor.SwitchSequenceType(EnvironmentConductor.SequenceType.Roaming, MusicManager.TransitionType.QUEUE_SWITCH);
        }

        SoundManager.Instance.PlaySFX(_endBattleSFX); //should come from one of the sound container scripts as an event


        if (_roomType == RoomType.SurvivalRoom)
        {
            _startSurvival = false;
            foreach (GameObject enemy in _currentActiveEnemies)
            {
                enemy.GetComponent<EnemyHealthController>().Death();
            }
            _currentActiveEnemies.Clear();

            //Unitmanager stuff :) -Daniel
            UnitManager.Instance.ActiveEnemies.Clear();

            BulletBag.instance.DeactivateAllBullets();
            Destroy(_timerUI.gameObject);
        }

        foreach (Gate gate in _gatesToLock)
        {
            gate.UnlockDoor();
            gate.OpenDoor();
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerBeenDetected(null);
            GetComponent<PolygonCollider2D>().enabled = false;

        }
    }

}




