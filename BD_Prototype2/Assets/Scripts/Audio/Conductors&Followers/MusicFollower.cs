using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MusicFollower : MonoBehaviour
{
    public enum Follower { Solo, Group}
    [SerializeField] private Follower _follower;

    [Header("Solo")]
    [SerializeField] private Movelist _movelist;


    [Header("Group")]
    [SerializeField] private Movelist[] _movelists;



    [Space]
    //array that is used to create beat actions from the inspector
    [SerializeField] private BeatAction[] _beatActions;

    //list that keeps track of what actions should happen on which beats for the currently used map
    //private List<(int, BeatAction)> _currentBeatList = new List<(int, BeatAction)>();
    private List<(int, Note)> _currentNoteList = new List<(int, Note)>();
    private int _beatIndex = 0;
    //and this shows what the last beat was on the use of the current map
    private int _beatAtStartOfThisMap = 0;


    [SerializeField] private FollowSequence[] _sequences;
    private FollowSequence _currentSequence;


    string na;


    public void Start()
    {
        na = gameObject.name;
        SetUp();
    }


    public void SetUp()
    {
        BeatMapReader.ReadBeatMapsFollower(_sequences);

        EventManager.Instance.OnBeat += CheckBeat;
        EventManager.Instance.OnNewSong += ChooseSequence;
    }
    

    /// <summary> Checks if the enemy should perform an action on this beat </summary> 
    public void CheckBeat(int beat)
    {
        if (_currentNoteList.Count == 0 || _beatIndex >= _currentNoteList.Count)
            return;

        //get current beat for this map
        int currentBeat = beat - _beatAtStartOfThisMap;

        //check if action should be performed
        Note action = null;
        if (currentBeat == _currentNoteList[_beatIndex].Item1)
        {
            action = _currentNoteList[_beatIndex].Item2;
            _beatIndex++;
        }
        else
        {
            return;
        }
        

        if (_follower == Follower.Solo)
        {
            _movelist.Action(action);
        }
        else if (_follower == Follower.Group)
        {

            if (action.movelistsToTrigger == Note.MovelistsToTrigger.All)
            {
                foreach (Movelist m in _movelists)
                {
                    if (m != null)
                    {
                        if (m.gameObject.activeSelf)
                        {
                            m.Action(action);
                        }
                    }
                }
            }
            else if (action.movelistsToTrigger == Note.MovelistsToTrigger.AllSpecified)
            {
                foreach (Movelist m in action.specifiedMovelists)
                {
                    if (m != null)
                    {
                        if (m.gameObject.activeSelf)
                        {
                            m.Action(action);
                        }
                    }
                }
            }
            else
            {
                List<Movelist> movelists = new List<Movelist>();


                if (action.movelistsToTrigger == Note.MovelistsToTrigger.RandomizeBetweenSpecified)
                {
                    movelists = action.specifiedMovelists.ToList<Movelist>();

                }
                else if (action.movelistsToTrigger == Note.MovelistsToTrigger.RandomizeBetweenAll)
                {
                    movelists = _movelists.ToList<Movelist>();
                }

                for (int i = 0; i < action.amountToRandomizeBetween; i++)
                {
                    if (movelists.Count == 0)
                    {
                        break;
                    }
                    int rand = Random.Range(0, movelists.Count);

                    if (movelists[rand] != null)
                    {
                        if (movelists[rand].gameObject.activeSelf)
                        {
                            movelists[rand].Action(action);
                        }
                        else i--;
                    }
                    else 
                        i--; //remove them from lists for optimization


                    movelists.RemoveAt(rand);
                }
            }





        }

    }

    public void ChooseSequence(AK.Wwise.Event newSong)
    {
        //check if there's a specific song it should follow
        for (int i = 0; i < _sequences.Length; i++)
        {
            if (_sequences[i].followSpecificSong)
            {
                if (_sequences[i].songToFollow.Name == newSong.Name)
                {
                    _currentSequence = _sequences[i];
                    _currentNoteList = _currentSequence.noteList;
                    _beatIndex = 0;
                    _beatAtStartOfThisMap = MusicManager.Instance.currentBeat;

                    return;
                }
            }
            
        }

        //if not, lets get a nonspecific sequence
        //find all nonspecific sequences
        List<int> indexes = new List<int>();
        for (int i = 0; i < _sequences.Length; i++)
        {
            if (_sequences[i].followSpecificSong == false)
            {
                indexes.Add(i);
            }
        }
        //randomize between the nonspecific sequences
        if (indexes.Count > 0)
        {
            int index = indexes[Random.Range(0, indexes.Count)];

            _currentSequence = _sequences[index];
            _currentNoteList = _currentSequence.noteList;
            _beatIndex = 0;
            _beatAtStartOfThisMap = MusicManager.Instance.currentBeat;

            print(index);
            return;
        }


        //if nothing fits, gets empty stuff
        _currentSequence = new FollowSequence();
        _currentNoteList = new List<(int, Note)>();
        _beatIndex = 0;
        _beatAtStartOfThisMap = MusicManager.Instance.currentBeat;
    }




    public void OnDestroy()
    {
        print("bitch "+ na);
        EventManager.Instance.OnBeat -= CheckBeat;
        EventManager.Instance.OnNewSong -= ChooseSequence;
    }

}




