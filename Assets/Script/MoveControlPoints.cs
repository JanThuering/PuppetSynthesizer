using System;
using UnityEngine;

public class MoveControlPoints : MonoBehaviour
{
    private CreateLine createLineScript;
    private GameObject[] linePointsArray;
    private int closestPointIndex;
    private int baseIndex;


    [Header("Movement")]
    [SerializeField] private float moveTowardsSpeed = 1;
    [SerializeField] private int xPosition;
    private int currentPos;
    private int middlePosition;
    [HideInInspector] public bool IsMovingToMiddle = false; //Is set in PuppetStoreControlPoints

    [Header("Delay Variables")]
    // [SerializeField] private GameObject delayObj; //for visualizing the delay
    private int animationDelayPointOnWave = 50;
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
        FindClosestPoint();
        baseIndex = closestPointIndex;
        xPosition = 0;

        //Calculate the middle position of the wave
        middlePosition = (linePointsArray.Length - 1) / 2;

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
        MoveToTheMiddle();
        MoveHorizontally();
        CopyLineMovement();
    }

    //mapping the wave frequency. used for delay (higher frequency = shorter delay)
    private void FrequencyMap()
    {
        mapFreq = Mathf.Lerp(1, 5, Mathf.InverseLerp(1, 10, createLineScript.globalFrequency));
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
    private void MoveToTheMiddle()
    {
        int newPos = 0;

        if (IsMovingToMiddle)
        {
            //depending on the current position on the wave the new position is calculated
            if (baseIndex < middlePosition) newPos = baseIndex + ((baseIndex - middlePosition) * -1);
            else if (baseIndex == middlePosition) newPos = middlePosition;

            if (currentPos != middlePosition)
            {
                if (newPos < 0) newPos = 0;
                else if (newPos > linePointsArray.Length - 1) newPos = linePointsArray.Length - 1;

                // //step by step move to the new position
                // if (newPos < closestPointIndex) closestPointIndex--;
                // else if (newPos > closestPointIndex) closestPointIndex++;

                // apply the new position to the control point
                closestPointIndex = newPos;
            }
        }
        else if (IsMovingToMiddle == false)
        {
            closestPointIndex = baseIndex;
        }

        currentPos = closestPointIndex;
    }

    private void MoveHorizontally()
    {
        //find the max and min position of the wave and apply it to the xPosition
        int maxPos = linePointsArray.Length - 1 - baseIndex;
        int minPos = baseIndex * -1;
        if (xPosition > maxPos) xPosition = maxPos;
        else if (xPosition < minPos) xPosition = minPos;
        int newPos;

        //depending on the parameter xPosition the closest point on the wave is calculated
        newPos = baseIndex + xPosition;

        if (currentPos != xPosition)
        {
            if (newPos < 0) newPos = 0;
            else if (newPos > linePointsArray.Length - 1) newPos = linePointsArray.Length - 1;

            // apply the new position to the control point
            closestPointIndex = newPos;
        }

        //store the current position
        currentPos = xPosition;
    }

    private void CopyLineMovement()
    {
        //copy the position of the closest HORIZONTAL point to the control point
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, linePointsArray[closestPointIndex].transform.position, moveTowardsSpeed * Time.deltaTime);
        DelayPosition = Vector3.MoveTowards(gameObject.transform.position, linePointsArray[closestPointDelayIndex].transform.position, moveTowardsSpeed * Time.deltaTime);
        // if (delayObj != null) delayObj.transform.position = DelayPosition; //for visualizing the delay
    }
}
