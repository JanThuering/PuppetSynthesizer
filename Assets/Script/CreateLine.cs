using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class CreateLine : MonoBehaviour
{
    [Header ("EXTERNAL REFERENCES")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private GameObject _lineStart;
    [SerializeField] private GameObject _lineEnd;

    [Header ("ANIMATION VALUES")]
    [SerializeField] private Vector3 _movement = new Vector3(0,1,0);
    [Range(-10, 10)]
    [SerializeField] private float _amplitude = 1;
    [Range(0.1f, 1)]
    [SerializeField] private float _frequency = 1;
    [Range(0f, 500f)]
    [SerializeField] private float _horizontalMovement = 10;

    [Header ("CURVES")]
    [SerializeField] private AnimationCurve _curveA;
    [SerializeField] private AnimationCurve _curveB;
    [SerializeField] private AnimationCurve _curveC;
    /*
    [Range(0, 1)]
    [SerializeField] private float _curveAStrength = 1;
    [Range(0, 1)]
    [SerializeField] private float _curveBStrength = 1;
    [Range(0, 1)]
    [SerializeField] private float _curveCStrength = 1;
    */
    

    [Header ("INTERFACE VALUES")]
    [SerializeField] private int _sliderAmmount = 3;

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



    [Header ("Points")]
    [SerializeField] private int _pointsCount = 2;
    public GameObject[] _pointsArray;
    private Vector3[] _startPointLocation;

    /*
    [Header ("Movement")]
    [SerializeField] private bool _sineWave = false;
    [SerializeField] private bool _squareWave = false;
    [SerializeField] private Vector3 _movement;
    [SerializeField] private float _amplitude = 1;
    [SerializeField] private float _frequency = 1;
    private float _timeCounter = 0;
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

        //Move the points in the y axis, acording to the curves
        ControlCurve(_curveA, 1, _amplitude, _frequency);
        ControlCurve(_curveB, 2, _amplitude, _frequency);
        ControlCurve(_curveC, 3, _amplitude, _frequency);

        _pointsArray[0].transform.position = _lineStart.transform.position;


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

    private void ControlCurve(AnimationCurve curve, int slider, float amplitude, float frequency){
        if(slider > _sliderAmmount || slider < 1){
            Debug.LogError("Slider out of range");
            return;
        }
        float timeCounter = Time.time*_horizontalMovement;  //counts up so the wave moves up the index

        //calculate range of points
        int range = _pointsArray.Length / _sliderAmmount;
        int start = range * (slider-1);
        int end = range * slider;


        //move points in the y axis, according to the curve
        for(int i = start+(int)timeCounter; i < end+(int)timeCounter; i++){
            /*
            float curveValue1 = _curveA.Evaluate((float)i/frequency + Time.time) * amplitude * _curveAStrength;
            float curveValue2 = _curveB.Evaluate((float)i/frequency + Time.time) * amplitude * _curveBStrength;
            float curveValue3 = _curveC.Evaluate((float)i/frequency + Time.time) * amplitude * _curveCStrength;
            float curveValue = curveValue1 + curveValue2 + curveValue3;
            */
            float curveValue = curve.Evaluate((float)i/frequency + Time.time) * amplitude;
            int currentIndex = i%_pointsArray.Length;
            
            

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



}
