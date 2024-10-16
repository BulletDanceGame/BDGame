using System;
using System.Collections.Generic;
using UnityEngine;

public enum BeatTiming
{
    PERFECT,
    GOOD,
    BAD,
    NONE
}

public enum ButtonInput { swing, dash, none };

public enum RhythmDifficulty { normal, easy, removed};

public class PlayerRhythm : MonoBehaviour
{
    public static PlayerRhythm Instance;

    public RhythmDifficulty rhythmDifficulty { get; set; }

    public double perfectHitTime { get; set; }
    public double okayHitTime { get; set; }


    public double offsetVisuals { get; set; }
    public double offsetSwing { get; set; }
    public double offsetDash { get; set; }


    private double _timeOfSequence = 0;
    private double _startTime = 0;


    /// <summary> How long we anticipate future beats </summary>
    private int _anticipation = 16;


    private List<double> _timesToHit = new List<double>();
    private List<(double, int)> _times = new List<(double,int)>();

    [Serializable]
    public class VisualUpdate
    {
        public int anticipation;
        public List<int> beats = new List<int>();
        public List<double> timesToUpdate = new List<double>();
    }
    private VisualUpdate[] visualUpdates = new VisualUpdate[5];
    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        visualUpdates[0] = new VisualUpdate(); visualUpdates[0].anticipation = 0;
        visualUpdates[1] = new VisualUpdate(); visualUpdates[1].anticipation = 4;
        visualUpdates[2] = new VisualUpdate(); visualUpdates[2].anticipation = 6;
        visualUpdates[3] = new VisualUpdate(); visualUpdates[3].anticipation = 8;
        visualUpdates[4] = new VisualUpdate(); visualUpdates[4].anticipation = 12;

        SaveData data = SaveSystem.Instance.GetData();
        offsetSwing = data.swingOffset;
        offsetDash = data.dashOffset;
        offsetVisuals = data.visualOffset;

