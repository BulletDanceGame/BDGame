using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class RTPCManager : MonoBehaviour
{
    static public RTPCManager Instance;

    [Serializable]
    public class RTPC
    {
        public string name;
        public float defaultValue;
        public float value { get; private set; }
        public void SetValue(float v) { value = v; }

        public float goalValue { get; private set; }
        public void SetGoalValue(float v) { goalValue = v; }

        public float steadyValue { get; private set; }
        public void SetSteadyValue(float v) { steadyValue = v; }


        public IEnumerator coroutine;
        [HideInInspector] public bool isCoroutineRunning;
    }

    [Serializable]
    public class RTPCAttribute
    {
        public string name;
        public RTPC[] _parameters;
    }

    [SerializeField] private RTPCAttribute[] _attributes;

    [SerializeField] private string[] _states;


    public enum CurveTypes { linear, high_curve, low_curve }

    public AnimationCurve[] _curves;





    void Awake()
    {
        Instance = this;

        ResetAll();
    }


    private void Start()
    {
        //Different ways to use it


        //SINGLE RTPC

        //For all following Functions theres a version for Set and Add

        //Change value of 1 RTPC, stays at that value
        //SetValue("HypeCoolness", 40, 10, CurveTypes.linear);

        //Change value of 1 RTPC, will go back to previous value after a set time
        //AddValue("HypeCoolness", 20, 5, CurveTypes.high_curve, 5, CurveTypes.low_curve, 3);


        //Change value of ALL RTPC in an Attribute, stays at that value
        //SetAttributeValue("Coolness", 20, 5, CurveTypes.high_curve);

        //Change value of ALL RTPC in an Attribute, will go back to previous value after a set time
        //AddAttributeValue("Coolness", 20, 5, CurveTypes.high_curve, 5, CurveTypes.low_curve, 3);

        //By writing the names of specific RTPCs when calling any of the AttributeValue-functions it will ignore them
        //SetAttributeValue("Coolness", 20, 5, CurveTypes.high_curve, "GeneralCoolness", "HypeCoolness");


    }


    //RESETTING
    private void ResetAll()
    {
        //Reset States
        //AkSoundEngine.SetState("Hurt", "high");
        //AkSoundEngine.SetState("DumbNotes", "P2P3");
        AkSoundEngine.SetState("DynamicGameplaySFX", "Low");

        //Reset Parameters
        for (int a = 0; a < _attributes.Length; a++)
        {
            for (int i = 0; i < _attributes[a]._parameters.Length; i++)
            {
                RTPC parameter = _attributes[a]._parameters[i];
                AkSoundEngine.SetRTPCValue(parameter.name, parameter.defaultValue);
                parameter.SetValue(parameter.defaultValue);
                parameter.SetSteadyValue(parameter.defaultValue);
                //doesnt use ResetValue as it starts a coroutine and ResetAll should be instantaneous
            }
        }


    }

    public void ResetValue(string parameterName, float fadeInDuration, CurveTypes fadeInCurve)
    {
        //find rtpc
        RTPC rtpc = GetParameter(parameterName);
        if (rtpc == null) return;

        //start fade coroutine
        rtpc.coroutine = Fade(rtpc, rtpc.defaultValue, fadeInDuration, GetCurve(fadeInCurve));
        StartCoroutine(rtpc.coroutine);
    }

    public void ResetAttributeValue(string attributeName, float fadeInDuration, CurveTypes fadeInCurve)
    {
        ResetAttributeValue(attributeName, fadeInDuration, fadeInCurve, "none");
    }
    public void ResetAttributeValue(string attributeName, float fadeInDuration, CurveTypes fadeInCurve, params string[] ignoredParameters)
    {
        //find rtpc
        RTPCAttribute attribute = GetAttribute(attributeName);

        if (attribute == null) return;

        foreach (RTPC rtpc in attribute._parameters)
        {
            if (!ignoredParameters.Contains(rtpc.name))
            {
                ResetValue(rtpc.name, fadeInDuration, fadeInCurve);
            }
        }
    }


    //SET STATE
    public void SetState(string parameterName, string state)
    {
        //check, has it been added
        if (!_states.Contains(parameterName))
        {
            print("RTPC " + parameterName + " has not been added!");
            return;
        }

        //set state
        AkSoundEngine.SetState(parameterName, state);
    }


    //SET VALUE
    public void SetValue(string parameterName, float value)
    {
        ////check, has it been added
        //if (!_allParameters.Contains(parameterName))
        //{
        //    print("RTPC " + parameterName + " has not been added!");
        //    return;
        //}
        ////set value
        //AkSoundEngine.SetRTPCValue(parameterName, value);

    }

    public void SetValue(string parameterName, float value, float fadeInDuration, CurveTypes fadeInCurve)
    {
        //find rtpc
        RTPC rtpc = GetParameter(parameterName);
        if (rtpc == null) return;

        //if theres already a coroutine stop it
        if (rtpc.isCoroutineRunning)
        {
            StopCoroutine(rtpc.coroutine);
        }

        //start fade coroutine
        rtpc.coroutine = Fade(rtpc, value, fadeInDuration, GetCurve(fadeInCurve));
        StartCoroutine(rtpc.coroutine);
        rtpc.isCoroutineRunning = true;
        rtpc.SetSteadyValue(value);
    }

    public void SetValue(string parameterName, float value, float fadeInDuration, CurveTypes fadeInCurve,
                         float fadeOutDuration, CurveTypes fadeOutCurve, float durationOfNewValue)
    {
        //find rtpc
        RTPC rtpc = GetParameter(parameterName);
        if (rtpc == null) return;

        //if theres already a coroutine, stop it
        if (rtpc.isCoroutineRunning)
        {
            StopCoroutine(rtpc.coroutine);
        }

        //start fade coroutine
        rtpc.coroutine = Fade(rtpc, value, fadeInDuration, GetCurve(fadeInCurve),
            fadeOutDuration, GetCurve(fadeOutCurve), durationOfNewValue);
        StartCoroutine(rtpc.coroutine);
        rtpc.isCoroutineRunning = true;
    }


    //ADD VALUE
    public void AddValue(string parameterName, float value, float fadeInDuration, CurveTypes fadeInCurve)
    {
        //find rtpc
        RTPC rtpc = GetParameter(parameterName);
        if (rtpc == null) return;

        float newValue;

        //if theres already a coroutine stop it
        if (rtpc.isCoroutineRunning)
        {
            StopCoroutine(rtpc.coroutine);

            newValue = rtpc.goalValue + value;
        }
        else
        {
            newValue = rtpc.value + value;
        }

        //start fade coroutine
        rtpc.coroutine = Fade(rtpc, newValue, fadeInDuration, GetCurve(fadeInCurve));
        StartCoroutine(rtpc.coroutine);
        rtpc.isCoroutineRunning = true;
        rtpc.SetSteadyValue(rtpc.steadyValue + value);
    }

    public void AddValue(string parameterName, float value, float fadeInDuration, CurveTypes fadeInCurve,
                         float fadeOutDuration, CurveTypes fadeOutCurve, float durationOfNewValue)
    {
        //find rtpc
        RTPC rtpc = GetParameter(parameterName);
        if (rtpc == null) return;

        float newValue;

        //if theres already a coroutine stop it
        if (rtpc.isCoroutineRunning)
        {
            StopCoroutine(rtpc.coroutine);

            newValue = rtpc.goalValue + value;
        }
        else
        {
            newValue = rtpc.value + value;
        }

        //start fade coroutine
        rtpc.coroutine = Fade(rtpc, newValue, fadeInDuration, GetCurve(fadeInCurve),
            fadeOutDuration, GetCurve(fadeOutCurve), durationOfNewValue);
        StartCoroutine(rtpc.coroutine);
        rtpc.isCoroutineRunning = true;
    }


    //SET ATTIRBUTES
    public void SetAttributeValue(string attributeName, float value, float fadeInDuration, CurveTypes fadeInCurve)
    {
        SetAttributeValue(attributeName, value, fadeInDuration, fadeInCurve, "none");
    }

    public void SetAttributeValue(string attributeName, float value, float fadeInDuration, CurveTypes fadeInCurve, params string[] ignoredParameters)
    {
        //find rtpc
        RTPCAttribute attribute = GetAttribute(attributeName);

        if (attribute == null) return;

        foreach (RTPC rtpc in attribute._parameters)
        {
            if (!ignoredParameters.Contains(rtpc.name))
            {
                SetValue(rtpc.name, value, fadeInDuration, fadeInCurve);
            }
        }
    }

    public void SetAttributeValue(string attributeName, float value, float fadeInDuration, CurveTypes fadeInCurve,
                         float fadeOutDuration, CurveTypes fadeOutCurve, float durationOfNewValue)
    {
        SetAttributeValue(attributeName, value, fadeInDuration, fadeInCurve, fadeOutDuration, fadeOutCurve, durationOfNewValue, "none");
    }

    public void SetAttributeValue(string attributeName, float value, float fadeInDuration, CurveTypes fadeInCurve,
                        float fadeOutDuration, CurveTypes fadeOutCurve, float durationOfNewValue, params string[] ignoredParameters)
    {
        //find rtpc
        RTPCAttribute attribute = GetAttribute(attributeName);

        if (attribute == null) return;

        foreach (RTPC rtpc in attribute._parameters)
        {
            if (!ignoredParameters.Contains(rtpc.name))
            {
                SetValue(rtpc.name, value, fadeInDuration, fadeInCurve, fadeOutDuration, fadeOutCurve, durationOfNewValue);
            }
        }
    }


    //ADD ATTRIBUTES
    public void AddAttributeValue(string attributeName, float value, float fadeInDuration, CurveTypes fadeInCurve)
    {
        AddAttributeValue(attributeName, value, fadeInDuration, fadeInCurve, "none");
    }

    public void AddAttributeValue(string attributeName, float value, float fadeInDuration, CurveTypes fadeInCurve, params string[] ignoredParameters)
    {
        //find rtpc
        RTPCAttribute attribute = GetAttribute(attributeName);

        if (attribute == null) return;

        foreach (RTPC rtpc in attribute._parameters)
        {
            if (!ignoredParameters.Contains(rtpc.name))
            {
                AddValue(rtpc.name, value, fadeInDuration, fadeInCurve);
            }
        }
    }

    public void AddAttributeValue(string attributeName, float value, float fadeInDuration, CurveTypes fadeInCurve,
                         float fadeOutDuration, CurveTypes fadeOutCurve, float durationOfNewValue)
    {
        AddAttributeValue(attributeName, value, fadeInDuration, fadeInCurve, fadeOutDuration, fadeOutCurve, durationOfNewValue, "none");
    }

    public void AddAttributeValue(string attributeName, float value, float fadeInDuration, CurveTypes fadeInCurve,
                        float fadeOutDuration, CurveTypes fadeOutCurve, float durationOfNewValue, params string[] ignoredParameters)
    {
        //find rtpc
        RTPCAttribute attribute = GetAttribute(attributeName);

        if (attribute == null) return;

        foreach (RTPC rtpc in attribute._parameters)
        {
            if (!ignoredParameters.Contains(rtpc.name))
            {
                AddValue(rtpc.name, value, fadeInDuration, fadeInCurve, fadeOutDuration, fadeOutCurve, durationOfNewValue);
            }
        }
    }








    //COROUTINES
    public IEnumerator Fade(RTPC rtpc, float newValue, float fadeDuration, AnimationCurve curve)
    {
        float startValue = rtpc.value;
        float progress = 0; //from 0->1
        rtpc.SetGoalValue(newValue);

        //FADING IN
        while (progress < 1)
        {
            //this will become 1 when its gone time equal to fadeDuration 
            if (fadeDuration <= 0) { progress = 1; }
            else { progress += (1 / fadeDuration) * Time.deltaTime; }

            //simply curve the progress based on the chosen curve
            float curvedProgress = curve.Evaluate(progress);

            //when progress = 1, value = newValue 
            float value = startValue + (newValue - startValue) * curvedProgress;

            AkSoundEngine.SetRTPCValue(rtpc.name, value);
            rtpc.SetValue(value);

            yield return null;
        }
        rtpc.isCoroutineRunning = false;
    }

    public IEnumerator Fade(RTPC rtpc, float newValue, float fadeInDuration, AnimationCurve fadeInCurve,
        float fadeOutDuration, AnimationCurve fadeOutCurve, float durationOfNewValue)
    {
        float startValue = rtpc.value;
        float progress = 0; //from 0->1
        rtpc.SetGoalValue(newValue);

        //FADING IN
        while (progress < 1)
        {
            //this will become 1 when its gone time equal to fadeDuration 
            if (fadeInDuration <= 0) { progress = 1; }
            else { progress += (1 / fadeInDuration) * Time.deltaTime; }

            //simply curve the progress based on the chosen curve
            float curvedProgress = fadeInCurve.Evaluate(progress);

            //when progress = 1, value = newValue 
            float value = startValue + (newValue - startValue) * curvedProgress;

            AkSoundEngine.SetRTPCValue(rtpc.name, value);
            rtpc.SetValue(value);

            yield return null;
        }

        //STAYING FOR A BIT
        progress = 0;
        while (progress < durationOfNewValue)
        {
            //just normal timer for how long the change is active
            progress += Time.deltaTime;

            yield return null;
        }

        //FADING OUT
        progress = 1;
        rtpc.SetGoalValue(rtpc.steadyValue);

        while (progress > 0)
        {
            //this will become 0 when its gone time equal to fadeDuration 
            if (fadeOutDuration <= 0) { progress = 0; }
            else { progress -= (1 / fadeOutDuration) * Time.deltaTime; }

            //simply curve the progress based on the chosen curve
            float curvedProgress = fadeOutCurve.Evaluate(progress);

            //this is reversed now so curvedProgress will start as 1 and become 0
            //when progress = 0, value = goalValue 
            float value = rtpc.goalValue + (newValue - rtpc.goalValue) * curvedProgress;

            AkSoundEngine.SetRTPCValue(rtpc.name, value);
            rtpc.SetValue(value);

            yield return null;
        }

        rtpc.isCoroutineRunning = false;
    }



    //GETS

    private RTPC GetParameter(string parameterName)
    {
        for (int a = 0; a < _attributes.Length; a++)
        {
            RTPC[] parameters = _attributes[a]._parameters;

            for (int p = 0; p < parameters.Length; p++)
            {
                if (parameters[p].name == parameterName)
                {
                    return parameters[p];
                }
            }
        }


        print("RTPC " + parameterName + " has not been added!");
        return null;
    }

    private RTPCAttribute GetAttribute(string attributeName)
    {
        for (int a = 0; a < _attributes.Length; a++)
        {
            if (_attributes[a].name == attributeName)
            {
                return _attributes[a];
            }
        }


        print("Attribute Group " + attributeName + " has not been added!");
        return null;
    }

    private AnimationCurve GetCurve(CurveTypes curveType)
    {
        if (curveType == CurveTypes.linear) { return _curves[0]; }
        else if (curveType == CurveTypes.high_curve) { return _curves[1]; }
        else { return _curves[2]; } //low curve

    }

}


