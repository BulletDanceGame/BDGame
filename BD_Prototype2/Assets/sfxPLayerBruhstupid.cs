using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sfxPLayerBruhstupid : MonoBehaviour
{
    public AK.Wwise.Event landingSFX, jumpSFX;

    public void PlayLanding()
    {
        landingSFX.Post(gameObject);
    }
}