        rhythmDifficulty = (RhythmDifficulty)data.rhythmDifficulty;
        if (rhythmDifficulty == RhythmDifficulty.normal)
        {
            perfectHitTime = 0.06;
            okayHitTime = 0.12;
        }
        else if (rhythmDifficulty == RhythmDifficulty.easy)
        {
            perfectHitTime = 0.1;
            okayHitTime = 0.2;
        }
    }



    private void Update()
    {
        RemoveOldBeats();

        UpdateVisuals();

        if (_times.Count > 0)
        {
            if (Time.timeAsDouble >= _times[0].Item1)
            {
                EventManager.Instance.PlayerRhythmBeat(_times[0].Item2);
                _times.RemoveAt(0);
            }
        }
        


        //testing
        if (Input.GetKeyDown(KeyCode.O))
        {
            diff = 0;
            diffNr = 0;
            max = -999;
            min = 999;
        }

    }


    public void ClearBeats()
    {
        _timesToHit.Clear();
        _times.Clear();

        _timeOfSequence = 0;

        foreach (VisualUpdate v in visualUpdates)
        {
            v.timesToUpdate.Clear();
            v.beats.Clear();
        }

    }




    public void PrepareBeatMap(MusicSequence nextSequence, double delay, bool cut = false)
    {
        List<int> _playerBeats = new List<int>();
        BeatMapReader.ReadBeatMapPlayer(nextSequence.sheet, _playerBeats, MusicManager.Instance.lastBeatOfSequence);


        double spb = 60.0/(nextSequence.bpm*2);

        //times
        //starttime changes because of possible different bpm
        if (cut == false)
        {
            _startTime = Time.timeAsDouble + _timeOfSequence + delay;
        }


        print("starttime " + _startTime);
        print("starttime t " + Time.timeAsDouble);
        print("starttime s " + _timeOfSequence);
        print("starttime d " + delay);
        _timeOfSequence = nextSequence.duration * spb;

        int startBeat = MusicManager.Instance.lastBeatOfSequence + 1;

        foreach (int beat in _playerBeats)
        {
            //for checking swings and dashes
            _timesToHit.Add(_startTime + (spb * (beat - startBeat)));
            print("hit " + (beat - startBeat) + " " + _timesToHit[^1]);

            //for times
            //needs to check for the last one better
            //unnesecary for now
            int index = _playerBeats.IndexOf(beat);
            if (index != _playerBeats.Count)
            {
                (double, int) a = (_startTime + (spb * (beat - startBeat) + offsetVisuals), _playerBeats[index] - beat);
                _times.Add(a);
            }

            foreach (VisualUpdate v in visualUpdates)
            {
                int a = beat - v.anticipation;
                if (a >= 0)
                {
                    double b = _startTime + (spb * (a - startBeat)) + offsetVisuals;// + 0.1f;
                    v.timesToUpdate.Add(b);
                    v.beats.Add(beat);

                    //print("added " + beat + " " + v.anticipation + " " + b);
                }
            }
        }

        _playerBeats.Clear();

    }


    /// <summary> Removes prepared beats for the past next sequence </summary>
    public void RemoveOldPreparedBeats()
    {
        
        for (int i = _timesToHit.Count - 1; i > -1; i--)
        {
            if (_timesToHit[i] > _startTime)
            {
                _timesToHit.RemoveAt(i);
            }
            else
            {
                break;
            }
        }


        for (int i = _times.Count - 1; i > -1; i--)
        {
            if (_times[i].Item2 > MusicManager.Instance.lastBeatOfSequence)
            {
                _times.RemoveAt(i);
            }
            else
            {
                break;
            }
        }


        foreach (VisualUpdate v in visualUpdates)
        {
            for (int i = v.beats.Count-1; i > -1; i--)
            {
                if (v.beats[i] > MusicManager.Instance.lastBeatOfSequence)
                {
                    v.timesToUpdate.RemoveAt(i);
                    v.beats.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

        }
    }




    //VISUALS AND HITS
    private void UpdateVisuals()
    {
        foreach (VisualUpdate v in visualUpdates)
        {
            if (v.timesToUpdate.Count == 0)
            {
                continue;
            }

            if (Time.timeAsDouble >= v.timesToUpdate[0])
            {
                EventManager.Instance.BeatForVisuals( v.anticipation, (float)MusicManager.Instance.secondsPerBeat, v.beats[0]);
                v.timesToUpdate.RemoveAt(0);
                v.beats.RemoveAt(0);
            }
        }
                

    }

    public void UpdateOffsetVisuals(double newOffset)
    {

        double change = newOffset - offsetVisuals;

        foreach (VisualUpdate v in visualUpdates)
        {
            for( int t = 0; t < v.timesToUpdate.Count; t++)
            {
                v.timesToUpdate[t] += change;
            }

        }

        for (int t = 0; t < _times.Count; t++)
        {
            (double, int) time = _times[t];
            time.Item1 += change;
            _times[t] = time;
        }

        offsetVisuals = newOffset;
    } 


    public BeatTiming GetBeatTiming(ButtonInput input)
    {
        double timeDiff = GetHitDelay(input);

        BeatTiming timing;

        if (Math.Abs(timeDiff) <= perfectHitTime)
        {
            timing = BeatTiming.PERFECT;
        }
        else if (Math.Abs(timeDiff) <= okayHitTime)
        {
            timing = BeatTiming.GOOD;
        }
        else
        {
            timing = BeatTiming.BAD;
        }

        return timing;
    }

    //for calibration testing
    double diff;
    int diffNr;
    bool haveAlerted;
    double max = -999;
    double min = 999;

    public double GetHitDelay(ButtonInput input)
    {
        if (rhythmDifficulty == RhythmDifficulty.removed)
        {
            return 0;
        }

        if (_timesToHit.Count == 0)
        {
            return 9999;
        }

        double time = Time.timeAsDouble;
        double offset = (input == ButtonInput.swing) ? offsetSwing :  ( 
                        (input == ButtonInput.dash)  ? offsetDash  : 0);

        double a = double.MaxValue;
        int index = 0;
        for (int i = _timesToHit.Count - 1; i >= 0; i--)
        {
            double b = Math.Abs(time - (_timesToHit[i] + offset));

            if (b < a)
            {
                index = i;
                a = b;
            }
        }

        double timeDiff = time - (_timesToHit[index] + offset);

        if ((input == ButtonInput.swing || input == ButtonInput.dash) && SceneManager.Instance._currentScene != SceneManager.Scenes.Menu)
        {

            if (Math.Abs(timeDiff) < MusicManager.Instance.secondsPerBeat)
            {
                print("delay: " + timeDiff + " time " + Time.timeAsDouble);
    
                diff += timeDiff;
                diffNr++;

                max = Math.Max(timeDiff, max);
                min = Math.Min(timeDiff, min);
                //print("max: " + max + " and min: " + min);

                if (diffNr == 20)
                {
                    diff -= max;
                    diff -= min;

                    double average = diff / diffNr;
                    print("average " + average);

                    if (Math.Abs(average) > 0.05)
                    {
                        double n = (double)Math.Round((decimal)average, 2);
                        print("average CHANGE to " + n);
                        offsetSwing += n;
                        offsetDash += n;
                        SaveSystem.Instance.GetData().swingOffset = offsetSwing;
                        SaveSystem.Instance.GetData().dashOffset = offsetDash;
                        SaveSystem.Instance.Save();

                        if (!haveAlerted)
                        {
                            haveAlerted = true;
                            EventManager.Instance.CalibrationAlert((offsetSwing * 1000) + "ms");

                        }
                    }

                    diff = 0;
                    diffNr = 0;
                    max = -999;
                    min = 999;
                }
            }

            
        }


        // if late > delay(offset) is positive
        // if early > delay(offset) is negative

        return timeDiff;
    }



    private void RemoveOldBeats()
    {
        if (_timesToHit.Count > 0)
        {
            if (_timesToHit[0] < Time.timeAsDouble - 5)
            {
                _timesToHit.RemoveAt(0);
            }
        }

        foreach (VisualUpdate v in visualUpdates)
        {
            if (v.timesToUpdate.Count > 0)
            {
                if (v.timesToUpdate[0] < Time.timeAsDouble - 5)
                {
                    v.timesToUpdate.RemoveAt(0);
                    v.beats.RemoveAt(0);
                }
            }
        }
    }


}
