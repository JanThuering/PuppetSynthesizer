using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{
    [Header("References to parameters")]

    //ANIMATIONS
    [SerializeField] private PuppetAnimation puppetAnimation;
    private float animationMultiplier; //effects the amount of movement of the puppet
    private float scaleMultiplier; //effects the amount of scaling of the controlpoints

    //WAVE
    [SerializeField] private CreateLine createLine;
    private float globalAmplitude;
    private float globalFrequency;
    private float globalSpeed;
    //wave - curveA
    private float amplitudeA;
    private float speedA;
    //wave - curveB
    private float amplitudeB;
    private float speedB;
    //wave - curveC
    private float amplitudeC;
    private float speedC;
    //wave - curveD
    private float amplitudeD;
    private float speedD;

    // Start is called before the first frame update
    void Start()
    {
        //puppet
        animationMultiplier = puppetAnimation.MovementMultiplier;
        scaleMultiplier = puppetAnimation.ScaleMultiplier;
        //wave - global values
        globalAmplitude = createLine.GlobalAmplitude;
        globalFrequency = createLine.GlobalFrequency;
        globalSpeed = createLine.GlobalSpeed;
        //wave - curveA
        amplitudeA = createLine.AmplitudeA;
        speedA = createLine.SpeedA;
        //wave - curveB
        amplitudeB = createLine.AmplitudeB;
        speedB = createLine.SpeedB;
        //wave - curveC
        amplitudeC = createLine.AmplitudeC;
        speedC = createLine.SpeedC;
        //wave - curveD
        amplitudeD = createLine.AmplitudeD;
        speedD = createLine.SpeedD;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
