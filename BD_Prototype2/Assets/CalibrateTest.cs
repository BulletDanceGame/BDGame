using BulletDance.Audio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CalibrateTest : MonoBehaviour
{

    [SerializeField] private CalibrationMenu menu;
    [SerializeField] private GameObject delayMarkerPrefab;
    private List<GameObject> delayMarkers = new List<GameObject>();
    [SerializeField] private GameObject averageDelayMarker;
    [SerializeField] private TextMeshProUGUI averageDelayText;

    private double secondsPerBeat = .0;

    private double averageDelay = .0;
    private List<double> delays = new List<double>();
    private int delayHitCounter = 0;

    [SerializeField] private Animator anim;

    public GameObject ballPrefab;

    public ParticleSystem particles;

    GameObject currentBall = null;
    GameObject nextBall = null;

    public GameObject resetButton;
    public GameObject doneArrow;
    public TextMeshProUGUI redoText;
    public Button doneButton;

    [SerializeField] private Gradient textColor;


    private void OnEnable()
    {
        EventManager.Instance.OnBeatForVisuals += PlayAnimations;


        visualOffset = PlayerRhythm.Instance.offsetVisuals;
        visualOffsetText.text = visualOffset * 1000 + "ms";


        double offset = GetOffset(ButtonInput.swing);
        input1OffsetText.text = (offset * 1000) + "ms";
        input1OffsetText.color = textColor.Evaluate((float)offset * 5f + 0.5f);


        offset = GetOffset(ButtonInput.dash);
        input2OffsetText.text = (offset * 1000) + "ms";
        input2OffsetText.color = textColor.Evaluate((float)offset * 5f + 0.5f);


        //Clear
        delays.Clear();
        delayHitCounter = 0;

        averageDelay = 0;
        averageDelayText.text = "";
        redoText.text = "";
        doneArrow.SetActive(false);

        //markers
        for (int i = 0; i < delayMarkers.Count; i++)
        {
            Destroy(delayMarkers[i]);
        }
        delayMarkers.Clear();

        averageDelayMarker.SetActive(false);
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
        OnPress(ButtonInput.swing);

    }

    void OnDash()
    {
        OnPress(ButtonInput.dash);
    }



    private void OnPress(ButtonInput input)
    {
        if (menu.canHit && currentBall != null)
        {

            double delay = PlayerRhythm.Instance.GetHitDelay(input);
            PlayerRhythm.Instance.GetComponent<PlayerSounds>().PlayerSwing(PlayerRhythm.Instance.GetBeatTiming(input), Vector2.zero); //ugh

            //do this once somewhere instead, have it as an event when switching songs?, just keep it a constant fit for the menu music?
            //*2 cause this DOESNT check for the 8th notes
            secondsPerBeat = MusicManager.Instance.secondsPerBeat * 2;

            //ball
            particles.transform.position = currentBall.transform.position;
            particles.Play();
            Destroy(currentBall);


            //remove when changed offset
            if (changedOffset)
            {
                changedOffset = false;


                delays.Clear();
                delayHitCounter = 0;

                averageDelay = 0;
                averageDelayText.text = "";
                redoText.text = "";
                doneArrow.SetActive(false);

                //markers
                for (int i = 0; i < delayMarkers.Count; i++)
                {
                    Destroy(delayMarkers[i]);
                }
                delayMarkers.Clear();

                averageDelayMarker.SetActive(false);
            }




            //marker
            double relativePos = delay / secondsPerBeat;
            GameObject d = Instantiate(delayMarkerPrefab, delayMarkerPrefab.transform.parent.Find("DelayMarkerParent"));
            d.transform.localPosition = new Vector3(112.5f + 75f * (float)relativePos, 0, 0);
            d.SetActive(true);
            delayMarkers.Add(d);

            //average
            delayHitCounter++;
            if (delayHitCounter == 5)
            {
                delayHitCounter--;

                Destroy(delayMarkers[0]);
                delayMarkers.RemoveAt(0);

                delays.RemoveAt(0);

            }


            delays.Add(delay);
            double combinedDelay = 0;
            foreach (double de in delays)
            {
                combinedDelay += de;
            }
            averageDelay = combinedDelay / delayHitCounter;
            string late = (averageDelay >= 0) ? "LATE" : "EARLY";
            averageDelayText.text = Math.Abs(Math.Round(averageDelay * 1000)) + "ms " + late;
            averageDelayText.color = textColor.Evaluate((float)averageDelay * 5f + 0.5f);

            //average marker
            double averageRelativePos = averageDelay / secondsPerBeat;
            averageDelayMarker.transform.localPosition = new Vector3(112.5f + 75f * (float)averageRelativePos, 0, 0);
            averageDelayMarker.SetActive(true);
            averageDelayMarker.transform.GetChild(0).GetComponent<Image>().color = textColor.Evaluate((float)averageDelay * 5f + 0.5f);



            if (delayHitCounter >= 4)
            {
                //Consistency
                double min = 1000, max = -1000;
                foreach (double de in delays)
                {
                    min = Math.Min(min, de);
                    max = Math.Max(max, de);
                }
                double dist = max - min;
                string text = "";
                if (dist > 0.15)
                {
                    text = "INCONSISTENT, this game will be difficult for you";
                    doneArrow.SetActive(false);
                }
                else if (dist > 0.075)
                {
                    text = "Inconsistent, try and hit to the 4th beat";
                    doneArrow.SetActive(false);
                }
                else
                {
                    if (Math.Abs(averageDelay) > 0.13)
                    {
                        text = "Too far OFF beat, you will miss each beat in game. Consider Redoing CALIBRATION!";
                        doneArrow.SetActive(false);
                    }
                    else if(Math.Abs(averageDelay) > 0.06)
                    {
                        text = "Quite far off beat, try to adjust delays!";
                        doneArrow.SetActive(false);
                    }
                    else
                    {
                        text = "Nice! Press DONE!";
                        doneArrow.SetActive(true);
                        doneButton.Select();
                    }
                }
                redoText.text = text;
            }
            

        }
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
        delayHitCounter = 0;
        averageDelay = 0;
        delays.Clear();

        //markers
        for (int i = 0; i < delayMarkers.Count; i++)
        {
            Destroy(delayMarkers[i]);
        }
        delayMarkers.Clear();

        averageDelayMarker.SetActive(false);


        resetButton.SetActive(false);
        doneArrow.SetActive(false);
        redoText.text = "";


        menu.ActivateTitleButton();
    }




    [Header("Adjusting")]
    private double visualOffset;
    public TextMeshProUGUI visualOffsetText;
    public TextMeshProUGUI input1OffsetText;
    public TextMeshProUGUI input2OffsetText;




    bool changedOffset;
    
    
    
    public void ChangeVisualOffset(int dir)
    {
        visualOffset += 0.010 * dir;
        visualOffset = Math.Round(Math.Max(-0.30, Math.Min(0.30, visualOffset)), 3);
        PlayerRhythm.Instance.UpdateOffsetVisuals(visualOffset);
        SaveSystem.Instance.GetData().visualOffset = visualOffset;
        visualOffsetText.text = visualOffset * 1000 + "ms";

        anim.enabled = false;
        Destroy(currentBall);
        Destroy(nextBall);

        changedOffset = true;
    }

    public void ChangeInput1Offset(int i)
    {
        double offset = GetOffset(ButtonInput.swing);

        SetOffset(ButtonInput.swing, offset + (i * 0.010));

        changedOffset = true;
    }

    public void ChangeInput2Offset(int i)
    {
        double offset = GetOffset(ButtonInput.dash);

        SetOffset(ButtonInput.dash, offset + (i * 0.010));

        changedOffset = true;
    }

    private double GetOffset(ButtonInput input)
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

    private void SetOffset(ButtonInput input, double offset)
    {
        if (input == ButtonInput.swing)
        {
            PlayerRhythm.Instance.offsetSwing = offset;
            SaveSystem.Instance.GetData().swingOffset = offset;
            input1OffsetText.text = Math.Round(offset * 1000) + "ms";
            input1OffsetText.color = textColor.Evaluate((float)offset * 5f + 0.5f);
        }
        else //dash
        {
            PlayerRhythm.Instance.offsetDash = offset;
            SaveSystem.Instance.GetData().dashOffset = offset;
            input2OffsetText.text = Math.Round(offset * 1000) + "ms";
            input2OffsetText.color = textColor.Evaluate((float)offset * 5f + 0.5f);

        }

        
    }
}
