using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class CreateLine : MonoBehaviour
{
    // Singleton instance
    private static CreateLine instance;
    public static CreateLine Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CreateLine>();
                if (instance == null)
                {
                    GameObject go = new GameObject("CreateLine");
                    instance = go.AddComponent<CreateLine>();
                }
            }
            return instance;
        }
    }

    [Header ("CHECKS")]
    [SerializeField] private bool segmentedCurves = true;

    [Header ("EXTERNAL REFERENCES")]
    private LineRenderer lineRenderer;
    [HideInInspector] public GameObject LineStart;
    public GameObject LineEnd;
    public Vector3 LineDirection { get { return (LineEnd.transform.position - LineStart.transform.position).normalized; } }



    [Header ("ANIMATION VALUES")]
    [SerializeField] private Vector3 movement = new Vector3(0,1,0);

    [Range(-5, 5)]
    [SerializeField] private float totalAmplitude = 1;
    [Range(1f, 10)]
    public float frequency = 1;
    [Range(0f, 5)]
    public float horizontalMovement = 10;

    [Header ("INTERFACE VALUES")]
    [SerializeField] private int sliderAmmount = 3;

    [Header ("Points")]
    [SerializeField] private int pointsCount = 300;
    public GameObject[] pointsArray;
    private Vector3[] startPointLocation;


    [Header ("CURVES")]
    [SerializeField] private AnimationCurve [] curveTypes;
    [SerializeField] private AnimationCurve [] curveArray;
    [SerializeField] private float [] amplitudeArray;
    [SerializeField] private float [] speedArray;



    [Range(-5, 5)]
    [SerializeField] float amplitudeA = 1;
    [Range(-5, 5)]
    [SerializeField] float speedA = 1;


    [Range(-5, 5)]
    [SerializeField] float amplitudeB = 1;
    [Range(-5, 5)]
    [SerializeField] float speedB = 1;


    [Range(-5, 5)]
    [SerializeField] float amplitudeC = 1;
    [Range(-5, 5)]
    [SerializeField] float speedC = 1;


    [Range(-5, 5)]
    [SerializeField] float amplitudeD = 1;
    [Range(-5, 5)]
    [SerializeField] float speedD = 1;

    [Range(-5, 5)]
    [SerializeField] float amplitudeE = 1;
    [Range(-5, 5)]
    [SerializeField] float speedE = 1;

    [Range(-5, 5)]
    [SerializeField] float amplitudeF = 1;
    [Range(-5, 5)]
    [SerializeField] float speedF = 1;
    

    // Start is called before the first frame update
    private void Awake()
    {
        // Ensure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        

    }

    private void Start()
    {
        GetVariables();
        // Existing Awake functionality
        CreatePoints();
        CreateArrays();
    }

    // Update is called once per frame
    void Update()
    { 
        FillLineRenderer(); //fill the line renderer with the points
        if(segmentedCurves){    //old curvesystem
            SegmentedCurve();

        }
        else{  //new curvesystem
            FillCurveArray(); //fill the curve array with the curves and amplitudes
            ControlGlobalCurve(); //Move the points in the defined axis, acording to the curves
        }

    }

    private void SegmentedCurve(){
            // Move the points in the defined axis, acording to the curves
            ControlSegmentedCurve(1, curveArray[0], "Curve A", amplitudeArray[0], frequency);
            ControlSegmentedCurve(2, curveArray[1], "Curve B", amplitudeArray[1], frequency);
            ControlSegmentedCurve(3, curveArray[2], "Curve C", amplitudeArray[2], frequency);
            ControlSegmentedCurve(4, curveArray[3], "Curve D", amplitudeArray[3], frequency);
            ControlSegmentedCurve(5, curveArray[4], "Curve E", amplitudeArray[4], frequency);
            ControlSegmentedCurve(6, curveArray[5], "Curve F", amplitudeArray[5], frequency);
    }

    private void CreateArrays(){
        curveArray = new AnimationCurve[6];
        amplitudeArray = new float[6];
        speedArray = new float[6];

        for(int i = 0; i < curveArray.Length; i++){
            curveArray[i] = curveTypes[1];
        }


    }

    private void FillCurveArray(){
        //Fills the arrays with the curves and amplitudes
        //easiere access to the values for the MIDI input

        amplitudeArray[0] = amplitudeA;
        speedArray[0] = speedA;


        amplitudeArray[1] = amplitudeB;
        speedArray[1] = speedB;


        amplitudeArray[2] = amplitudeC;
        speedArray[2] = speedC;


        amplitudeArray[3] = amplitudeD;
        speedArray[3] = speedD;


        amplitudeArray[4] = amplitudeE;
        speedArray[4] = speedE;


        amplitudeArray[5] = amplitudeF;
        speedArray[5] = speedF;
    }
    private void CreatePoints(){    
        //check if (points count + 2) is divisible by slider ammount
        if((pointsCount+2)%sliderAmmount != 0){
            //if not, add points until it is
            for(int i = 0; (pointsCount+2)%sliderAmmount != 0; i++){
                pointsCount++;
            }
        }
        //initialize array of points
        pointsArray = new GameObject[pointsCount+2];
        startPointLocation = new Vector3[pointsCount+2];

        //calculate distance between line start and line end
        Vector3 distancePoints = (LineEnd.transform.position - LineStart.transform.position) / (pointsArray.Length-1);

        //create parent for points
        GameObject pointParent = new GameObject("Point Parent");

        //create points
        for(int i = 0; i < pointsArray.Length; i++)
        {
            if(i == 0){
                pointsArray[i] = LineStart;
                startPointLocation[i] = LineStart.transform.position;
                pointsArray[i].transform.SetParent(pointParent.transform);
            }
            else if(i == pointsArray.Length-1){
                pointsArray[i] = LineEnd;
                startPointLocation[i] = LineEnd.transform.position;
                pointsArray[i].transform.SetParent(pointParent.transform);
            }
            else{
                pointsArray[i] = new GameObject("Point " + i);
                pointsArray[i].transform.position = LineStart.transform.position + distancePoints * i;
                startPointLocation[i] = pointsArray[i].transform.localPosition;
                pointsArray[i].transform.SetParent(pointParent.transform);
            }
        }

    }
    private void GetVariables(){
        lineRenderer = GetComponent<LineRenderer>();
        if(lineRenderer == null){
            Debug.LogError("LineRenderer not found");
        }
        LineStart = GameObject.Find("LineStart");
        if(LineStart == null){
            Debug.LogError("LineStart not found");
        }
        LineEnd = GameObject.Find("LineEnd");
        if(LineEnd == null){
            Debug.LogError("LineEnd not found");
        }
    }
    public void FillLineRenderer()
    {
        //initialize line renderer
        lineRenderer.positionCount = pointsArray.Length;

        //set points to line renderer
        for(int i = 0; i < lineRenderer.positionCount; i++){
            lineRenderer.SetPosition(i, pointsArray[i].transform.position);
        }
    }

    public void MidiDefineWaveType(int controlNumber, float controlValue, float valueAmmount){
        /*VALUE EXPLANATION
            controlNumber -> slider (for which slider the curve is)
            controlValue -> waveType (which curve is selected)
            valueAmmount -> max value of the slider (how to distribute the waveTypes on the values)
        */

        //float increments = controlValue / valueAmmount * 10 - 5;
        int curveTypeIndex;
        float steps = valueAmmount / sliderAmmount;
        int slider = 1;

        switch(controlNumber){
            case 21: slider = 1; break;
            case 22: slider = 2; break;
            case 23: slider = 3; break;
            case 24: slider = 4; break;
            case 25: slider = 5; break;
            case 26: slider = 6; break;

        }

        //assign curve to slider depending on value
        for(int i = 1; i <= sliderAmmount; i++){
            if(controlValue > (steps*i - steps) && controlValue < (steps*i)){
                curveTypeIndex = i-1;
                curveArray[slider-1] = curveTypes[curveTypeIndex];
            }

        }

    }
    
    
    public void MidiControlWave(int controlNumber, float controlValue, float valueAmmount){
        /*VALUE EXPLANATION
            controlNumber -> segment (which curve is selected)
            controlValue -> waveType (amplitude or speed of the curve)
            valueAmmount -> max value of the slider (how to distribute the waveTypes on the values)
        */
        
        float increments = controlValue / valueAmmount * 10 - 5;    //calculate the value of the slider / knob

        switch(controlNumber){
            case 1: amplitudeA = increments; break;
            case 2: amplitudeB = increments; break;
            case 3: amplitudeC = increments; break;
            case 4: amplitudeD = increments; break;
            case 5: amplitudeE = increments; break;
            case 6: amplitudeF = increments; break;
            case 11: speedA = increments; break;
            case 12: speedB = increments; break;
            case 13: speedC = increments; break;
            case 14: speedD = increments; break;
            case 15: speedE = increments; break;
            case 16: speedF = increments; break;
        }

    }

    //SEGMENTED CURVES
    private void ControlSegmentedCurve(int slider, AnimationCurve curve, string curveName, float amplitude, float frequency){
        /*EXPLANATION
            slider -> which slider is selected
            curve -> the curve that is selected
            curveName -> the name of the curve
            amplitude -> the amplitude of the curve
            frequency -> the frequency of the curve
        */

        //counts up so the wave moves up the index
        float timeCounter = Time.time*horizontalMovement*100;

        //calculate range of points
        int range = pointsArray.Length / sliderAmmount;
        int start = range * (slider-1);
        int end = range * slider;

        //move points in the y axis, according to the curve
        for(int i = start+(int)timeCounter; i < end+(int)timeCounter; i++){
           
            int currentIndex = i%pointsArray.Length;
            
            // Normalize evaluation value between 0 and 1 for current segment
            float normalizedT = ((float)(i - (start+(int)timeCounter)) / range) * frequency;

            // Use modulo to wrap value between 0-1
            normalizedT = normalizedT % 1.0f;
            
            float curveValue = curve.Evaluate(normalizedT) * amplitude * totalAmplitude;

            Vector3 right = pointsArray[currentIndex].transform.right * curveValue * this.movement.x;
            Vector3 up = pointsArray[currentIndex].transform.up * curveValue * this.movement.y;
            Vector3 forward = pointsArray[currentIndex].transform.forward * curveValue * this.movement.z;
            Vector3 movement = up + right + forward;

            if(currentIndex == 0 || currentIndex == pointsArray.Length-1){ //dont move start and end point
                movement = new Vector3(0,0,0);
            }
            else{   //move all other points
                pointsArray[currentIndex].transform.position =  startPointLocation[currentIndex] + movement;
            }
        }
 
    }

    //NEW CURVESYSTEM
    private void ControlGlobalCurve(){
        // counts up so the wave moves up the index
        float timeCounter = Time.time * horizontalMovement;

        // Move points in the defined axis, according to the curves
        for (int i = 0; i < pointsArray.Length; i++) {
            int currentIndex = i % pointsArray.Length;
            float combinedCurveValue = 0;

            //Combine all curves
            for (int j = 0; j < sliderAmmount ; j++) {  // Evaluate the curve based on the current index and frequency
                float curveEvaluation = curveArray[j].Evaluate((currentIndex / (float)pointsArray.Length) * frequency + timeCounter * speedArray[j]);
                combinedCurveValue += curveEvaluation * amplitudeArray[j] * totalAmplitude;
            }

            //Move in defined direction
            this.movement = this.movement.normalized;
            Vector3 right = pointsArray[currentIndex].transform.right * combinedCurveValue * this.movement.x;
            Vector3 up = pointsArray[currentIndex].transform.up * combinedCurveValue * this.movement.y;
            Vector3 forward = pointsArray[currentIndex].transform.forward * combinedCurveValue * this.movement.z;
            Vector3 movement = up + right + forward;

            //Apply movement
            if (currentIndex == 0 || currentIndex == pointsArray.Length - 1) { // don't move start and end point
                movement = Vector3.zero;
            } else {
                pointsArray[currentIndex].transform.position = startPointLocation[currentIndex] + movement;
            }
        }
    }

}
