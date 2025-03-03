using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PuppetMoveControlPoints : MonoBehaviour
{
    private CreateLine createLineScript;
    private GameObject[] linePointsArray;
    private PuppetStoreControlPoints puppetStoreControlPointsScript;
    private GameObject[] controlPoints;
    private int[] baseIndex;
    private int middlePosition;
    public bool IsMovingToMiddle = false;
    private bool isMovingToMiddleApplied = false;
    // Start is called before the first frame update
    void Start()
    {
        //Variables
        createLineScript = CreateLine.Instance;
        linePointsArray = createLineScript.pointsArray;
        puppetStoreControlPointsScript = GetComponent<PuppetStoreControlPoints>();
        controlPoints = puppetStoreControlPointsScript.ControlPoints;

        //Calculate the middle position of the wave
        middlePosition = (linePointsArray.Length - 1) / 2;

        //Save all starting positions on the wave in the baseIndex Array
        baseIndex = new int[controlPoints.Length];
        for (int i = 0; i < controlPoints.Length; i++)
        {
            baseIndex[i] = controlPoints[i].GetComponent<MoveControlPoints>().BaseIndex;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsMovingToMiddle && isMovingToMiddleApplied!) MovePointsToTheMiddle();
        
    }


    private void MovePointsToTheMiddle()
    {

        for (int i = 0; i < controlPoints.Length; i++)
        {
            //depending on the current position on the wave the new position is calculated
            int newPos = middlePosition - baseIndex[i];

            controlPoints[i].GetComponent<MoveControlPoints>().ClosestPointIndex = newPos;
        }
    }
}
