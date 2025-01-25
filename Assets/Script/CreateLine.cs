using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class CreateLine : MonoBehaviour
{
    [Header ("DEBUGGING")]
    [SerializeField] private bool _debug = true;
    [SerializeField] private bool segmentedCurves = false;

    [Header ("EXTERNAL REFERENCES")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private GameObject _lineStart;
    [SerializeField] private GameObject _lineEnd;

    [Header ("ANIMATION VALUES")]
  
    [SerializeField] private Vector3 _movement = new Vector3(0,1,0);

    [Range(-5, 5)]
    [SerializeField] private float _totalAmplitude = 1;
    [Range(1f, 10)]
    public float _frequency = 1;
    [Range(0f, 5)]
    public float _horizontalMovement = 10;

    [Header ("CURVES")]
    [SerializeField] private AnimationCurve [] _curveArray;
    [SerializeField] private float [] _amplitudeArray;
    [SerializeField] private float [] _speedArray;


    [SerializeField] private AnimationCurve _curveA;
    [Range(-5, 5)]
    [SerializeField] float _amplitudeA = 1;
    [Range(-5, 5)]
    [SerializeField] float _speedA = 1;

    [SerializeField] private AnimationCurve _curveB;
    [Range(-5, 5)]
    [SerializeField] float _amplitudeB = 1;
    [Range(-5, 5)]
    [SerializeField] float _speedB = 1;

    [SerializeField] private AnimationCurve _curveC;
    [Range(-5, 5)]
    [SerializeField] float _amplitudeC = 1;
    [Range(-5, 5)]
    [SerializeField] float _speedC = 1;

    [SerializeField] private AnimationCurve _curveD;
    [Range(-5, 5)]
    [SerializeField] float _amplitudeD = 1;
    [Range(-5, 5)]
    [SerializeField] float _speedD = 1;

    [SerializeField] private AnimationCurve _curveE;
    [Range(-5, 5)]
    [SerializeField] float _amplitudeE = 1;
    [Range(-5, 5)]
    [SerializeField] float _speedE = 1;

    [SerializeField] private AnimationCurve _curveF;
    [Range(-5, 5)]
    [SerializeField] float _amplitudeF = 1;
    [Range(-5, 5)]
    [SerializeField] float _speedF = 1;
    
    [Header ("INTERFACE VALUES")]
    [SerializeField] private int _sliderAmmount = 3;

    [Header ("Points")]
    [SerializeField] private int _pointsCount = 2;
    public GameObject[] _pointsArray;
    private Vector3[] _startPointLocation;

    /*TODO
    1. x curve sequence should move along the line
        (orient around the two endingpoints)
    2. combine curve
    3. Input with MIDI


    - x 3D Linie (in alle Vectoren movement einbauen)
    - Kurven segmente interpolieren
        (immer die selbe Frequenz so dass Form der Kurve ersichtlich bleibt)
    - Speed Horizontal der Kurve muss zur Wellenfrequenzpassen

    - Midi
    - DryWetMidi

    - Wellen kombinieren

    */


    // Start is called before the first frame update
    void Start()
    {
        CreatePoints();
        
    }

    // Update is called once per frame
    void Update()
    { 
        FillLineRenderer(); //fill the line renderer with the points
        if(segmentedCurves){    //old curvesystem
            // Move the points in the defined axis, acording to the curves
            ControlCurve(1, _curveA, "Curve A", _amplitudeA, _frequency);
            ControlCurve(2, _curveB, "Curve B", _amplitudeB, _frequency);
            ControlCurve(3, _curveC, "Curve C", _amplitudeC, _frequency);
            ControlCurve(4, _curveD, "Curve D", _amplitudeD, _frequency);
            ControlCurve(5, _curveE, "Curve E", _amplitudeE, _frequency);
            ControlCurve(6, _curveF, "Curve F", _amplitudeF, _frequency);

        }else{  //new curvesystem
            FillCurveArray(); //fill the curve array with the curves and amplitudes
            ControlGlobalCurve(); //Move the points in the defined axis, acording to the curves
        }



    }

    private void FillCurveArray(){
        //Fills the arrays with the curves and amplitudes
        //easiere access to the values for the MIDI input
        _curveArray = new AnimationCurve[7];
        _amplitudeArray = new float[7];
        _speedArray = new float[7];

        _amplitudeArray [0] = _totalAmplitude;
        _speedArray[0] = _horizontalMovement;

        _curveArray[1] = _curveA;
        _amplitudeArray[1] = _amplitudeA;
        _speedArray[1] = _speedA;

        _curveArray[2] = _curveB;
        _amplitudeArray[2] = _amplitudeB;
        _speedArray[2] = _speedB;

        _curveArray[3] = _curveC;
        _amplitudeArray[3] = _amplitudeC;
        _speedArray[3] = _speedC;

        _curveArray[4] = _curveD;
        _amplitudeArray[4] = _amplitudeD;
        _speedArray[4] = _speedD;

        _curveArray[5] = _curveE;
        _amplitudeArray[5] = _amplitudeE;
        _speedArray[5] = _speedE;

        _curveArray[6] = _curveF;  
        _amplitudeArray[6] = _amplitudeF;
        _speedArray[6] = _speedF;
    }

    private void CreatePoints(){    
        //check if (points count + 2) is divisible by slider ammount
        if((_pointsCount+2)%_sliderAmmount != 0){
            //if not, add points until it is
            for(int i = 0; (_pointsCount+2)%_sliderAmmount != 0; i++){
                _pointsCount++;
            }
        }
        //initialize array of points
        _pointsArray = new GameObject[_pointsCount+2];
        _startPointLocation = new Vector3[_pointsCount+2];

        //calculate distance between line start and line end
        Vector3 distancePoints = (_lineEnd.transform.position - _lineStart.transform.position) / (_pointsArray.Length-1);

        //create parent for points
        GameObject pointParent = new GameObject("Point Parent");

        //create points
        for(int i = 0; i < _pointsArray.Length; i++)
        {
            if(i == 0){
                _pointsArray[i] = _lineStart;
                _startPointLocation[i] = _lineStart.transform.position;
                _pointsArray[i].transform.SetParent(pointParent.transform);
            }
            else if(i == _pointsArray.Length-1){
                _pointsArray[i] = _lineEnd;
                _startPointLocation[i] = _lineEnd.transform.position;
                _pointsArray[i].transform.SetParent(pointParent.transform);
            }
            else{
                _pointsArray[i] = new GameObject("Point " + i);
                _pointsArray[i].transform.position = _lineStart.transform.position + distancePoints * i;
                _startPointLocation[i] = _pointsArray[i].transform.localPosition;
                _pointsArray[i].transform.SetParent(pointParent.transform);
            }
        }

    }

    public void FillLineRenderer()
    {
        //initialize line renderer
        _lineRenderer.positionCount = _pointsArray.Length;

        //set points to line renderer
        for(int i = 0; i < _lineRenderer.positionCount; i++){
            _lineRenderer.SetPosition(i, _pointsArray[i].transform.position);
        }
    }

    public void InputToValues(int controlNumber, float controlValue){
        AnimationCurve curve = _curveArray[controlNumber];
        _amplitudeArray[controlNumber] = controlValue; 
    }

    private void ControlCurve(int slider, AnimationCurve curve, string curveName, float amplitude, float frequency){
        //check if slider is in range
        if(slider > _sliderAmmount || slider < 1){
            if(_debug){
            Debug.LogWarning("Slider out of range. Curve " + curveName + " will be ignored.");
            }
            return;
        }

        //counts up so the wave moves up the index
        float timeCounter = Time.time*_horizontalMovement*100;

        //calculate range of points
        int range = _pointsArray.Length / _sliderAmmount;
        int start = range * (slider-1);
        int end = range * slider;

        //move points in the y axis, according to the curve
        for(int i = start+(int)timeCounter; i < end+(int)timeCounter; i++){
           
            int currentIndex = i%_pointsArray.Length;
            
            // Normalize evaluation value between 0 and 1 for current segment
            float normalizedT = ((float)(i - (start+(int)timeCounter)) / range) * frequency;

            // Use modulo to wrap value between 0-1
            normalizedT = normalizedT % 1.0f;
            
            float curveValue = curve.Evaluate(normalizedT) * amplitude * _totalAmplitude;
            
            Vector3 right = _pointsArray[currentIndex].transform.right * curveValue * _movement.x;
            Vector3 up = _pointsArray[currentIndex].transform.up * curveValue * _movement.y;
            Vector3 forward = _pointsArray[currentIndex].transform.forward * curveValue * _movement.z;
            Vector3 movement = up + right + forward;

            if(currentIndex == 0 || currentIndex == _pointsArray.Length-1){ //dont move start and end point
                movement = new Vector3(0,0,0);
            }
            else{   //move all other points
                _pointsArray[currentIndex].transform.position =  _startPointLocation[currentIndex] + movement;
            }
        }
 
    }


    private void ControlGlobalCurve(){
        // counts up so the wave moves up the index
        float timeCounter = Time.time * _horizontalMovement;

        // Move points in the defined axis, according to the curves
        for (int i = 0; i < _pointsArray.Length; i++) {
            int currentIndex = i % _pointsArray.Length;
            float combinedCurveValue = 0;

            //Combine all curves
            for (int j = 1; j < _sliderAmmount + 1; j++) {  // Evaluate the curve based on the current index and frequency
                float curveEvaluation = _curveArray[j].Evaluate((currentIndex / (float)_pointsArray.Length) * _frequency + timeCounter * _speedArray[j]);
                combinedCurveValue += curveEvaluation * _amplitudeArray[j] * _totalAmplitude;
            }

            //Move in defined direction
            _movement = _movement.normalized;
            Vector3 right = _pointsArray[currentIndex].transform.right * combinedCurveValue * _movement.x;
            Vector3 up = _pointsArray[currentIndex].transform.up * combinedCurveValue * _movement.y;
            Vector3 forward = _pointsArray[currentIndex].transform.forward * combinedCurveValue * _movement.z;
            Vector3 movement = up + right + forward;

            //Apply movement
            if (currentIndex == 0 || currentIndex == _pointsArray.Length - 1) { // don't move start and end point
                movement = Vector3.zero;
            } else {
                _pointsArray[currentIndex].transform.position = _startPointLocation[currentIndex] + movement;
            }
        }
    }

}
