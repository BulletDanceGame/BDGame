using System;
using UnityEngine;

public class EditorCheck
{
    public static bool inEditMode { get { return !Application.isPlaying; } }
    public static bool GetComponentInEditMode<T>(ref T component, Func<T> getComponentMethod)
    {
        if(!inEditMode) 
        {
            //Debug.Log("Not in edit mode");
            return false;
        }
        if(component != null) 
        {
            //Debug.Log("Component already has a reference");
            return false;
        }

        component = getComponentMethod();
        if(component == null) 
        {
            //Debug.Log("Failed to get component of type " + typeof(T).ToString() + " in edit mode");
            return false;
        }

        //Debug.Log("Succesfully got component of type " + typeof(T).ToString() + " in edit mode");
        return true;
    }
}