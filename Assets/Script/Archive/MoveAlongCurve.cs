using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MoveAlongCurve : MonoBehaviour
{
    [Header ("External References")]
    [SerializeField] private GameObject[] _controlPoints; //fill in the inspector with the target points
    [SerializeField] private CreateLine _csCreateLine; //fill in the inspector with the line renderer


    [Header ("Movement")]
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _moveTreshhold = 1.0f;
    private int [] _currentPointIndex; //fill in the inspector with the closest point to the control point





    // Start is called before the first frame update
    void Start()
    {
        MovePointToStartPos();
    }


    // Update is called once per frame
    void Update()
    {
        MovePointAlongCurve();
    }

    private void MovePointToStartPos(){

        _currentPointIndex = new int[_controlPoints.Length];

        //find closest point to the control point
        for(int i = 0; i < _controlPoints.Length; i++){
            Vector3 posControlPoint = new Vector3(_controlPoints[i].transform.position.x, 0, _controlPoints[i].transform.position.z);
            int closestIndex = 0;
            float closestDistance = float.PositiveInfinity;
            //find the closest point
            for(int j = 0; j < _csCreateLine.pointsArray.Length; j++){
                Vector3 posPoint = new Vector3(_csCreateLine.pointsArray[j].transform.position.x, 0, _csCreateLine.pointsArray[j].transform.position.z);
                Vector3 horizontalDistance = posControlPoint - posPoint;
                if(closestDistance > horizontalDistance.magnitude){
                    closestDistance = horizontalDistance.magnitude;
                    closestIndex = j;
                }
            }
            //save the index of the closest point
            _controlPoints[i].transform.position = _csCreateLine.pointsArray[closestIndex].transform.position;
            _currentPointIndex[i] = closestIndex;
        }
    }


    private void MovePointAlongCurve(){

        //2. move controlpoint to the next point in the linerenderer (by increasing index)
        //define the speed of the movement
        //3. if the controlpoint reaches the end of the linerenderer, move it back to the start


  
        for(int i = 0; i < _controlPoints.Length; i++){
            
            //if the index is bigger than the length of the array, reset the index
            if(_currentPointIndex[i] >= _csCreateLine.pointsArray.Length-1){
                _currentPointIndex[i] = 0;
                _controlPoints[i].transform.position = _csCreateLine.pointsArray[_currentPointIndex[i]].transform.position;
            }

            //calculate the distance between the controlpoint and the next point in the linerenderer
            float distance = Vector3.Distance(_controlPoints[i].transform.position, _csCreateLine.pointsArray[_currentPointIndex[i]].transform.position);
            
            //if the distance is smaller than the treshhold, increase the index
            if(distance <= _moveTreshhold/100){
                _currentPointIndex[i]++;
            }
            
            //move the controlpoint to the next point in the linerenderer
            _controlPoints[i].transform.position = Vector3.MoveTowards(_controlPoints[i].transform.position, _csCreateLine.pointsArray[_currentPointIndex[i]].transform.position, _moveSpeed*Time.deltaTime );
        


            if(i == 0){
                Debug.Log(_currentPointIndex[i]);
            }
                
            
        }
        


    }
}
