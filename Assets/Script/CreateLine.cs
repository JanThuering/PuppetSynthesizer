using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CreateLine : MonoBehaviour
{
    [Header ("Line Renderer")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private GameObject _lineStart;
    [SerializeField] private GameObject _lineEnd;

    [Header ("Points")]
    [SerializeField] private int _pointsCount = 2;
    [SerializeField] private GameObject[] _pointsArray;
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
        MovePoints();

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
            if(i == _lineRenderer.positionCount-1){
            }
        }
    }

    public void MovePoints(){
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
}
