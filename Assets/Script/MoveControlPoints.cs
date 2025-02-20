using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControlPoints : MonoBehaviour
{
    [Header ("External References")]
    public int closestPointIndex;
    private CreateLine createLineScript;

    [Header ("Movement")]
    [SerializeField] private float moveTowardsSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        createLineScript = CreateLine.Instance;
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
        

            Vector3 posControlPoint = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            int closestIndex = 0;
            float closestDistance = float.PositiveInfinity;
            //find the closest point
            
            for(int j = 0; j < createLineScript.pointsArray.Length; j++){
                Vector3 posPoint = new Vector3(createLineScript.pointsArray[j].transform.position.x, 0, createLineScript.pointsArray[j].transform.position.z);
                Vector3 horizontalDistance = posControlPoint - posPoint;
                if(closestDistance > horizontalDistance.magnitude){
                    closestDistance = horizontalDistance.magnitude;
                    closestIndex = j;
                }
            }
            //save the index of the closest point
            closestPointIndex = closestIndex;
        
    }

     private void CopyLineMovement(){
        //go trough all control points  
        //copy the position of the closest HORIZONTAL point to the control point
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position ,createLineScript.pointsArray[closestPointIndex].transform.position, moveTowardsSpeed * Time.deltaTime);   
    }



}
