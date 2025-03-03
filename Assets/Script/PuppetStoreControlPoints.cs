using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetStoreControlPoints : MonoBehaviour
{
    [SerializeField] public GameObject[] ControlPoints;
    public bool IsMovingToMiddle = false;
    private bool changeOnce = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBool();
    }

    private void UpdateBool()
    {
        if (IsMovingToMiddle && !changeOnce)
        {
            changeOnce = true;
            for (int i = 0; i < ControlPoints.Length; i++)
            {
                ControlPoints[i].GetComponent<MoveControlPoints>().IsMovingToMiddle = IsMovingToMiddle;
            }
        }
        else if (!IsMovingToMiddle && changeOnce)
        {
            changeOnce = false;
            for (int i = 0; i < ControlPoints.Length; i++)
            {
                ControlPoints[i].GetComponent<MoveControlPoints>().IsMovingToMiddle = IsMovingToMiddle;
            }
        }
    }
}
