using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWinScreen : MonoBehaviour
{

    public GameObject WinScreen;
    // Start is called before the first frame update
    void Start()
    {
        WinScreen.SetActive(true);

    }

    public void DisableWinScreenButton()
    {
        GameObject.Find("Player").GetComponent<PlayerMovement>().canMove = true;
        GameObject.Find("DirectionController").GetComponent<PlayerSwing>().SwingActivated = true;

        WinScreen.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
