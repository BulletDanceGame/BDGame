using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatCount : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI text;
    private string defaultText = "BEAT: ";


    void Update()
    {
        int currentBeat = BossConductor.Instance.currentBeat;
        if(currentBeat == 0) currentBeat += 1;
        else currentBeat = currentBeat%8;
        if(currentBeat == 0) currentBeat = 8;
        text.text = defaultText + currentBeat;
    }
}
