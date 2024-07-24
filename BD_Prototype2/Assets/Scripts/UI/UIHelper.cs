using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper : MonoBehaviour
{
    public GameObject ObjectToHide;
    public void HideGameObject()
    {
        ObjectToHide.SetActive(false);
    }
}
