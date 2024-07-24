using BulletDance.Audio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class CalibrateInput : MonoBehaviour
{
    public ButtonInput input;

    [SerializeField] private CalibrationMenu menu;

    [SerializeField] private GameObject delayMarkerPrefab;
    private List<GameObject> delayMarkers = new List<GameObject>();
    [SerializeField] private GameObject averageDelayMarker;
    [SerializeField] private TextMeshProUGUI averageDelayText;
    [SerializeField] private GameObject offsetMarker;
    [SerializeField] private TextMeshProUGUI offsetText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject resetButton;

    private double secondsPerBeat = .0;

    private double averageDelay = .0;
    private List<double> delays = new List<double>();


    [SerializeField] private Animator anim;


    public GameObject ballPrefab;

    public ParticleSystem particles;


    GameObject currentBall = null;
    GameObject nextBall = null;


    [SerializeField] private TextMeshProUGUI averageText;
    [SerializeField] private TextMeshProUGUI consistencyText;

    [SerializeField] private Gradient textColor;

    private bool changedOffset;





    private void OnEnable()
    {
        EventManager.Instance.OnBeatForVisuals += PlayAnimations;

        double offset = GetOffset();
        string late = (offset >= 0) ? "LATE" : "EARLY";
        offsetText.text = Math.Abs(offset * 1000) + "ms " + late;
        offsetText.color = textColor.Evaluate((float)offset * 5f + 0.5f);
    }

    private void OnDisable()
    {

        EventManager.Instance.OnBeatForVisuals -= PlayAnimations;

        Destroy(currentBall);
        Destroy(nextBall);
    }


    //Called through PlayerInput
    void OnSwing()
    {
        if (input == ButtonInput.swing)
        {
            OnPress();
        }
        
    }

    void OnDash()
    {
        if (input == ButtonInput.dash)
        {
            OnPress();
        }
    }



    private void OnPress()
    {
        if (menu.canHit)// && currentBall != null)
        {

            double delay = PlayerRhythm.Instance.GetHitDelay(input);
            PlayerRhythm.Instance.GetComponent<PlayerSounds>().PlayerSwing(PlayerRhythm.Instance.GetBeatTiming(input), Vector2.zero); //ugh

            //do this once somewhere instead, have it as an event when switching songs?, just keep it a constant fit for the menu music?
            //*2 cause this DOESNT check for the 8th notes
            secondsPerBeat = MusicManager.Instance.secondsPerBeat * 2;

            //ball
            if (currentBall)
            {
                particles.transform.position = currentBall.transform.position;
                particles.Play();
                Destroy(currentBall);
            }

            //remove when changed offset
            if (changedOffset)
            {
                changedOffset = false;


                delays.Clear();

                averageDelay = 0;
                averageText.text = "";
                consistencyText.text = "";


                //markers
                for (int i = 0; i < delayMarkers.Count; i++)
                {
                    Destroy(delayMarkers[i]);
                }
                delayMarkers.Clear();

                averageDelayMarker.SetActive(false);
            }


            //Remove last delay
            if (delays.Count == 4)
            {

                delays.RemoveAt(0);
                Destroy(delayMarkers[0]);
                delayMarkers.RemoveAt(0);
            }


            //marker
            double relativePos = delay / secondsPerBeat;
            GameObject d = Instantiate(delayMarkerPrefab, delayMarkerPrefab.transform.parent.Find("DelayMarkerParent"));
            d.transform.localPosition = new Vector3(112.5f + 75f * (float)relativePos, 0, 0);
            d.SetActive(true);
            delayMarkers.Add(d);


            //resetButton.SetActive(true);





            //count
            //countText.gameObject.SetActive(true);
            //countText.text = "Count: " + delayHitCounter + "/5";





            //average
            delays.Add(delay);
            double combinedDelay = 0;
            foreach (double de in delays)
            {
                combinedDelay += de; 
            }
            averageDelay = combinedDelay / delays.Count;
            string late = (averageDelay >= 0) ? "LATE" : "EARLY";
            averageText.text = Math.Abs(Math.Round(averageDelay * 1000)) + "ms " + late;
            averageText.color = textColor.Evaluate((float)averageDelay * 5f + 0.5f);

            //average marker
            double averageRelativePos = averageDelay / secondsPerBeat;
            averageDelayMarker.transform.localPosition = new Vector3(112.5f + 75f * (float)averageRelativePos, 0, 0);
            averageDelayMarker.SetActive(true);
            averageDelayMarker.transform.GetChild(0).GetComponent<Image>().color = textColor.Evaluate((float)averageDelay * 5f + 0.5f);


            //offset n marker
            if (delays.Count == 4)
            {

                //double offset = Math.Round(averageDelay, 3);

                //SetOffset(offset);

                //double offsetRelativePos = offset / secondsPerBeat;
                //offsetMarker.transform.localPosition = new Vector3(112.5f + 75f * (float)offsetRelativePos, 0, 0);
                //offsetMarker.SetActive(true);
                //offsetText.text = "Offset: " + Math.Round(offset * 1000) + "ms";

                //canHit = false;


                //Consistency
                double min = 1000, max = -1000;
                foreach (double de in delays)
                {
                    min = Math.Min(min, de);
                    max = Math.Max(max, de);
                }
                double dist = max - min;
                string text = "";
                if (dist > 0.10)
                {
                    text = "Very Inconsistent Hits, Click on the 4th Beat!";
                }
                else if (dist > 0.05)
                {
                    text = "Inconsistent Hits, Keep Going!";
                }
                else
                {
                    text = "Consistent Hits! Good Job!";
                }

                consistencyText.text = text;
            }


        }
    }


    //Called through Button Press
    public void ChangeOffset(int i)
    {

        double offset = GetOffset();

        SetOffset(offset + (i * 0.010));

        changedOffset = true;


    }

    private double GetOffset()
    {
        if (input == ButtonInput.swing)
        {
            return PlayerRhythm.Instance.offsetSwing;
        }
        else //dash
        {
            return PlayerRhythm.Instance.offsetDash;
        }
    }
    
    private void SetOffset(double offset)
    {
        if (input == ButtonInput.swing)
        {
            PlayerRhythm.Instance.offsetSwing = offset;
            SaveSystem.Instance.GetData().swingOffset = offset;
        }
        else //dash
        {
            PlayerRhythm.Instance.offsetDash = offset;
            SaveSystem.Instance.GetData().dashOffset = offset;

        }

        string late = (offset >= 0) ? "LATE" : "EARLY";
        offsetText.text = Math.Round(Math.Abs(offset * 1000)) + "ms " + late;
        offsetText.color = textColor.Evaluate((float)offset * 5f + 0.5f);
    }


    public void PlayAnimations(int anticipation, float duration, int beat)
    {
        if (!gameObject.activeSelf) return;

        if (anticipation == 6)
        {
            if (currentBall)
            {
                particles.transform.position = currentBall.transform.position;
                particles.Play();
                Destroy(currentBall);
            }
            if (nextBall)
            {
                //below
                anim.speed = 1 / (duration * 8);
                anim.Play("CalibrationNew");
            }
        }
        else if (anticipation == 12)
        {

            if (nextBall) currentBall = nextBall;

            GameObject b = Instantiate(ballPrefab, ballPrefab.transform.parent);
            b.GetComponent<Animator>().enabled = true;
            b.GetComponent<Animator>().speed = 1 / (duration * 8);
            nextBall = b;
        }

    }


    //Called through Button Press
    public void Restart()
    {
        delays.Clear();

        averageDelay = 0;
        averageText.text = "";
        consistencyText.text = "";


        //markers
        for (int i = 0; i < delayMarkers.Count; i++)
        {
            Destroy(delayMarkers[i]);
        }
        delayMarkers.Clear();

        averageDelayMarker.SetActive(false);
        ////offsetMarker.SetActive(false);

        //countText.gameObject.SetActive(false);
        //countText.text = "Count: 0/5";

        //resetButton.SetActive(false);


        SetOffset(0);

    }

}
