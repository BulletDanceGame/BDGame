using BulletDance.Audio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class CalibrateVisuals : MonoBehaviour
{
    public ButtonInput input;


    private int stage = 0;

    private double visualOffset;

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

    private double averageDelayWithV = .0;
    private double averageDelayWithoutV = .0;
    private List<double> delays = new List<double>();


    [SerializeField] private Animator anim;


    public GameObject ballPrefab;

    public ParticleSystem particles;


    GameObject currentBall = null;
    GameObject nextBall = null;


    [SerializeField] private TextMeshProUGUI averageText;
    [SerializeField] private TextMeshProUGUI consistencyText;
    [SerializeField] private TextMeshProUGUI visualAidText;
    [SerializeField] private GameObject visuals;
    [SerializeField] private GameObject cali;
    [SerializeField] private TextMeshProUGUI recomendedOffset;

    [SerializeField] private Gradient textColor;

    private void OnEnable()
    {
        EventManager.Instance.OnBeatForVisuals += PlayAnimations;


        visualOffset = PlayerRhythm.Instance.offsetVisuals;

        if (visualOffset != (double)default)
        {
            stage = 2;
            offsetText.text = visualOffset * 1000 + "ms";

            visuals.SetActive(false);
            visualAidText.gameObject.SetActive(false);
            cali.SetActive(true);

            recomendedOffset.text = " - ";
         

            anim.enabled = false;
        }
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
        if (stage == 2)
        {
            return;
        }


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
            double averageDelay = 0;
            if (stage == 0)
            {
                averageDelayWithV = combinedDelay / delays.Count;
                averageDelay = averageDelayWithV;
            }
            else if(stage == 1)
            {
                averageDelayWithoutV = combinedDelay / delays.Count;
                averageDelay = averageDelayWithoutV;
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
                //Consistency
                double min = 1000, max = -1000;
                foreach (double de in delays)
                {
                    min = Math.Min(min, de);
                    max = Math.Max(max, de);
                }
                double dist = max - min;
                if (dist > 0.12)
                {
                    consistencyText.text = "Very Inconsistent, Keep Pressing!";
                }
                else if (dist > 0.06)
                {
                    consistencyText.text = "Inconsistent, Keep Pressing!";
                }
                else
                {
                    stage++;

                    if (stage == 1)
                    {
                        visualAidText.gameObject.SetActive(true);
                    }
                    else if (stage == 2)
                    {
                        visuals.SetActive(false);
                        visualAidText.gameObject.SetActive(false);
                        cali.SetActive(true);
                        double diff = averageDelayWithV - averageDelayWithoutV;
                        recomendedOffset.text = Math.Abs(Math.Round(diff * 1000)) + "ms ";
                        recomendedOffset.color = textColor.Evaluate((float)diff * 5f + 0.5f);
                    }


                    delays.Clear();
                    averageText.text = "";
                    consistencyText.text = "";
                    //markers
                    for (int i = 0; i < delayMarkers.Count; i++)
                    {
                        Destroy(delayMarkers[i]);
                    }
                    delayMarkers.Clear();

                    averageDelayMarker.SetActive(false);


                    anim.enabled = false;
                    Destroy(currentBall);
                    Destroy(nextBall);
                }


            }


        }
    }


    //Called through Button Press
    public void ChangeVisualOffset(int dir)
    {
        visualOffset += 0.010 * dir;
        visualOffset = Math.Round(Math.Max(-0.30, Math.Min(0.30, visualOffset)), 3);
        PlayerRhythm.Instance.UpdateOffsetVisuals(visualOffset);
        SaveSystem.Instance.GetData().visualOffset = visualOffset;
        offsetText.text = visualOffset * 1000 + "ms";

    }



    public void PlayAnimations(int anticipation, float duration, int beat)
    {
        if (!gameObject.activeSelf || stage > 0) return;

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

        stage = 0;
        averageDelayWithV = 0;
        averageDelayWithoutV = 0;
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


        visualOffset = (double) default;
        PlayerRhythm.Instance.UpdateOffsetVisuals(visualOffset);
        SaveSystem.Instance.GetData().visualOffset = visualOffset;

        visuals.SetActive(true);
        visualAidText.gameObject.SetActive(false);
        cali.SetActive(false);

        anim.enabled = true;
    }

}
