using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalibrateInput : MonoBehaviour
{
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
    private List<Transform> balls = new List<Transform>(); //testing

    private void OnEnable()
    {
        EventManager.Instance.OnBeatForVisuals += PlayAnimations;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnBeatForVisuals -= PlayAnimations;
    }


    //Called through PlayerInput
    void OnSwing()
    {
        if (canHit)
        {
            double delay = PlayerRhythm.Instance.GetHitDelaySwing();

            //do this once somewhere instead, have it as an event when switching songs?, just keep it a constant fit for the menu music?
            //*2 cause this DOESNT check for the 8th notes
            secondsPerBeat = MusicManager.Instance.secondsPerBeat * 2;

            //marker
            double relativePos = delay / secondsPerBeat;
            GameObject d = Instantiate(delayMarkerPrefab, delayMarkerPrefab.transform.parent);
            d.transform.localPosition = new Vector3(112.5f + 75f * (float)relativePos, 0, 0);
            d.SetActive(true);
            delayMarkers.Add(d);

            //testvisualmarker
            //foreach (Transform ball in balls)
            //{
            //    if (!ball) continue;
            //    GameObject baX = Instantiate(delayMarkerPrefab, delayMarkerPrefab.transform.parent);
            //    baX.transform.position = ball.transform.position;
            //    baX.SetActive(true);
            //}

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
                PlayerRhythm.Instance.offsetSwing = offset;

                double offsetRelativePos = offset / secondsPerBeat;
                offsetMarker.transform.localPosition = new Vector3(112.5f + 75f * (float)offsetRelativePos, 0, 0);
                offsetMarker.SetActive(true);
                offsetText.text = "Offset: " + Math.Round(PlayerRhythm.Instance.offsetSwing * 1000) + "ms";

                canHit = false;
            }

            //if (delayHitCounter > 1)
            //    Debug.Break();
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
        if (!gameObject.activeSelf) return;

        if (anticipation == 6)
        {
            anim.speed = 1 / (duration * 8);
            anim.Play("CalibrationNew");
        }
        else if (anticipation == 12)
        {
            GameObject b = Instantiate(ballPrefab, ballPrefab.transform.parent);
            b.GetComponent<Animator>().enabled = true;
            b.GetComponent<Animator>().speed = 1 / (duration * 8);
            Destroy(b, 7);
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

        PlayerRhythm.Instance.offsetSwing = 0;
    }

}
