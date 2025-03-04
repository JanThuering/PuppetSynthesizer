using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetStoreControlPoints : MonoBehaviour
{
    [SerializeField] public GameObject[] ControlPoints;
    private int biggestIndex = 0;
    private int smallestIndex = 300;

    // Start is called before the first frame update
    void Start()
    {
        FindHighestAndLowest();
        ApplyingHightLow();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FindHighestAndLowest()
    {
        for (int i = 0; i < ControlPoints.Length; i++)
        {
            int baseIndex = ControlPoints[i].GetComponent<MoveControlPoints>().BaseIndex;

            if (baseIndex > biggestIndex)
            {
                biggestIndex = baseIndex;
            }
            if (baseIndex < smallestIndex)
            {
               smallestIndex = baseIndex;
            }
        }
    }

    private void ApplyingHightLow()
    {
        for (int i = 0; i < ControlPoints.Length; i++)
        {
            ControlPoints[i].GetComponent<MoveControlPoints>().MostRightPositionOfControlPoint = biggestIndex;
            ControlPoints[i].GetComponent<MoveControlPoints>().MostLeftPositionOfControlPoint = smallestIndex;
        }
    }
}
