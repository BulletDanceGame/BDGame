using System;
using UnityEngine;

public class EnvironmentConductor : MusicConductor
{
    public enum SequenceType { Roaming, Battle, BattleOutro }

    [Serializable]
    public struct SequenceGroup
    {
        public string name;
        public SequenceType type;
        public MusicSequence[] sequences;
        public string sequencePlayedFirst;
        public bool neverPlayFirstSequenceAgain;
    }
    [SerializeField] SequenceGroup[] _sequenceGroups;

    private bool _active;

    [SerializeField] private SequenceType _startingSequenceType;
    
    public static SequenceType currentSequenceType { get; private set; }


    private void Start()
    {
        SplitSequenceWeight();

    }


    public override void WhenTakingControl()
    {
        currentSequenceType = _startingSequenceType;
    }


    private void SplitSequenceWeight()
    {
        for (int p = 0; p < _sequenceGroups.Length; p++)
        {
            SequenceGroup group = _sequenceGroups[p];

            for (int seq = 0; seq < group.sequences.Length; seq++)
            {
                //if a specific one shouldnt be played first, split equally
                if (group.sequencePlayedFirst == "")
                {
                    group.sequences[seq].weight = 100.0f / group.sequences.Length;
                }
                //if theres one that should be played first
                else
                {
                    if (group.sequencePlayedFirst == group.sequences[seq].name)
                    {
                        group.sequences[seq].weight = 100.0f;
                    }
                    else
                    {
                        group.sequences[seq].weight = 0f;
                    }
                }
            }
        }
    }



    public override MusicSequence GetNextSequence()
    {
        int sequenceIndex = 0;
        int groupIndex = 0;

        for (int i = 0; i < _sequenceGroups.Length; i++)
        {
            if (_sequenceGroups[i].type == currentSequenceType)
            {
                groupIndex = i;
                break;
            }
        }
        MusicSequence[] sequences = _sequenceGroups[groupIndex].sequences;

        //get the intro that shouldnt be played again
        int introSequenceIndex = -1;
        if (_sequenceGroups[groupIndex].neverPlayFirstSequenceAgain)
        {
            for (int i = 0; i < sequences.Length; i++)
            {
                if (sequences[i].name == _sequenceGroups[groupIndex].sequencePlayedFirst)
                {
                    introSequenceIndex = i;
                }
            }
        }

        if (sequences.Length > 1)
        {
            //randomize the next seqeunce based on weight
            float value = UnityEngine.Random.Range(0.0f, 100.0f);
            float minValueInRange = 0.0f;
            float maxValueInRange = 0.0f;
            sequenceIndex = 0;

            for (int i = 0; i < sequences.Length; i++)
            {
                maxValueInRange = minValueInRange + sequences[i].weight;

                //if randomized value is in the range of the current sequences weight, it is the chosen one
                if (value > minValueInRange && value <= maxValueInRange)
                {
                    sequenceIndex = i;
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
            if (introSequenceIndex != -1 && sequences.Length > 2) fuck--;

            float splitWeight = sequences[sequenceIndex].weight / fuck;
            sequences[sequenceIndex].weight = 0.0f;

            if (_sequenceGroups[groupIndex].neverPlayFirstSequenceAgain && sequences.Length == 2)
            {
                int theotherone = (introSequenceIndex == 0) ? 1 : 0;
                sequences[theotherone].weight = splitWeight;
            }
            else
            {
                for (int i = 0; i < sequences.Length; i++)
                {
                    if (i != sequenceIndex && i != introSequenceIndex)
                    {
                        sequences[i].weight += splitWeight;
                    }
                }
            }

            
        }
        else
        {
            sequenceIndex = 0;
        }

        _nextSequence = sequences[sequenceIndex];
        return _nextSequence;
    }



    public static void SwitchSequenceType(SequenceType sequenceType, MusicManager.TransitionType transition)
    {
        currentSequenceType = sequenceType;

        MusicManager.Instance.SwitchMusic(transition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (_active == false)
            {
                _active = true;

                MusicManager.TransitionType transition;
                if (ConductorManager.Instance.GetCurrentController() == null) 
                { 
                    transition = MusicManager.TransitionType.INSTANT_SWITCH; 
                }
                else 
                { 
                    transition = MusicManager.TransitionType.QUEUE_SWITCH; 
                }

                ConductorManager.Instance.AddController(this, transition);
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (_active == true)
            {
                _active = false;
                ConductorManager.Instance.RemoveController(this, MusicManager.TransitionType.QUEUE_SWITCH);
            }
        }
            
    }


    public void StartForCutscene()
    {
            if (_active == false)
            {
                _active = true;

                MusicManager.TransitionType transition;
                if (ConductorManager.Instance.GetCurrentController() == null) 
                { 
                    transition = MusicManager.TransitionType.INSTANT_SWITCH; 
                }
                else 
                { 
                    transition = MusicManager.TransitionType.QUEUE_SWITCH; 
                }

                ConductorManager.Instance.AddController(this, transition);
            }        
    }

}


