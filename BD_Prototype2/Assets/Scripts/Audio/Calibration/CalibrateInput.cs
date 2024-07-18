using BulletDance.Audio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;


public enum ButtonInput { swing, dash, none };

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
    private int delayHitCounter = 0;


    [SerializeField] private Animator anim;


    public GameObject ballPrefab;

    public ParticleSystem particles;


    GameObject currentBall = null;
    GameObject nextBall = null;


    [SerializeField] private TextMeshProUGUI[] triesTexts;
    [SerializeField] private TextMeshProUGUI averageText;
    [SerializeField] private TextMeshProUGUI consistencyText;



    private void OnEnable()
    {
        EventManager.Instance.OnBeatForVisuals += PlayAnimations;


        offsetText.text = "Offset: " + GetOffset() * 1000 + "ms";
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

            double delay = PlayerRhythm.Instance.GetHitDelay(ButtonInput.none);
            PlayerRhythm.Instance.GetComponent<PlayerSounds>().PlayerSwing(PlayerRhythm.Instance.GetBeatTiming(ButtonInput.none), Vector2.zero); //ugh

            //do this once somewhere instead, have it as an event when switching songs?, just keep it a constant fit for the menu music?
            //*2 cause this DOESNT check for the 8th notes
            secondsPerBeat = MusicManager.Instance.secondsPerBeat * 2;

            //ball
            //particles.transform.position = currentBall.transform.position;
            //particles.Play();
            //Destroy(currentBall);

            //Remove last delay
            if (delayHitCounter == 5)
            {
                delays.RemoveAt(0);
                //Destroy(delayMarkers[0]);
                //delayMarkers.RemoveAt(0);
                delayHitCounter--;

                for (int i = 0; i < triesTexts.Length-1; i++)
                {
                    string l = (delays[i] >= 0) ? "late" : "early";
                    triesTexts[i].text = (i + 1).ToString() + ". " + Math.Abs(Math.Round(delays[i] * 1000)) + "ms " + l;
                }
            }



            //marker
            //double relativePos = delay / secondsPerBeat;
            //GameObject d = Instantiate(delayMarkerPrefab, delayMarkerPrefab.transform.parent.Find("DelayMarkerParent"));
            //d.transform.localPosition = new Vector3(112.5f + 75f * (float)relativePos, 0, 0);
            //d.SetActive(true);
            //delayMarkers.Add(d);


            //resetButton.SetActive(true);


            triesTexts[delayHitCounter].gameObject.SetActive(true);
            string late = (delay >= 0) ? "late" : "early"; 
            triesTexts[delayHitCounter].text = (delayHitCounter+1).ToString() + ". " + Math.Abs(Math.Round(delay * 1000)) + "ms " + late;

            delayHitCounter++;
            
            
            
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
            late = (averageDelay >= 0) ? "late" : "early";
            averageDelay = combinedDelay / delayHitCounter;
            averageText.text = "Average: " + Math.Abs(Math.Round(averageDelay * 1000)) + "ms " + late;

            //average marker
            //double averageRelativePos = averageDelay / secondsPerBeat;
            //averageDelayMarker.transform.localPosition = new Vector3(112.5f + 75f * (float)averageRelativePos, 0, 0);
            //averageDelayMarker.SetActive(true);


            //offset n marker
            if (delayHitCounter == 5)
            {
                double offset = Math.Round(averageDelay, 3);

                SetOffset(offset);

                //double offsetRelativePos = offset / secondsPerBeat;
                //offsetMarker.transform.localPosition = new Vector3(112.5f + 75f * (float)offsetRelativePos, 0, 0);
                //offsetMarker.SetActive(true);
                //offsetText.text = "Offset: " + Math.Round(offset * 1000) + "ms";

                //canHit = false;

            }

        }
    }


    //Called through Button Press
    public void ChangeOffset(int i)
    {
        double offset = GetOffset();

        SetOffset(offset + (i * 0.010));

        //double offsetPos = offset / secondsPerBeat;
        //offsetMarker.transform.localPosition = new Vector3(112.5f + 75f * (float)offsetPos, 0, 0);


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

        offsetText.text = "Offset: " + offset * 1000 + "ms";
    }


    public void PlayAnimations(int anticipation, float duration, int beat)
    {
        return;
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
        delayHitCounter = 0;
        averageDelay = 0;
        averageText.text = "";

        foreach(TextMeshProUGUI t in triesTexts)
        {
            t.text = "";
            t.gameObject.SetActive(false);
        }

        //markers
        //for (int i = 0; i < delayMarkers.Count; i++)
        //{
        //    Destroy(delayMarkers[i]);
        //}
        //delayMarkers.Clear();

        //averageDelayMarker.SetActive(false);
        ////offsetMarker.SetActive(false);

        //countText.gameObject.SetActive(false);
        //countText.text = "Count: 0/5";

        //resetButton.SetActive(false);


        SetOffset(0);

    }

}
