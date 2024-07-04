using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;

public class CalibrateAudio : MonoBehaviour
{
    private double audioOffset;
    public TextMeshProUGUI offsetText;

    public Animator anim;

    public GameObject ballPrefab;
    private GameObject currentBall;
    private GameObject nextBall;
    public ParticleSystem particles;


    private bool ballsActive;

    private void OnEnable()
    {
        EventManager.Instance.OnBeatForVisuals += PlayAnimations;

        audioOffset = PlayerRhythm.Instance.offsetVisuals;
        offsetText.text = "Offset: " + audioOffset * 1000 + "ms";
    }

    private void OnDisable()
    {
        EventManager.Instance.OnBeatForVisuals -= PlayAnimations;
        ballsActive = false;
        Destroy(currentBall);
        Destroy(nextBall);
    }


    public void ChangeVisualOffset(int dir)
    {
        audioOffset += 0.010 * dir;
        audioOffset = Math.Round(Math.Max(-0.30, Math.Min(0.30, audioOffset)), 3);
        PlayerRhythm.Instance.UpdateOffsetVisuals(audioOffset);
        offsetText.text = "Offset: " + audioOffset * 1000 + "ms";


        anim.enabled = false;

        Destroy(currentBall);
        Destroy(nextBall);
    }



    public void PlayAnimations(int anticipation, float duration, int beat)
    {
        if (!gameObject.activeSelf) return;

        if (anticipation == 0)
        {
            if (!currentBall) return;

            particles.transform.position = currentBall.transform.position;
            particles.Play();
            Destroy(currentBall);
        }
        else if (anticipation == 6)
        {
            if (!nextBall) return;

            print("check bar" + beat + " "  + Time.realtimeSinceStartupAsDouble);
            anim.enabled = true;
            anim.speed = 1 / (duration * 8);
            anim.Play("CalibrationNew");
            currentBall = nextBall;
        }
        else if (anticipation == 12)
        {
            print("check ball" + beat + " " + Time.realtimeSinceStartupAsDouble);
            GameObject b = Instantiate(ballPrefab, ballPrefab.transform.parent);
            b.GetComponent<Animator>().enabled = true;
            b.GetComponent<Animator>().speed = 1 / (duration * 8);
            nextBall = b;
            ballsActive = true;
        }
    }

}
