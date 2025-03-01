using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class GlobalControl : MonoBehaviour
{

    //Singleton Instance
    public static GlobalControl Instance {get; private set;}

    [Header("EXTERNAL REFERENCES")]
    //ANIMATIONS-PARAMETERS
    [SerializeField] private PuppetAnimation puppetAnimation;
    [SerializeField] private CreateLine createLineScript;
    private float animationMultiplier; //effects the amount of movement of the puppet
    private float scaleMultiplier; //effects the amount of scaling of the controlpoints


    [Header("DEFAULT VALUES")]
    private float lastChangeTime;
    private bool valuesChanged = false;
    private float timeToDefault = 10.0f;
    private float defaultGlobalSpeed = 0.3f;
    private float defaultGlobalFrequency = 1.0f;
    private float defaultAmplitude = 0.1f;
    private float defaultFrequency = 1.0f;



    [Header("LIMITS")]
    [Header("Amplitude")]
    private float minAmplitude = 0;
    private float maxAmplitude = 5;
    private float maxGlobalAmplitude = 2.4f;

    [Header("Frequency")]
    private float minFrequency = 1;
    private float maxFrequency = 5;

    [Header("Speed")]
    private float minSpeed = 0f;
    private float maxSpeed = 2.5f;



    //WAVE-PARAMETERS
    [Header("GLOBAL WAVE")]
    [SerializeField] private float globalFrequency;
    [SerializeField] private float globalSpeed;
    [HideInInspector]
    [SerializeField] private float globalAmplitude;
    [Range(0, 3)]
    [SerializeField] private int [] waveType =  {0, 0, 0}; 

    public int [] WaveType
    {
        get => waveType;
        set => waveType = value;
    }
    public float GlobalAmplitude
    {
        get => globalAmplitude;
        set => globalAmplitude = Mathf.Clamp(value, 1, maxGlobalAmplitude);
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
    [Range(0, 5)]
    [SerializeField] private float amplitudeA;
    [Range(0, 2.5f)]
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
    [Range(0, 5)]
    [SerializeField] private float amplitudeB;
    [Range(0, 2.5f)]
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
    [Range(0, 5)]
    [SerializeField] private float amplitudeC;
    [Range(0, 2.5f)]
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
        if(Time.time-lastChangeTime > timeToDefault) {
            ResetToDefaultValues();
            ValuesChanged(false);
        }
    }

    private void SetStartValues(){
        createLineScript = CreateLine.Instance;

        GlobalAmplitude = 1.8f;
        GlobalFrequency = 1;
        GlobalSpeed = 1;

        AmplitudeA = 1;
        SpeedA = 1;

        AmplitudeB = 1;
        SpeedB = 1;

        AmplitudeC = 1;
        SpeedC = 1;

    }

    private void ResetToDefaultValues(){
        Debug.Log("Resetting to default values");
        
        float timeToLerp = 1.0f * Time.deltaTime;

        //global values
        GlobalFrequency = Mathf.Lerp(GlobalFrequency, defaultGlobalFrequency, timeToLerp);
        GlobalSpeed = Mathf.Lerp(GlobalSpeed, defaultGlobalSpeed, timeToLerp);

        //individual values
        AmplitudeA = Mathf.Lerp(AmplitudeA, defaultAmplitude, timeToLerp);
        SpeedA = Mathf.Lerp(SpeedA, defaultFrequency, timeToLerp);
        AmplitudeB = Mathf.Lerp(AmplitudeB, defaultAmplitude, timeToLerp);
        SpeedB = Mathf.Lerp(SpeedB, defaultFrequency, timeToLerp);
        AmplitudeC = Mathf.Lerp(AmplitudeC, defaultAmplitude, timeToLerp);
        SpeedC = Mathf.Lerp(SpeedC, defaultFrequency, timeToLerp);
    }

    private void ValuesChanged(bool valueChanged){
        if(valueChanged){
            valuesChanged = valueChanged;
            lastChangeTime = Time.time;
        }
        if(!valueChanged){
            valuesChanged = valueChanged;
        }
    }

    public float GetMaxAmplitude(){
        return maxAmplitude;
    }

    public void MidiGlobalWave(int controlNumber, float controlValue, float valueAmmount){
        //check activity
        ValuesChanged(true);

        /*VALUE EXPLANATION
            controlNumber -> slider (for which slider the curve is)
            controlValue -> waveType (amplitude of the curve)
            valueAmmount -> max value of the slider (how to distribute the waveTypes on the values)
        */
        switch(controlNumber){
            //case 7: GlobalAmplitude = controlValue / valueAmmount * (maxGlobalAmplitude-minAmplitude) + minAmplitude; break;
            case 8: GlobalFrequency = controlValue / valueAmmount * (maxFrequency-minFrequency) + minFrequency; break;
            case 9: GlobalSpeed = controlValue / valueAmmount * (maxSpeed-(-maxSpeed)) + (-maxSpeed); break;
        }



    }

    public void MidiAmplitudeWave(int controlNumber, float controlValue, float maxValue){
        //check activity
        ValuesChanged(true);

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
        }


    }

    public void MidiSpeedWave(int controlNumber, float controlValue, float valueAmmount){
        //check activity
        ValuesChanged(true);

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
        }



    }

    public void MidiWaveType(int controlNumber, float controlValue, float valueAmmount, bool fader){
        //check activity
        ValuesChanged(true);
        
        /*VALUE EXPLANATION
            controlNumber -> slider (for which slider the curve is)
            controlValue -> waveType (amplitude of the curve)
            valueAmmount -> max value of the slider (how to distribute the waveTypes on the values)
        */
        
        int index = 0;
        
        //set the index of the waveType
        if(fader){
            switch(controlNumber){
                case 15: index = 0; break;
                case 16: index = 1; break;
                case 17: index = 2; break;
                case 18: index = 3; break;
            }

            float steps =  valueAmmount / createLineScript.SliderAmount;

            for( int i = 1; i <= createLineScript.curveTypes.Length; i++){
                if(controlValue >= (steps*i - steps) && controlValue <= (steps*i)){
                    if(i > createLineScript.curveTypes.Length){
                        i = createLineScript.curveTypes.Length;
                    }
                    waveType[index] = i-1;
                }
            }
        }

        if(!fader){
            switch(controlNumber){
                case 32: index = 0; break;
                case 33: index = 1; break;
                case 34: index = 2; break;
                case 35: index = 3; break;
            }
            //switch to next wavetype
            if(controlValue == 0){
                WaveType[index] += 1;
                if(WaveType[index] > 3){
                    WaveType[index] = 0;
                }
            }
        }

    }



}
