using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private CinemachineFreeLook cameraFL;

    // Start is called before the first frame update
    void Start()
    {
        FillVariables();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FillVariables()
    {
      cameraFL = gameObject.GetComponent<CinemachineFreeLook>();
    }
}
