using System.Collections.Generic;
using UnityEngine;

public class BossConductor : MusicConductor
{
    public static BossConductor Instance;

    [SerializeField] private Movelist _movelist;

    //array that is used to create beat actions from the inspector
    [SerializeField] private BeatAction[] _beatActions; //will be removed


    //list that keeps track of what actions should happen on which beats for the currently used map
    private List<(int, Note)> _currentNoteList = new List<(int, Note)>();
    private int _beatIndex = 0;

    //this shows which sequence is currently in use
    private int _nextSequenceIndex = 0;
    //and this shows what the last beat was on the use of the current map
    private int _beatAtStartOfThisMap = 0;

    //_phases
    //Contains several Sequences and has an hp-limit on when it should go over to the next phase/ is a transition


    [System.Serializable]
    public struct Phase
    {
        public string name;
        public bool transition;
        public MusicSequence[] sequences;
        public string cutsceneName;
        public string sequencePlayedFirst;
        public bool neverPlayFirstSequenceAgain;
        public bool lastTransition;
    }
    [SerializeField] private Phase[] _phases;
    private int _currentPhaseIndex = 0;



    private void Awake()
    {
        Instance = this;
    }

    public void SetUp()
    {
        BeatMapReader.ReadBeatMapsBoss(_phases);

        SplitSequenceWeight();

        EventManager.Instance.OnBeat += CheckBeat;
        EventManager.Instance.OnBossPhaseChange += PhaseChange;

        ConductorManager.Instance.AddController(this, MusicManager.TransitionType.INSTANT_STOP);


        _nextSequence = _phases[_currentPhaseIndex].sequences[0];
        EventManager.Instance.StartCutscene(_nextSequence.cutsceneName);

        print("boss setup");
    }

    private void SplitSequenceWeight()
    {
        for (int p = 0; p < _phases.Length; p++)
        {
            Phase phase = _phases[p];

            for (int seq = 0; seq < phase.sequences.Length; seq++)
            {
                //if a specific one shouldnt be played first, split equally
                if (phase.sequencePlayedFirst == "")
                {
                    phase.sequences[seq].weight = 100.0f / phase.sequences.Length;
                }
                //if theres one that should be played first
                else
                {
                    if (phase.sequencePlayedFirst == phase.sequences[seq].name)
                    {
                        phase.sequences[seq].weight = 100.0f;
                    }
                    else
                    {
                        phase.sequences[seq].weight = 0f;
                    }
                }
            }
        }
    }

    public void PrepareNewPhase()
    {
        _currentPhaseIndex++;
        if (_currentPhaseIndex >= _phases.Length)
        {
            PrepareForEndOfBossFight();
        }

        //Increase volume of PlayerActions (swinging, hitting, etc) with each phase
        if(_currentPhaseIndex == 2)
            RTPCManager.Instance.SetValue("VOLUME_SPECIAL____PlayerActions__Dynamic__Mixing", 50, 0.0000000001f, 0);
        if (_currentPhaseIndex >= 4)
            RTPCManager.Instance.SetValue("VOLUME_SPECIAL____PlayerActions__Dynamic__Mixing", 100, 0.0000000001f, 0);
    }

    public override MusicSequence GetNextSequence()
    {
        //find next sequence
        MusicSequence[] sequences = _phases[_currentPhaseIndex].sequences;

        int index = -1;
        if (_phases[_currentPhaseIndex].neverPlayFirstSequenceAgain)
        {
            for (int i = 0; i < sequences.Length; i++)
            {
                if (sequences[i].name == _phases[_currentPhaseIndex].sequencePlayedFirst)
                {
                    if (sequences[i].weight == 0) //not first time
                    {
                        index = i;
                    }
                }
            }
        }

        if (sequences.Length > 1)
        {
            //randomize the next seqeunce based on weight
            float value = Random.Range(0.0f, 100.0f);
            float minValueInRange = 0.0f;
            float maxValueInRange = 0.0f;
            _nextSequenceIndex = 0;

            for (int i = 0; i < sequences.Length; i++)
            {
                maxValueInRange = minValueInRange + sequences[i].weight;
                
                //if randomized value is in the range of the current sequences weight, it is the chosen one
                if (value > minValueInRange && value <= maxValueInRange)
                {
                    _nextSequenceIndex = i;
                    break;
                }
                else
                {
                    //if not, get the min value in the range of the next sequences weight
                    minValueInRange += sequences[i].weight;
                }
            }

            //Splitting up the weight of the chosen sequence to the other ones
            int fuck = sequences.Length - 1;
            if (index != -1) fuck--;

            float splitWeight = sequences[_nextSequenceIndex].weight / fuck;
            sequences[_nextSequenceIndex].weight = 0.0f;

            for (int i = 0; i < sequences.Length; i++)
            {
                if (i != _nextSequenceIndex && i != index)
                {
                    sequences[i].weight += splitWeight;
                }
            }
        }
        else
        {
            _nextSequenceIndex = 0;
        }

        _nextSequence = sequences[_nextSequenceIndex];

        return _nextSequence;
    }

    /// <summary> Starts a new sequence </summary> 
    public override void SequenceHasStarted()
    {

        //Ambience State Set

        if (_phases[_currentPhaseIndex].transition == false)
        {
            _currentNoteList = _phases[_currentPhaseIndex].sequences[_nextSequenceIndex].noteList;
            _beatAtStartOfThisMap = MusicManager.Instance.currentBeat;
            _beatIndex = 0;
        }
        else
        {
            PrepareNewPhase();
        }
    }

    /// <summary> Checks if the enemy should perform an action on this beat </summary> 
    public void CheckBeat(int beat)
    {
        if (_currentNoteList.Count == 0 || _beatIndex >= _currentNoteList.Count)
            return;

        //get current beat for this map
        int currentBeat = beat - _beatAtStartOfThisMap;

        //print("b 0" + _beatAtStartOfThisMap);
        //print("b " + currentBeat);
        //print("b 2 " + _currentNoteList[_beatIndex].Item1);

        //check if action should be performed
        Note action = new Note();
        if (currentBeat == _currentNoteList[_beatIndex].Item1)
        {
            action = _currentNoteList[_beatIndex].Item2;
            _beatIndex++;
        }

        _movelist.Action(action);
    }

    public void PhaseChange()
    {
        PrepareNewPhase(); // start transition
        MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.INSTANT_STOP);
        _nextSequence = _phases[_currentPhaseIndex].sequences[0];
        EventManager.Instance.StartCutscene(_nextSequence.cutsceneName);
    }

    public void StartCutsceneSequence()
    {
        //MusicManager.Instance.startSequence = true;
        //MusicManager.Instance.StartSequence();
        MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.INSTANT_SWITCH);
        //MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.QUEUE_SWITCH);
    }


    public void PrepareForEndOfBossFight()
    {
        ConductorManager.Instance.RemoveController(this, MusicManager.TransitionType.QUEUE_SWITCH);
    }


    private void OnDisable()
    {
        EventManager.Instance.OnBeat -= CheckBeat;
        EventManager.Instance.OnBossPhaseChange -= PhaseChange;
    }
}
