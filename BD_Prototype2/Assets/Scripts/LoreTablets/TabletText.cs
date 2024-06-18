using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TabletText", menuName = "InteractableUIObjects/LoreTablet", order = 1)]
public class TabletText : ScriptableObject
{
    [TextArea]
    public string textForTablet;
}
