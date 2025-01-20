using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class CreateLine : MonoBehaviour
{
    [Header ("Animation Curve")]
    [SerializeField] private AnimationCurve _curveA;
    [SerializeField] private AnimationCurve _curveB;
    [SerializeField] private AnimationCurve _curveC;


    [Header ("External References")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private GameObject _lineStart;
    [SerializeField] private GameObject _lineEnd;

    [Header ("Points")]
    [SerializeField] private int _pointsCount = 2;
    public GameObject[] _pointsArray;
    private Vector3[] _startPointLocation;

    [Header ("Movement")]
    [SerializeField] private bool _sineWave = false;
    [SerializeField] private bool _squareWave = false;
    [SerializeField] private Vector3 _movement;
    [SerializeField] private float _amplitude = 1;
    [SerializeField] private float _frequency = 1;
    private float _timeCounter = 0;


    // Start is called before the first frame update
    void Start()
    {
        CreatePoints();

    }

    // Update is called once per frame
    void Update()
    {
        FillLineRenderer();
        //MovePoints();
        MovePointsCurve();

    }



    private void CreatePoints(){
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
                _startPointLocation[i] = _pointsArray[i].transform.position;
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

    public void MovePoints(){   //OLD with sinusoidal movement
        _timeCounter += Time.deltaTime;

        //move points
        if(_sineWave){
            for(int i = 0; i < _pointsArray.Length; i++){
                //Point 4: 4/4 * 2π = 2π  -> (360°)
                float phase = (float)i / (_pointsArray.Length - 1) * 2f * Mathf.PI; // 0 to 2π
                Vector3 wave = _movement * Mathf.Sin((_timeCounter * _frequency) + phase) * _amplitude;
                _pointsArray[i].transform.position = _startPointLocation[i] + wave;
            }
        }
        else if(_squareWave){
            for(int i = 0; i < _pointsArray.Length; i++){
                float phase = (float)i / (_pointsArray.Length - 1) * 2f * Mathf.PI;
                // Square wave using Sign of Sine
                Vector3 wave = _movement * Mathf.Sign(Mathf.Sin((_timeCounter * _frequency) + phase)) * _amplitude;
                _pointsArray[i].transform.position = _startPointLocation[i] + wave;
            }
        }
    }

    private void MovePointsCurve(){
        /*TODO
        - structure change of curve more easily
        - move the section of the curves indexes
        - make curves editable during runtime by player


        */


        //move first 1/3 of the points in the y axis, acording to the curve  A
        for(int i = 0; i < _pointsArray.Length/3; i++){ 
            _pointsArray[i].transform.position =  new Vector3(_pointsArray[i].transform.position.x, _startPointLocation[i].y +_curveA.Evaluate((float)i/10), _pointsArray[i].transform.position.z);
        }

        //move second 1/3 of the points in the y axis, acording to the curve  B
        for(int i = _pointsArray.Length/3; i < _pointsArray.Length/3*2; i++){ 
            _pointsArray[i].transform.position =  new Vector3(_pointsArray[i].transform.position.x, _startPointLocation[i].y +_curveB.Evaluate((float)i/10), _pointsArray[i].transform.position.z);
        }

        //move third 1/3 of the points in the y axis, acording to the curve  C
        for(int i = _pointsArray.Length/3*2; i < _pointsArray.Length; i++){ 
            _pointsArray[i].transform.position =  new Vector3(_pointsArray[i].transform.position.x, _startPointLocation[i].y +_curveC.Evaluate((float)i/10), _pointsArray[i].transform.position.z);
        }

    }


}
