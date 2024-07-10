using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;

public class CalibrateAudio : MonoBehaviour
{
    private double audioOffset;
    public TextMeshProUGUI offsetText;

    public RectTransform visuals;
    public Animator anim;

    public GameObject ballPrefab;
    private GameObject currentBall;
    private GameObject nextBall;
    public ParticleSystem particles;



    private void OnEnable()
    {
        EventManager.Instance.OnBeatForVisuals += PlayAnimations;

        audioOffset = PlayerRhythm.Instance.offsetVisuals;
        offsetText.text = "Offset: " + audioOffset * 1000 + "ms";
    }

    private void OnDisable()
    {
        EventManager.Instance.OnBeatForVisuals -= PlayAnimations;
        Destroy(currentBall);
        Destroy(nextBall);
    }


    public void ChangeVisualOffset(int dir)
    {
        audioOffset += 0.010 * dir;
        audioOffset = Math.Round(Math.Max(-0.30, Math.Min(0.30, audioOffset)), 3);
        PlayerRhythm.Instance.UpdateOffsetVisuals(audioOffset);
        offsetText.text = "Offset: " + audioOffset * 1000 + "ms";

        Vector2 pos = visuals.anchoredPosition;
        pos.x += dir*(50f / 30f);
        pos.x = Mathf.Clamp(pos.x, -50f, 50f);
        visuals.anchoredPosition = pos;

        anim.enabled = false;

        Destroy(currentBall);
        Destroy(nextBall);
    }



    public void PlayAnimations(int anticipation, float duration, int beat)
    {
        if (!gameObject.activeSelf) return;

        if (anticipation == 0)
        {
            //if (!currentBall) return;

            //particles.transform.position = currentBall.transform.position;
            //particles.Play();
            //Destroy(currentBall);
        }
        else if (anticipation == 6)
        {
            if (!nextBall) return;

            print("check bar" + beat + " "  + Time.timeAsDouble);
            anim.enabled = true;
            anim.speed = 1 / (duration * 8);
            anim.Play("CalibrationNew");
            currentBall = nextBall;
        }
        else if (anticipation == 12)
        {
            print("check ball" + beat + " " + Time.timeAsDouble);
            GameObject b = Instantiate(ballPrefab, ballPrefab.transform.parent);
            b.GetComponent<Animator>().enabled = true;
            b.GetComponent<Animator>().speed = 1 / (duration * 8);
            nextBall = b;
            Destroy(b, 4);
        }
    }

}
