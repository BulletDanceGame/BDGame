using UnityEngine;

public class MainMenuConductor : MusicConductor
{
    public static MainMenuConductor Instance;

    //normal music
    [SerializeField] private MusicSequence[] _menuSequences;
    private int _index;
    
    //trailer music
    [SerializeField] private MusicSequence _trailerSequence;
    private bool _useTrailerMusic;
    private bool _isTrailerPlaying;
    [SerializeField] private MainMenuSettings _trailerScript;

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        ConductorManager.Instance.AddController(this, MusicManager.TransitionType.INSTANT_SWITCH);
    }


    public override MusicSequence GetNextSequence()
    {
        if (_useTrailerMusic)
        {
            _nextSequence = _trailerSequence;
        }
        else
        {
            _index = Random.Range(0, _menuSequences.Length);
            _nextSequence = _menuSequences[_index];
        }
        
        return _nextSequence;
    }



    public override void SequenceHasStarted()
    {
        if (_useTrailerMusic)
        {
            _trailerScript.PlayTrailer();
            _isTrailerPlaying = true;
        }

        if (_isTrailerPlaying && !_useTrailerMusic)
        {
            _trailerScript.StopTrailer();
            _isTrailerPlaying = false;
        }
    }


    public void StopMenuMusic()
    {
        MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.FADE_STOP);
    }

    public void SwitchToTrailerMusic()
    {
        _useTrailerMusic = true;
        MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.FADE_SWITCH);
    }

    public void SwitchToMenuMusic()
    {
        _useTrailerMusic = false;
        MusicManager.Instance.SwitchMusic(MusicManager.TransitionType.FADE_SWITCH);
    }
}



