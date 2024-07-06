using BulletDance.Audio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum ButtonInput { swing, dash };

public class CalibrateInput : MonoBehaviour
{
    public ButtonInput input;

    [SerializeField] private GameObject delayMarkerPrefab;
    private List<GameObject> delayMarkers = new List<GameObject>();
    [SerializeField] private GameObject averageDelayMarker;
    [SerializeField] private TextMeshProUGUI averageDelayText;
    [SerializeField] private GameObject offsetMarker;
    [SerializeField] private TextMeshProUGUI offsetText;
    [SerializeField] private TextMeshProUGUI countText;

    private double secondsPerBeat = .0;

    private double averageDelay = .0;
    private double combinedDelay = .0;
    private int delayHitCounter = 0;

    private double max = 0;
    private double min = 0;

    [SerializeField] private Animator anim;

    private bool canHit = true;

    public GameObject ballPrefab;

    public ParticleSystem particles;


    GameObject currentBall = null;
    GameObject nextBall = null;

    public GameObject title1;
    public GameObject title2;


    private void OnEnable()
    {
        EventManager.Instance.OnBeatForVisuals += PlayAnimations;
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
        if (canHit)// && currentBall != null)
        {

            double delay = PlayerRhythm.Instance.GetHitDelay(input);
            PlayerRhythm.Instance.GetComponent<PlayerSounds>().PlayerSwing(PlayerRhythm.Instance.GetBeatTiming(input), Vector2.zero); //ugh

            //do this once somewhere instead, have it as an event when switching songs?, just keep it a constant fit for the menu music?
            //*2 cause this DOESNT check for the 8th notes
            secondsPerBeat = MusicManager.Instance.secondsPerBeat * 2;

            //ball
            //particles.transform.position = currentBall.transform.position;
            //particles.Play();
            //Destroy(currentBall);

            //marker
            double relativePos = delay / secondsPerBeat;
            GameObject d = Instantiate(delayMarkerPrefab, delayMarkerPrefab.transform.parent.Find("DelayMarkerParent"));
            d.transform.localPosition = new Vector3(112.5f + 75f * (float)relativePos, 0, 0);
            d.SetActive(true);
            delayMarkers.Add(d);


            //count
            delayHitCounter++;
            countText.text = "Count: " + delayHitCounter + "/10";

            //average
            combinedDelay += delay;
            averageDelay = combinedDelay / delayHitCounter;
            averageDelayText.text = "Average: " + Math.Round(averageDelay * 1000) + "ms";

            //average marker
            double averageRelativePos = averageDelay / secondsPerBeat;
            averageDelayMarker.transform.localPosition = new Vector3(112.5f + 75f * (float)averageRelativePos, 0, 0);
            averageDelayMarker.SetActive(true);


            //offset n marker
            if (delayHitCounter == 10)
            {
                double offset = Math.Round(averageDelay, 3);

                if(input == ButtonInput.swing)
                {
                    PlayerRhythm.Instance.offsetSwing = offset;
                }
                else//ddash
                {
                    PlayerRhythm.Instance.offsetDash = offset;
                }

                double offsetRelativePos = offset / secondsPerBeat;
                offsetMarker.transform.localPosition = new Vector3(112.5f + 75f * (float)offsetRelativePos, 0, 0);
                offsetMarker.SetActive(true);
                offsetText.text = "Offset: " + Math.Round(offset * 1000) + "ms";

                canHit = false;

                title1.SetActive(false);
                title2.SetActive(true);
            }

        }
    }


    //Called through Button Press
    public void ChangeOffset(int i)
    {
        PlayerRhythm.Instance.offsetSwing += i * 0.010;

        double offset = PlayerRhythm.Instance.offsetSwing / secondsPerBeat;
        offsetMarker.transform.localPosition = new Vector3(112.5f + 75f * (float)offset, 0, 0);

        offsetText.text = "Offset: " + PlayerRhythm.Instance.offsetSwing * 1000 + "ms";
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
        delayHitCounter = 0;
        averageDelay = 0;
        combinedDelay = 0;
        max = 0;
        min = 0;

        //markers
        for (int i = 0; i < delayMarkers.Count; i++)
        {
            Destroy(delayMarkers[i]);
        }
        delayMarkers.Clear();

        averageDelayMarker.SetActive(false);
        offsetMarker.SetActive(false);

        countText.text = "Count: 0/10";

        canHit = true;

        if(input == ButtonInput.swing)
        {
            PlayerRhythm.Instance.offsetSwing = 0;
        }
        else //dash
        {
            PlayerRhythm.Instance.offsetDash = 0;

        }

        title1.SetActive(true);
        title2.SetActive(false);
    }

}
