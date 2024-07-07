using BulletDance.Audio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CalibrateTest : MonoBehaviour
{

    [SerializeField] private GameObject delayMarkerPrefab;
    private List<GameObject> delayMarkers = new List<GameObject>();
    [SerializeField] private GameObject averageDelayMarker;
    [SerializeField] private TextMeshProUGUI averageDelayText;

    private double secondsPerBeat = .0;

    private double averageDelay = .0;
    private double combinedDelay = .0;
    private int delayHitCounter = 0;

    [SerializeField] private Animator anim;

    public GameObject ballPrefab;

    public ParticleSystem particles;

    GameObject currentBall = null;
    GameObject nextBall = null;


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
        OnPress(ButtonInput.swing);

    }

    void OnDash()
    {
        OnPress(ButtonInput.dash);
    }



    private void OnPress(ButtonInput input)
    {
        if (currentBall != null)
        {

            double delay = PlayerRhythm.Instance.GetHitDelay(input);
            PlayerRhythm.Instance.GetComponent<PlayerSounds>().PlayerSwing(PlayerRhythm.Instance.GetBeatTiming(input), Vector2.zero); //ugh

            //do this once somewhere instead, have it as an event when switching songs?, just keep it a constant fit for the menu music?
            //*2 cause this DOESNT check for the 8th notes
            secondsPerBeat = MusicManager.Instance.secondsPerBeat * 2;
            print("what " + secondsPerBeat);

            //ball
            particles.transform.position = currentBall.transform.position;
            particles.Play();
            Destroy(currentBall);

            //marker
            double relativePos = delay / secondsPerBeat;
            GameObject d = Instantiate(delayMarkerPrefab, delayMarkerPrefab.transform.parent.Find("DelayMarkerParent"));
            d.transform.localPosition = new Vector3(112.5f + 75f * (float)relativePos, 0, 0);
            d.SetActive(true);
            delayMarkers.Add(d);

            //average
            delayHitCounter++;
            combinedDelay += delay;
            averageDelay = combinedDelay / delayHitCounter;
            averageDelayText.text = "Average: " + Math.Round(averageDelay * 1000) + "ms";

            //average marker
            double averageRelativePos = averageDelay / secondsPerBeat;
            averageDelayMarker.transform.localPosition = new Vector3(112.5f + 75f * (float)averageRelativePos, 0, 0);
            averageDelayMarker.SetActive(true);



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
        combinedDelay = 0;

        //markers
        for (int i = 0; i < delayMarkers.Count; i++)
        {
            Destroy(delayMarkers[i]);
        }
        delayMarkers.Clear();

        averageDelayMarker.SetActive(false);
    }

}
