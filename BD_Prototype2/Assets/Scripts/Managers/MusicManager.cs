using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    //if the music manager has been activated by a conductor
    private bool _isActive;

    //startupwait
    private bool _waitBeforeStartUp = true;
    private float _waitBeforeStartUpTimer = 2f;
    private bool _startUpIsInvoked = false;


    //needed for wwise
    public GameObject bankPrefab;


    //TRANSITIONS BETWEEN SONGS
    public enum TransitionType { QUEUE_SWITCH, INSTANT_SWITCH, FADE_SWITCH, QUEUE_STOP, INSTANT_STOP, FADE_STOP }

    private bool _isSoftlyStopping;
    private bool _isSoftlyCutting;
    private float _softStopDuration = 2;
    private float _softStopTimer;


    //current songs and sequences
    private AK.Wwise.Event _currentSong;
    private MusicSequence _nextSequence;
    public MusicSequence _currentSequence { get; private set; }


    //General beat and time information
    public int currentBeat { get; private set; } = 0;
    public int lastBeatOfSequence { get; private set; }


    public double secondsPerBeat { get; set; }
    public int maxFPS;
    private double framesPerBeat; //this should theoretically be able to be a int, but as its made by 2 doubles its a double for safety
    private int framerate;
    private int frameDelay;

    [HideInInspector] public bool speedingUpInCutscene;


    //if Song should be kept playing on switch
    uint ignoreID;



    double frameDuration = 0;

    [HideInInspector] public bool startSequence;
    private bool prepareNextSequence;


    private int sequenceDuration = 0;

    private double totalDelay = 0;
    private double lastFrameTime = 0;
    private double audioDelay = 0;

    bool isDestroyed = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Instantiate(bankPrefab, transform);
        }
        else
        {
            Destroy(gameObject);
            return;
        }


    }


    bool checkFrames = false;
    int frames = 0;


    private void Update()
    {
        if (checkFrames)
        {
            frames++;
        }


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
            }
        }

        if (_isSoftlyStopping)
        {
            SoftlyStopping();
        }

                

        if (playing)
        {

            songTimer++;
            if (speedingUpInCutscene) { songTimer += 3; }
            //print("songtimer " + songTimer + " - time " + Time.timeAsDouble);
            //print("goal " + (timedBeats * framesPerBeat - endTime) + " off " + (songTimer - (timedBeats * framesPerBeat - endTime)));

            double delay = 0;
            if (lastFrameTime != 0)
            {
                double frameTimer = Time.realtimeSinceStartup - lastFrameTime;
                delay = frameTimer - frameDuration;
            }
            totalDelay += delay;
                
            lastFrameTime = Time.realtimeSinceStartup;
            //print("skip unitycheck " + totalDelay);

            int f = (int)(totalDelay / frameDuration);

            if (Mathf.Abs(f) >= 1)
            {
                //print("skip unity " + f + " time " + totalDelay);
                songTimer += f;
                totalDelay -= frameDuration * f;
            }


            //on beat (or endTime-frames before beat on the last one of a song)
            if (songTimer >= timedBeats * framesPerBeat - endTime)
            {
                timedBeats++;

                //save from pausing
                double delayTimer = Time.realtimeSinceStartup - lastTimer;
                lastTimer = Time.realtimeSinceStartup;
                //print("beat " + timedBeats + " time " + Time.timeAsDouble + " delay " + delayTimer);

                if (timedBeats == sequenceDuration+1) //DURATION
                {
                    StartSequence();

                    endTime = 0;
                }
                else //else if <duration+1
                {
                    currentBeat++;
                    EventManager.Instance.Beat(currentBeat);

                    if (timedBeats == sequenceDuration)
                    {
                        endTime = frameDelay;

                    }
                }
            }
        }

        if (prepareNextSequence)
        {
            PrepareNextSequence(frameDuration*frameDelay);
            prepareNextSequence = false;
        }
        if (startSequence)
        {
            StartSequence();
            startSequence = false;
        }

        AkSoundEngineController.Instance.LateUpdate();
    }

    private void OnDestroy()
    {
        isDestroyed = true;
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
                //PrepareNextSequence(0); //well this is fucked lol

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
                PrepareNextSequence(0, true);
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
                RTPCManager.Instance.SetAttributeValue("VOLUME", 50, _softStopDuration, RTPCManager.CurveTypes.linear);
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

        PlayerRhythm.Instance.ClearBeats();

        _currentSong.Stop(gameObject);
    }

    /// <summary> Prepares the next sequence and song by taking it from the current Controller </summary>
    private void PrepareNextSequence(double delay, bool cut = false)
    {
        MusicConductor controller = ConductorManager.Instance.GetCurrentController();
        if (controller == null)
        {
            return;
        }

        _nextSequence = controller.GetNextSequence();

        SetFPS();

        if (PlayerRhythm.Instance)
        {
            PlayerRhythm.Instance.PrepareBeatMap(_nextSequence, delay + audioDelay, cut);
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


        //fps check
        int supposedFrames = (int)framesPerBeat * sequenceDuration;
        //print("aaa framecount " + frames + " should have been " + supposedFrames);
        if (frames < supposedFrames *0.9f)
        {
            //print("aaa warning fps is too low you should switch");

            double consider = frames / (secondsPerBeat * sequenceDuration);
            //print("aaa consider " + consider);

            double b = _currentSequence.bpm * 2.0 / 60.0;
            double c = 0;

            for (int i = 1; i < 11; i++)
            {
                c = b * i;
                if (c == (int)c)
                {
                    print("aaa c " + c);
                    break;
                }
            }

            double m = consider % c;
            //print("aaa m " + m);

            consider -= m;
            //print("aaa consider " + consider);


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
            print("start " + Time.timeAsDouble);
            print("sequence " + _currentSequence.name);
            startDelay = Time.timeAsDouble;

            //secondsPerBeat = 60.0 / (_currentSequence.bpm * 2);


            Application.targetFrameRate = framerate;
            framesPerBeat = secondsPerBeat / frameDuration;
            //print("fpb " + framesPerBeat);
            sequenceDuration = _currentSequence.duration;

            AkSoundEngineController.Instance.starting = true;
            AkSoundEngineController.Instance.frameDelay = frameDelay;
            AkSoundEngineController.Instance.frameDuration = frameDuration;

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
        if(isDestroyed)
            return;
        if (gameObject == null) { return; }

        

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
            print("nani te fuck");
            StartCoroutine(OnEntry());
        }
        //EXIT
        else if (in_type == AkCallbackType.AK_MusicSyncExit)
        {
            //print("totaldelay " + totalDelay);
            //totalDelay = 0;
            //StartCoroutine(SongExit(info));

            //playing = false;
        }


    }

    double startDelay = 0;
    double lastSequenceDelay = 0;

    bool playing = false;
    double songTimer = 0;
    double endTime = 0;
    double lastTimer = 0;
    int timedBeats = 1;

    private void SetFPS()
    {
        secondsPerBeat = 60.0 / (_nextSequence.bpm * 2);

        double lowestFPS = 0.5;
        int i = 0;
        while (lowestFPS % 1 != 0)
        {
            i++;
            lowestFPS = i / secondsPerBeat;
            if (i == _currentSequence.bpm)
            {
                print("couldnt find an fps");
                break;
            }
        }
        //print("lowest " + lowestFPS + "plues " + i);
        framerate = (int)(maxFPS / lowestFPS) * (int)lowestFPS;
        //print("new fps " + framerate);
        frameDuration = 1.0 / framerate;


        frameDelay = framerate / 10; //so the start is roughly 100ms delayed, might need to be calculated differently
        
        
        audioDelay = frameDuration + (512.0 / 48000.0) * 4.0;
        //print("audio " + audioDelay);
    }


    IEnumerator OnEntry()
    {

        AkSoundEngineController.Instance.starting = false;

        currentBeat++;
        EventManager.Instance.Beat(currentBeat);

        print("start entry " + Time.timeAsDouble);
        startDelay = Time.timeAsDouble - startDelay;
        print("startdelay " + startDelay + " fr " + frames);
        lastSequenceDelay = Time.timeAsDouble - lastSequenceDelay;
        //print("lastdelay " + lastSequenceDelay);
        lastSequenceDelay = Time.timeAsDouble;

        //checkFrames = false;

        PrepareNextSequence(0);//move to after offset

        playing = true;
        songTimer = 0;
        timedBeats = 1;
        double delayTimer = Time.realtimeSinceStartup - lastTimer;
        lastTimer = Time.realtimeSinceStartup;
        //print("fixed, beat " + timedBeats + " time " + Time.timeAsDouble + " delay " + delayTimer);

        


        yield return null;
    }





}