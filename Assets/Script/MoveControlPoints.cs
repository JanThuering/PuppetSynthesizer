using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class MoveControlPoints : MonoBehaviour
{
    private CreateLine createLineScript;
    private GameObject[] linePointsArray;
    private int closestPointIndex;
    [SerializeField] private GameObject delayObj;

    [Header("Movement")]
    [SerializeField] private float moveTowardsSpeed = 1;
    [Range(-2f, 2f)]
    [SerializeField] private float xPosition;
    private Vector3 startPosition;

    // DELAY VARIABLES
    [SerializeField] private int animationDelayPointOnWave = 50;
    private int initialAnimationDelay;  // Store initial delay value
    [HideInInspector] public Vector3 DelayPosition;
    private int closestPointDelayIndex;
    private float mapFreq;
    private int delay;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Wave");
        createLineScript = CreateLine.Instance;
        linePointsArray = createLineScript.pointsArray;
        startPosition = gameObject.transform.position;
        xPosition = startPosition.x;

        // Store the initial delay to ensure it always remains 50 when frequency is 1
        initialAnimationDelay = animationDelayPointOnWave;
    }
    // Update is called once per frame
    void Update()
    {
        MoveControlPoint();
    }

    private void MoveControlPoint()
    {
        FrequencyMap();
        FindClosestPoint();
        MoveHorizontally();
        CopyLineMovement();
    }

    //mapping the wave frequency. used for delay (higher frequency = shorter delay)
    private void FrequencyMap()
    {
        mapFreq = Mathf.Lerp(1, 5, Mathf.InverseLerp(1, 10, createLineScript.frequency));
        delay = Mathf.CeilToInt(initialAnimationDelay / mapFreq);
    }

    private void FindClosestPoint()
    {
        Vector3 posControlPoint = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
        int closestIndex = 0;
        int closestIndexDelay = 0;
        float closestDistance = float.PositiveInfinity;

        //find the closest point
        for (int i = 0; i < linePointsArray.Length; i++)
        {
            Vector3 posWavePoint = new Vector3(linePointsArray[i].transform.position.x, 0, linePointsArray[i].transform.position.z);
            Vector3 horizontalDistance = posControlPoint - posWavePoint;
            if (closestDistance > horizontalDistance.magnitude)
            {
                closestDistance = horizontalDistance.magnitude;
                closestIndex = i;
                closestIndexDelay = i + delay;
            }

            //check if delay is out of wave
            if (closestIndexDelay > linePointsArray.Length) closestIndexDelay = linePointsArray.Length - 1;
            else if (closestIndexDelay < 0) closestIndexDelay = 0;

            //save the index of the closest point
            closestPointIndex = closestIndex;
            closestPointDelayIndex = closestIndexDelay;
        }

    }
    private void CopyLineMovement()
    {
        //go trough all control points  
        //copy the position of the closest HORIZONTAL point to the control point
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, linePointsArray[closestPointIndex].transform.position, moveTowardsSpeed * Time.deltaTime);

        DelayPosition = Vector3.MoveTowards(gameObject.transform.position, linePointsArray[closestPointDelayIndex].transform.position, moveTowardsSpeed * Time.deltaTime);
        if (delayObj != null) delayObj.transform.position = DelayPosition;
    }

    private void MoveHorizontally()
    {
        Vector3 position = new Vector3(xPosition, linePointsArray[closestPointIndex].transform.position.y, linePointsArray[closestPointIndex].transform.position.z);
        Vector3 closestPoint = linePointsArray[closestPointIndex].transform.position;
        Vector3 lineStart = createLineScript.LineStart.transform.position;
        Vector3 lineEnd = createLineScript.LineEnd.transform.position;

        if (gameObject.transform.position.x > lineStart.x && gameObject.transform.position.x < lineEnd.x + 0.5f)
        {
            gameObject.transform.position = position;
        }
        if (gameObject.transform.position.x < lineStart.x)
        {
            Debug.Log("Out of bounds");
            gameObject.transform.position = new Vector3(lineStart.x, closestPoint.y, closestPoint.z);
            // stop the player from decreasing the x position
            if (xPosition < lineStart.x)
            {
                xPosition = lineStart.x;
            }
        }
        if (gameObject.transform.position.x > lineEnd.x)
        {
            Debug.Log("Out of bounds");
            gameObject.transform.position = new Vector3(lineEnd.x, closestPoint.y, closestPoint.z);
            //stop the player from increasing the x position
            if (xPosition > lineEnd.x)
            {
                xPosition = lineEnd.x;
            }
        }
    }
}
