using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    //References to other scripts
    private GlobalControl globalControl;

    //FMOD
    private EventInstance eventInstance;
    private string fmodpara_Amplitude = "amp";
    private string fmodpara_Frequency = "freq";
    private string fmodpara_Wavetype_SawTooth = "saw";
    private string fmodpara_Wavetype_Sinus = "sin";
    private string fmodpara_Wavetype_Square = "squ";
    private string fmodpara_Wavetype_Triangle = "amp";

    //Variables
    private float totalAmplitude;
    private float totalFrequency;

    // Start is called before the first frame update
    void Start()
    {
        globalControl = GlobalControl.Instance;
        eventInstance = RuntimeManager.CreateInstance("event:/waveforms_v0.2");
        eventInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Amplitude, UpdateAmplitude(totalAmplitude, globalControl.AmplitudeA + globalControl.AmplitudeB + globalControl.AmplitudeC, 10, 15));
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Frequency, UpdateFrequency(totalFrequency, globalControl.GlobalSpeed, -4, 4));
        UpdateWaveFormParameter();
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
    private void UpdateWaveFormParameter()
    {

        // Normalize individual amplitudes.
        // Assuming your amplitude values range from 0 to 5.
        float normA = Mathf.InverseLerp(0, 5, globalControl.AmplitudeA);
        float normB = Mathf.InverseLerp(0, 5, globalControl.AmplitudeB);
        float normC = Mathf.InverseLerp(0, 5, globalControl.AmplitudeC);

        // Prepare accumulators for each FMOD waveform parameter.
        float sinAmp = 0f;
        float squAmp = 0f;
        float triAmp = 0f;
        float sawAmp = 0f;

        // Curve A (index 0)
        switch (globalControl.WaveType[0])
        {
            case 0: sinAmp += normA; break;
            case 1: squAmp += normA; break;
            case 2: triAmp += normA; break;
            case 3: sawAmp += normA; break;
        }

        // Curve B (index 1)
        switch (globalControl.WaveType[1])
        {
            case 0: sinAmp += normB; break;
            case 1: squAmp += normB; break;
            case 2: triAmp += normB; break;
            case 3: sawAmp += normB; break;
        }

        // Curve C (index 2)
        switch (globalControl.WaveType[2])
        {
            case 0: sinAmp += normC; break;
            case 1: squAmp += normC; break;
            case 2: triAmp += normC; break;
            case 3: sawAmp += normC; break;
        }

        // Optionally, clamp to ensure the FMOD parameter is within [0,1] in case multiple curves add up.
        sinAmp = Mathf.Clamp01(sinAmp);
        squAmp = Mathf.Clamp01(squAmp);
        triAmp = Mathf.Clamp01(triAmp);
        sawAmp = Mathf.Clamp01(sawAmp);

        // Now update the FMOD parameters with the corresponding normalized amplitude.
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Wavetype_Sinus, sinAmp);
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Wavetype_Square, squAmp);
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Wavetype_Triangle, triAmp);
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Wavetype_SawTooth, sawAmp);
    }

}
