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
    private string fmodpara_Wavetype_Triangle = "tri";
    private string fmodpara_Camera = "camera";
    private string fmodpara_Color = "color";

    //Variables
    private float totalAmplitude;
    private float totalFrequency;
    private int[] videoSettings = {1, 1};

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
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Amplitude, 1);
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Frequency, UpdateFrequency(totalFrequency, globalControl.GlobalSpeed, -4, 4));
        UpdateWaveFormParameter();

        if (globalControl.CurrentCamera != videoSettings[0]){
            RuntimeManager.StudioSystem.setParameterByName(fmodpara_Camera, globalControl.CurrentCamera);
            videoSettings[0] = globalControl.CurrentCamera;
        }
        if (globalControl.CurrentColor != videoSettings[1]){
            RuntimeManager.StudioSystem.setParameterByName(fmodpara_Color, globalControl.CurrentColor);
            videoSettings[1] = globalControl.CurrentColor;
        }
    }

    private float UpdateFrequency(float newFMODValue, float currentValue, float minValue, float maxValue)
    {
        currentValue = Mathf.Lerp(0, 2, Mathf.InverseLerp(minValue, maxValue, currentValue));
        if (newFMODValue != currentValue) newFMODValue = currentValue;
        return newFMODValue;
    }

    private float ComputeBlendedValue(float sum, int count)
    {
        if (count == 0)
            return 0f;

        // Determine the maximum output value based on how many curves contribute
        // Ternary operator syntax: condition ? value_if_true : value_if_false
        float maxMapping = count == 1 ? 0.7f : (count == 2 ? 0.8f : 1.0f);

        // Using the average of the normalized amplitudes
        return (sum / count) * maxMapping;
    }

    private void UpdateWaveFormParameter()
    {
        // Normalize individual amplitudes
        float normA = Mathf.InverseLerp(0, 5, globalControl.AmplitudeA);
        float normB = Mathf.InverseLerp(0, 5, globalControl.AmplitudeB);
        float normC = Mathf.InverseLerp(0, 5, globalControl.AmplitudeC);

        float sinAmp = 0f;
        int sinCount = 0;
        float squAmp = 0f;
        int squCount = 0;
        float triAmp = 0f;
        int triCount = 0;
        float sawAmp = 0f;
        int sawCount = 0;

        
        switch (globalControl.WaveType[0])
        {
            case 0: sinAmp += normA; sinCount++; break;
            case 1: squAmp += normA; squCount++; break;
            case 2: triAmp += normA; triCount++; break;
            case 3: sawAmp += normA; sawCount++; break;
        }
        
        switch (globalControl.WaveType[1])
        {
            case 0: sinAmp += normB; sinCount++; break;
            case 1: squAmp += normB; squCount++; break;
            case 2: triAmp += normB; triCount++; break;
            case 3: sawAmp += normB; sawCount++; break;
        }
        
        switch (globalControl.WaveType[2])
        {
            case 0: sinAmp += normC; sinCount++; break;
            case 1: squAmp += normC; squCount++; break;
            case 2: triAmp += normC; triCount++; break;
            case 3: sawAmp += normC; sawCount++; break;
        }

        // map the value for each waveform
        float sinVal = ComputeBlendedValue(sinAmp, sinCount);
        float squVal = ComputeBlendedValue(squAmp, squCount);
        float triVal = ComputeBlendedValue(triAmp, triCount);
        float sawVal = ComputeBlendedValue(sawAmp, sawCount);

        // Update FMOD parameters
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Wavetype_Sinus, sinVal);
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Wavetype_Square, squVal);
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Wavetype_Triangle, triVal);
        RuntimeManager.StudioSystem.setParameterByName(fmodpara_Wavetype_SawTooth, sawVal);

        // print("sinewave " + sinVal);
        // print("squarewave " + squVal);
        // print("trianglewave " + triVal);
        // print("sawtoothwave " + sawVal);
    }

}
