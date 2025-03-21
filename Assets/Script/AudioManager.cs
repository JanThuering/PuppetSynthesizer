using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    //References to other scripts
    private GlobalControl globalControl;

    //FMOD
    private string fmodpara_Amplitude = "amp";
    private string fmodpara_Frequency = "freq";
    private string fmodpara_Wavetype_SawTooth = "saw";
    private string fmodpara_Wavetype_Sinus = "sin";
    private string fmodpara_Wavetype_Square = "squ";
    private string fmodpara_Wavetype_Triangle = "amp";

    //Variables
    private float totalAmplitude;
    private float totalFrequency;
    private float wavetypeSawTooth;
    private float wavetypeSinus;
    private float wavetypeSquare;
    private float wavetyüeTriangle;


    // Start is called before the first frame update
    void Start()
    {
        globalControl = GlobalControl.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Amplitude, UpdateAmplitude(totalAmplitude, globalControl.AmplitudeA + globalControl.AmplitudeB + globalControl.AmplitudeC, 10, 15));
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Frequency, UpdateFrequency(totalFrequency, globalControl.GlobalSpeed, -4, 4));
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Wavetype_Sinus, (float)globalControl.WaveType[1]);
    }

    private float UpdateAmplitude(float newFMODValue, float currentValue, float minValue, float maxValue)
    {
        currentValue = Mathf.InverseLerp(minValue, maxValue, currentValue);
        if (newFMODValue != currentValue) newFMODValue = currentValue;
        return newFMODValue;
    }

    private float UpdateFrequency(float newFMODValue, float currentValue, float minValue, float maxValue)
    {
        currentValue = Mathf.Lerp(0, 2, Mathf.InverseLerp(minValue, maxValue, currentValue));
        if (newFMODValue != currentValue) newFMODValue = currentValue;
        return newFMODValue;
    }

    //TODO Wave combinaion logic 
    private float UpdateWaveForm(float currentValue)
    {
        float newFMODValue = 0;

        if (currentValue == 1)
        {
            newFMODValue = wavetypeSinus;
        }
        //value from 1-4? if 1, return wavetype Sinus
        return newFMODValue;
    }
}
