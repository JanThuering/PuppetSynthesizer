using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
  [Header("External References")]
    private CinemachineFreeLook cameraFL;
    private GlobalControl globalControlScript;
    [Range(0, 2)]
    [SerializeField] private int currentCamera = 0;

    [SerializeField] private float lerpTime = 0.5f;

    [Range(-90, 90)]
    [SerializeField]float [] xValues = {0.0f, 0.0f, 0.0f};
    [Range(0, 1)]
    [SerializeField] float [] yValues = {0.0f, 0.0f, 0.0f};
    [SerializeField] float [] fovValues = {10.0f, 30.0f, 20.0f};

    // Start is called before the first frame update
    void Start()
    {
      FillVariables();
    }

    // Update is called once per frame
    void Update()
    {
      currentCamera = globalControlScript.CurrentCamera;
      ChooseCameraPos();
    }

    private void FillVariables()
    {
      cameraFL = gameObject.GetComponent<CinemachineFreeLook>();
      globalControlScript = GlobalControl.Instance;
    }
    private void ChooseCameraPos()
    {
      float currentXValue = 0.0f;
      float currentYValue = 0.0f;
      float currentFOV = 0.0f;

      switch (currentCamera){
        case 0: currentXValue = xValues[0]; currentYValue = yValues[0]; currentFOV = fovValues[0]; break;
        case 1: currentXValue = xValues[1]; currentYValue = yValues[1]; currentFOV = fovValues[1]; break;
        case 2: currentXValue = xValues[2]; currentYValue = yValues[2]; currentFOV = fovValues[2]; break;

      }

      cameraFL.m_XAxis.Value = Mathf.Lerp(cameraFL.m_XAxis.Value, currentXValue, lerpTime);
      cameraFL.m_YAxis.Value = Mathf.Lerp(cameraFL.m_YAxis.Value, currentYValue, lerpTime);
      cameraFL.m_Lens.FieldOfView = Mathf.Lerp(cameraFL.m_Lens.FieldOfView, currentFOV, lerpTime);
    }

}
