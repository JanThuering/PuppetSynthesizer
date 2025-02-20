using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePuppet : MonoBehaviour
{
    [Header ("External References")]
    [SerializeField] private GameObject[] _controlPoints; //fill in the inspector with the target points
    public int [] _closestPointIndex; //fill in the inspector with the closest point to the control point
    [SerializeField] public CreateLine _csCreateLine; //fill in the inspector with the line renderer

    [Header ("Movement")]
    [SerializeField] private float _moveTowardsSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        _closestPointIndex = new int[_controlPoints.Length];
    }

    // Update is called once per frame
    void Update()
    {
        MoveControlPoint();
        
    }


    private void MoveControlPoint(){
        FindClosestPoint();
        CopyLineMovement();
    }


   

    private void FindClosestPoint(){
        
        for (int i = 0; i < _controlPoints.Length; i++){
            Vector3 posControlPoint = new Vector3(_controlPoints[i].transform.position.x, 0, _controlPoints[i].transform.position.z);
            int closestIndex = 0;
            float closestDistance = float.PositiveInfinity;
            //find the closest point
            for(int j = 0; j < _csCreateLine._pointsArray.Length; j++){
                Vector3 posPoint = new Vector3(_csCreateLine._pointsArray[j].transform.position.x, 0, _csCreateLine._pointsArray[j].transform.position.z);
                Vector3 horizontalDistance = posControlPoint - posPoint;
                if(closestDistance > horizontalDistance.magnitude){
                    closestDistance = horizontalDistance.magnitude;
                    closestIndex = j;
                }
            }
            //save the index of the closest point
            _closestPointIndex[i] = closestIndex;
        }
    }

     private void CopyLineMovement(){
        //go trough all control points
        for (int i = 0; i < _controlPoints.Length; i++){    
            //copy the position of the closest HORIZONTAL point to the control point
            _controlPoints[i].transform.position = Vector3.MoveTowards(_controlPoints[i].transform.position ,_csCreateLine._pointsArray[_closestPointIndex[i]].transform.position, _moveTowardsSpeed * Time.deltaTime);   
        }
    }


}
