using BulletDance.Audio;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    //if the music manager has been activated by a conductor
    private bool _isActive;

    //startupwait
    private bool _waitBeforeStartUp = true;
    private float _waitBeforeStartUpTimer;
    private bool _startUpIsInvoked = false;


    //needed for wwise
    public GameObject bankPrefab;


    //TRANSITIONS BETWEEN SONGS
    public enum TransitionType { QUEUE_SWITCH, INSTANT_SWITCH, FADE_SWITCH, QUEUE_STOP, INSTANT_STOP, FADE_STOP }

    private bool _isSoftlyStopping;
    private bool _isSoftlyCutting;
    private float _softStopDuration = 1;
    private float _softStopTimer;


    //current songs and sequences
    private AK.Wwise.Event _currentSong;
    public MusicSequence _nextSequence { get; private set; }
    public MusicSequence _currentSequence { get; private set; }


    //General beat and time information
    public int currentBeat { get; private set; } = 0;
    public int lastBeatOfSequence { get; private set; }


    public double secondsPerBeat { get; set; }
    public int maxFPS { get; set; }
    private double framesPerBeat; //this should theoretically be able to be a int, but its calculated by 2 doubles so its a double for safety
    private int currentFramerate;
    private double currentFrameDuration = 0;
    private int currentFrameDelay;


    private int nextFramerate;
    private double nextFrameDuration = 0;
    private int nextFrameDelay;
    private double nextAudioDelay = 0;



    [HideInInspector] public bool speedingUpInCutscene;


    //if Song should be kept playing on switch
    uint ignoreID;




    [HideInInspector] public bool startSequence;
    private bool prepareNextSequence;


    private int sequenceDuration = 0;

    private double totalDelay = 0;
    private double lastFrameTime = 0;

    bool isDestroyed = false;

    bool checkFrames = false;
    int frames = 0;

    bool cantCalculateSpeed;

    private void Awake()
    {

        Instance = this;
        Instantiate(bankPrefab, transform);

    }


    private void Start()
    {
        _isActive = false;
        _waitBeforeStartUpTimer = 2.02f;
        _waitBeforeStartUp = true;
        _startUpIsInvoked = false;

        maxFPS = SaveSystem.Instance.GetData().maxFPS;

        EventManager.Instance.DisableInput();

        EventManager.Instance.OnPlayerDeath += OnPlayerDeath;
    }



    private void Update()
    {
        //start up timer
        if (_waitBeforeStartUp)
        {
            _waitBeforeStartUpTimer -= Time.deltaTime;
            if (_waitBeforeStartUpTimer < 0)
            {
                _waitBeforeStartUp = false;

                if (_startUpIsInvoked)
                {
                    SwitchMusic(TransitionType.INSTANT_SWITCH);
                }

                if (LoadingScreen.Instance)
                {
                    LoadingScreen.Instance.UnCover();
                }

                print("start wwise " + Time.timeAsDouble);

                EventManager.Instance.EnabableInput();
            }
            else
            {
                return;
            }
        }



        if (_isSoftlyStopping)
        {
            SoftlyStopping();
        }


        if (checkFrames)
        {
            frames++;
        }


        if (playing && !cantCalculateSpeed)
        {
            songTimer++;
            double frameDuration = currentFrameDuration;

            if (speedingUpInCutscene)
            {
                songTimer += 3;
                frameDuration *= 4;
            }


            //print("goal " + (timedBeats * framesPerBeat - endTime) + " off " + (songTimer - (timedBeats * framesPerBeat - endTime)));

            double delay = 0;

            if (lastFrameTime != 0)
            {
                double frameTimer = Time.timeAsDouble - lastFrameTime;
                delay = frameTimer - frameDuration;
            }
            totalDelay += delay;
                
            lastFrameTime = Time.timeAsDouble;

            int f = (int)(totalDelay / frameDuration);

            if (Mathf.Abs(f) >= 1)
            {
                //print("skip unity " + f + " time " + totalDelay);
                songTimer += f;
                if(speedingUpInCutscene) { songTimer += f * 3; }
                totalDelay -= frameDuration * f;
            }


            //on beat (or endTime-frames before beat on the last one of a song)
            if (songTimer >= timedBeats * framesPerBeat - endTime)
            {
                timedBeats++;

                //save from pausing
                double delayTimer = Time.timeAsDouble - lastTimer;
                lastTimer = Time.timeAsDouble;
                //print("beat " + timedBeats + " time " + Time.timeAsDouble);// + " delay " + delayTimer);
                //print("beat duration " + sequenceDuration);
                //print("beat fpb " + framesPerBeat);

                if (timedBeats == sequenceDuration+1) //DURATION
                {
                    StartSequence();

                    endTime = 0;
                }
                else // timedBeats < sequenceDuration+1
                {
                    currentBeat++;
                    EventManager.Instance.Beat(currentBeat);

                    if (timedBeats == sequenceDuration)
                    {
                        endTime = currentFrameDelay;
                    }
                }
            }
        }




        if (prepareNextSequence)
        {
            PrepareNextSequence(true);
            prepareNextSequence = false;
        }
        if (startSequence)
        {
            StartSequence();
            startSequence = false;
        }





        AkSoundEngineController.Instance.LateUpdate();
    }


    


    //transitions
    public void SwitchMusic (TransitionType transition)
    {
        if (!_isActive)
        {
            //Activate if not already done
            if (transition != TransitionType.QUEUE_STOP || transition != TransitionType.INSTANT_STOP || transition != TransitionType.FADE_STOP) 
            {
                //Dont start music until StartUpWait is done!
                if (_waitBeforeStartUp)
                {
                    if (!_startUpIsInvoked)
                    {
                        _startUpIsInvoked = true;
                    }
                    return;
                }

                _isActive = true;

                prepareNextSequence = true;
                startSequence = true;
            }
        }
        else
        {
            if (transition == TransitionType.INSTANT_STOP)
            {
                ResetMusicSystem();

                //"keep playing on switch"
                if (_currentSequence.keepPlayingOnSwitch == true)
                {
                    ignoreID = _currentSong.PlayingId;
                }
            }
            else if (transition == TransitionType.INSTANT_SWITCH)
            {
                ResetMusicSystem();

                //"keep playing on switch"
                if (_currentSequence.keepPlayingOnSwitch == true)
                {
                    ignoreID = _currentSong.PlayingId;
                }

                prepareNextSequence = true;
                startSequence = true;
            }
            else if (transition == TransitionType.QUEUE_SWITCH)
            {
                PlayerRhythm.Instance.RemoveOldPreparedBeats();
                PrepareNextSequence(false, true);
            }
            else if (transition == TransitionType.QUEUE_STOP)
            {
                PlayerRhythm.Instance.RemoveOldPreparedBeats();
            }
            else if (transition == TransitionType.FADE_SWITCH)
            {
                _isSoftlyCutting = true;
                _isSoftlyStopping = true;
                _softStopTimer = _softStopDuration;
                RTPCManager.Instance.SetAttributeValue("VOLUME", 50, _softStopDuration, RTPCManager.CurveTypes.linear);
            }
            else if (transition == TransitionType.FADE_STOP)
            {
                _isSoftlyStopping = true;
                _softStopTimer = _softStopDuration;
                RTPCManager.Instance.SetAttributeValue("VOLUME", 0, _softStopDuration, RTPCManager.CurveTypes.linear, "VOLUME____Menu");
            }
            
        }
    }


    //for transition
    private void SoftlyStopping()
    {
        _softStopTimer -= Time.deltaTime;
        if (_softStopTimer < 0)
        {
            _isSoftlyStopping = false;
            RTPCManager.Instance.ResetAttributeValue("VOLUME", 0, RTPCManager.CurveTypes.linear);

            if (_isSoftlyCutting)
            {
                ResetMusicSystem();
                prepareNextSequence = true;
                startSequence = true;
            }
            else
            {
                ResetMusicSystem();
                _isActive = false;
            }

        }

    }




    private void ResetMusicSystem()
    {
        currentBeat = 0;
        lastBeatOfSequence = 0;

        playing = false;
        songTimer = 0;
        timedBeats = 1;
        lastFrameTime = 0;
        endTime = 0;

        PlayerRhythm.Instance.ClearBeats();

        
        if (_currentSong != default)
            _currentSong.Stop(gameObject);
    }

    /// <summary> Prepares the next sequence and song by taking it from the current Controller </summary>
    private void PrepareNextSequence(bool frameDelay = false, bool cut = false)
    {
        MusicConductor controller = ConductorManager.Instance.GetCurrentController();
        if (controller == null)
        {
            return;
        }

        _nextSequence = controller.GetNextSequence();

        SetNextFPS();

        double delay = 0;
        if (frameDelay) delay = nextFrameDuration * nextFrameDelay;

        if (PlayerRhythm.Instance)
        {
            PlayerRhythm.Instance.PrepareBeatMap(_nextSequence, delay + nextAudioDelay, cut);
        }
        
    }


    /// <summary> Plays the song that has been prepared </summary>
    public void StartSequence()
    {

        //check if theres a conductor (for when music has ended and wanna restart)
        if (ConductorManager.Instance.GetCurrentController() == null)
        {
            return;
        }

        if (_currentSequence.replayInSameSequence == false)
        {
            if (_currentSong != null && _currentSequence.keepPlayingOnSwitch == false)
            {
                _currentSong.Stop(gameObject);
            }
            _currentSequence = _nextSequence;
            _currentSong = _currentSequence.song;
            _currentSong.Post(gameObject,
                (uint)AkCallbackType.AK_MusicSyncBeat + (uint)AkCallbackType.AK_MusicSyncExit + (uint)AkCallbackType.AK_MusicSyncEntry,
                MusicCallbacks);
            print("_start " + Time.timeAsDouble + " - sequence " + _currentSequence.name);
            startDelay = Time.timeAsDouble;

            //secondsPerBeat = 60.0 / (_currentSequence.bpm * 2);

            currentFramerate = nextFramerate;
            currentFrameDuration = nextFrameDuration;
            currentFrameDelay = nextFrameDelay;
            print("framedel " + currentFrameDelay);

            Application.targetFrameRate = currentFramerate;
            sequenceDuration = _currentSequence.duration;

            AkSoundEngineController.Instance.starting = true;
            AkSoundEngineController.Instance.frameDelay = currentFrameDelay;
            AkSoundEngineController.Instance.frameDuration = currentFrameDuration;

            checkFrames = true;
            frames = 0;

        }


        lastBeatOfSequence += _currentSequence.duration;

        ConductorManager.Instance.GetCurrentController().SequenceHasStarted();

        EventManager.Instance.NewSong(_currentSong);

    }



    /// <summary> Callbacks from songs from WWise internal stuff.
    /// Both for Beats and the End of Songs. </summary>
    private void MusicCallbacks(object in_cookie, AkCallbackType in_type, object in_info)
    {
        if (isDestroyed || gameObject == null)
            return;

        

        //seconds per beat
        AkMusicSyncCallbackInfo info = (AkMusicSyncCallbackInfo)in_info;
        //secondsPerBeat = info.segmentInfo_fBeatDuration;
        //if (speedingUpInCutscene) { secondsPerBeat *= 4; }//this bad

        //BEAT
        if (in_type == AkCallbackType.AK_MusicSyncBeat)
        {
            //beatDelay = Time.timeAsDouble - lastTime - secondsPerBeat;
            //lastTime = Time.timeAsDouble;
            //totalDelay += beatDelay;

            //print("beat " + currentBeat + " time " + Time.timeAsDouble + " + delay " + beatDelay);
            //StartCoroutine(Every(info));

        }
        //Entry
        else if (in_type == AkCallbackType.AK_MusicSyncEntry)
        {
            StartCoroutine(OnEntry());
        }
        //EXIT
        else if (in_type == AkCallbackType.AK_MusicSyncExit)
        {
            if (cantCalculateSpeed)
            {
                StartSequence();
                PrepareNextSequence();
            }
        }


    }

    double startDelay = 0;
    double lastSequenceDelay = 0;

    bool playing = false;
    double songTimer = 0;
    double endTime = 0;
    double lastTimer = 0;
    int timedBeats = 1;

    private void SetNextFPS()
    {


        secondsPerBeat = 60.0 / (_nextSequence.bpm * 2);

        double lowestFPS = 0.5;
        int i = 0;
        while (lowestFPS % 1 != 0)
        {
            i++;
            lowestFPS = i / secondsPerBeat;
            if (i == _nextSequence.bpm)
            {
                Debug.LogWarning("couldnt find an fps");
                break;
            }
        }
        nextFramerate = (int)(maxFPS / lowestFPS) * (int)lowestFPS;
        nextFrameDuration = 1.0 / nextFramerate;

        print("fps " + nextFramerate);


        nextFrameDelay = nextFramerate / 10; //so the start is roughly 100ms delayed
        
        
        nextAudioDelay = nextFrameDuration + (512.0 / 48000.0) * 4.0;
        //print("audio " + audioDelay);
    }


    double lastEntry = 0;

    IEnumerator OnEntry()
    {

        AkSoundEngineController.Instance.starting = false;

        currentBeat++;
        EventManager.Instance.Beat(currentBeat);

        framesPerBeat = secondsPerBeat / currentFrameDuration;
        //print("fpb " + framesPerBeat);


        print("_start entry yo " + Time.timeAsDouble);
        startDelay = Time.timeAsDouble - startDelay;
        print("_startdelay " + startDelay + " fr " + frames);
        print("_s between entries " + (Time.timeAsDouble-lastEntry));
        lastEntry = Time.timeAsDouble;
        //print("_start finish " + (Time.timeAsDouble + secondsPerBeat*_currentSequence.duration));

        lastSequenceDelay = Time.timeAsDouble - lastSequenceDelay;
        //print("lastdelay " + lastSequenceDelay);
        lastSequenceDelay = Time.timeAsDouble;

        //checkFrames = false;

        playing = true;
        songTimer = 0;
        timedBeats = 1;
        double delayTimer = Time.timeAsDouble - lastTimer;
        lastTimer = Time.timeAsDouble;
        //print("fixed, beat " + timedBeats + " time " + Time.timeAsDouble + " delay " + delayTimer);



        PrepareNextSequence();


        yield return null;
    }



    void OnPlayerDeath()
    {
        cantCalculateSpeed = true;
    }


    private void OnDestroy()
    {
        isDestroyed = true;
        cantCalculateSpeed = false;
        EventManager.Instance.OnPlayerDeath -= OnPlayerDeath;
    }

    private void OnDisable()
    {
        isDestroyed = true;
        cantCalculateSpeed = false;
        EventManager.Instance.OnPlayerDeath -= OnPlayerDeath;
    }
}
