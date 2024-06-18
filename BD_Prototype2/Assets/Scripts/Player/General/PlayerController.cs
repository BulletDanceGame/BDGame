using System.Collections;
using UnityEngine;

//Control the player spawning & positioning
//Remove everything else
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private void Awake()
    {
        Instance = this;
    }
}