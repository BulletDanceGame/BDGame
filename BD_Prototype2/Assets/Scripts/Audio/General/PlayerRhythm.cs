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

public class PlayerRhythm : MonoBehaviour
{
    public static PlayerRhythm Instance;

    public double offsetVisuals;
    public double offsetSwing;
    public double offsetDash;


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
    

    //for hitting 
    [SerializeField] private double _perfectHitTime;
    [SerializeField] private double _okayHitTime;


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
        if (Input.GetKeyDown(KeyCode.P))
        {
            diff = 0;
            diffNr = 0;
            max = -999;
            min = 999;
        }

    }


    public void ClearBeats()
    {
        print("clear");
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
            int index = _playerBeats.IndexOf(beat) + 1;
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
        //if (_timesToHit.Count > 0)
        //{
        //    int lastBeatInPlayerBeats = _playerBeats[^1];
        //    for (int i = MusicManager.Instance.lastBeatOfSequence + 1; i < lastBeatInPlayerBeats + 1; i++)
        //    {
        //        if (_playerBeats.Contains(i))
        //        {
        //            _playerBeats.Remove(i);
        //        }
        //    }
        //}




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

        if (Math.Abs(timeDiff) <= _perfectHitTime)
        {
            timing = BeatTiming.PERFECT;
        }
        else if (Math.Abs(timeDiff) <= _okayHitTime)
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

    double max = -999;
    double min = 999;

    public double GetHitDelay(ButtonInput input)
    {
        if (_timesToHit.Count == 0)
        {
            return 9999;
        }

        double time = Time.timeAsDouble;
        double offset = (input == ButtonInput.swing) ? offsetSwing : offsetDash;

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
        print("delay: " + timeDiff + " time " + Time.timeAsDouble);

        diff += timeDiff;
        diffNr++;
        //print("average " + diff / diffNr);

        max = Math.Max(timeDiff, max);
        min = Math.Min(timeDiff, min);
        //print("max: " + max + " and min: " + min);

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
