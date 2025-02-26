using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;

public class GlobalControl : MonoBehaviour
{

    //Singleton Instance
    public static GlobalControl Instance {get; private set;}

    [Header("EXTERNAL REFERENCES")]
    //ANIMATIONS-PARAMETERS
    [SerializeField] private PuppetAnimation puppetAnimation;
    [SerializeField] private CreateLine createLine;
    private float animationMultiplier; //effects the amount of movement of the puppet
    private float scaleMultiplier; //effects the amount of scaling of the controlpoints

    [Header("LIMITS")]
    [Header("Amplitude")]
    [SerializeField] private float minAmplitude = 0;
    [SerializeField] private float maxAmplitude = 5;
    [Header("Frequency")]
    [SerializeField] private float minFrequency = 1;
    [SerializeField] private float maxFrequency = 10;
    [Header("Speed")]
    [SerializeField] private float minSpeed = 1;
    [SerializeField] private float maxSpeed = 5;



    //WAVE-PARAMETERS
    [Header("GLOBAL WAVE")]
    [SerializeField] private float globalAmplitude;
    [SerializeField] private float globalFrequency;
    [SerializeField] private float globalSpeed;
    [SerializeField] private int [] waveType =  {0, 0, 0, 0}; 

    public int [] WaveType
    {
        get => waveType;
        set => waveType = value;
    }

    public float GlobalAmplitude
    {
        get => globalAmplitude;
        set => globalAmplitude = Mathf.Clamp(value, minAmplitude, maxAmplitude);
    }
    public float GlobalFrequency
    {
        get => globalFrequency;
        set => globalFrequency = Mathf.Clamp(value, minFrequency, maxFrequency);
    }
    public float GlobalSpeed
    {
        get => globalSpeed;
        set => globalSpeed = Mathf.Clamp(value, minSpeed, maxSpeed);
    }


    //wave - curveA
    [Header ("Curve A")]
    [SerializeField] private float amplitudeA;
    [SerializeField] private float speedA;
    public float AmplitudeA
    {
        get => amplitudeA;
        set => amplitudeA = Mathf.Clamp(value, minAmplitude, maxAmplitude);
    }
    public float SpeedA
    {
        get => speedA;
        set => speedA = Mathf.Clamp(value, minSpeed, maxSpeed);
    }
    //wave - curveB
    [Header ("Curve B")]
    [SerializeField] private float amplitudeB;
    [SerializeField] private float speedB;
    public float AmplitudeB
    {
        get => amplitudeB;
        set => amplitudeB = Mathf.Clamp(value, minAmplitude, maxAmplitude);
    }
    public float SpeedB
    {
        get => speedB;
        set => speedB = Mathf.Clamp(value, minSpeed, maxSpeed);
    }
    //wave - curveC
    [Header ("Curve C")]
    [SerializeField] private float amplitudeC;
    [SerializeField] private float speedC;
    public float AmplitudeC
    {
        get => amplitudeC;
        set => amplitudeC = Mathf.Clamp(value, minAmplitude, maxAmplitude);
    }
    public float SpeedC
    {
        get => speedC;
        set => speedC = Mathf.Clamp(value, minSpeed, maxSpeed);
    }
    //wave - curveD
    [Header ("Curve D")]
    [SerializeField] private float amplitudeD;
    [SerializeField] private float speedD;
    public float AmplitudeD
    {
        get => amplitudeD;
        set => amplitudeD = Mathf.Clamp(value, minAmplitude, maxAmplitude);
    }
    public float SpeedD
    {
        get => speedD;
        set => speedD = Mathf.Clamp(value, minSpeed, maxSpeed);
    }


    private void Awake()
    {
        if(Instance != null && Instance != this){
            Destroy(this);
        }else{
            Instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        SetStartValues();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetStartValues(){
        GlobalAmplitude = 1;
        GlobalFrequency = 1;
        GlobalSpeed = 1;

        AmplitudeA = 1;
        SpeedA = 1;

        AmplitudeB = 1;
        SpeedB = 1;

        AmplitudeC = 1;
        SpeedC = 1;

        AmplitudeD = 1;
        SpeedD = 1;
    }

    public float GetMaxAmplitude(){
        return maxAmplitude;
    }

    public void MidiGlobalWave(int controlNumber, float controlValue, float valueAmmount){
        /*VALUE EXPLANATION
            controlNumber -> slider (for which slider the curve is)
            controlValue -> waveType (amplitude of the curve)
            valueAmmount -> max value of the slider (how to distribute the waveTypes on the values)
        */
        switch(controlNumber){
            case 7: GlobalAmplitude = controlValue / valueAmmount * (maxAmplitude-minAmplitude) + minAmplitude; break;
            case 8: GlobalFrequency = controlValue / valueAmmount * (maxFrequency-minFrequency) + minFrequency; break;
            case 9: GlobalSpeed = controlValue / valueAmmount * (maxSpeed-minSpeed) + minSpeed; break;
        }

    }

    public void MidiAmplitudeWave(int controlNumber, float controlValue, float maxValue){
        /*VALUE EXPLANATION
            controlNumber -> segment (which curve is selected)
            controlValue -> waveType (amplitude or speed of the curve)
            maxValue -> max value of the slider (how to distribute the waveTypes on the values)
        */
        float increments = controlValue / maxValue * (maxAmplitude-minAmplitude) + minAmplitude;    //calculate the value of the slider / knob

        switch(controlNumber){
            case 1: AmplitudeA = increments; break;
            case 2: AmplitudeB = increments; break;
            case 3: AmplitudeC = increments; break;
            case 4: AmplitudeD = increments; break;
        }
    }

    public void MidiSpeedWave(int controlNumber, float controlValue, float valueAmmount){
        /*VALUE EXPLANATION
            controlNumber -> slider (for which slider the curve is)
            controlValue -> waveType (amplitude of the curve)
            valueAmmount -> max value of the slider (how to distribute the waveTypes on the values)
        */

        float increments = controlValue / valueAmmount * (maxSpeed-minSpeed) + minSpeed;    //calculate the value of the slider / knob

        switch(controlNumber){
            case 10: speedA = increments; break;
            case 11: speedB = increments; break;
            case 12: speedC = increments; break;
            case 13: speedD = increments; break;
        }

    }



}
