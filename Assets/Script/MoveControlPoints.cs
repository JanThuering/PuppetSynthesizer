using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class MoveControlPoints : MonoBehaviour
{
    [Header("External References")]
    [HideInInspector]public int closestPointIndex;
    private CreateLine createLineScript;

    [Header("Movement")]
    [SerializeField] private float moveTowardsSpeed = 1;
    [Range(-2f, 2f)]
    [SerializeField] private float xPosition;
    private Vector3 startPosition;

    // Delay variables
    [SerializeField] private int delay = 10;
    private Vector3 delayPos;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Wave");
        createLineScript = CreateLine.Instance;
        startPosition = gameObject.transform.position;
        xPosition = startPosition.x;
    }
    // Update is called once per frame
    void Update()
    {
        MoveControlPoint();
    }

    private void MoveControlPoint()
    {
        FindClosestPoint();
        MoveHorizontally();
        CopyLineMovement();
    }

    private void FindClosestPoint()
    {
        Vector3 posControlPoint = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
        int closestIndex = 0;
        int closestIndexDelay;
        float closestDistance = float.PositiveInfinity;

        //find the closest point
        for (int i = 0; i < createLineScript.pointsArray.Length; i++)
        {
            Vector3 posWavePoint = new Vector3(createLineScript.pointsArray[i].transform.position.x, 0, createLineScript.pointsArray[i].transform.position.z);
            Vector3 horizontalDistance = posControlPoint - posWavePoint;
            if (closestDistance > horizontalDistance.magnitude)
            {
                closestDistance = horizontalDistance.magnitude;
                closestIndex = i;
                closestIndexDelay = i + delay;
            }
        }

        //save the index of the closest point
        closestPointIndex = closestIndex;
    }


    private void CopyLineMovement()
    {
        //go trough all control points  
        //copy the position of the closest HORIZONTAL point to the control point
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, CreateLine.Instance.pointsArray[closestPointIndex].transform.position, moveTowardsSpeed * Time.deltaTime);
    }

    private void MoveHorizontally()
    {
        Vector3 position = new Vector3(xPosition, CreateLine.Instance.pointsArray[closestPointIndex].transform.position.y, CreateLine.Instance.pointsArray[closestPointIndex].transform.position.z);
        Vector3 closestPoint = CreateLine.Instance.pointsArray[closestPointIndex].transform.position;
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
