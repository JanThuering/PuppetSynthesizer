using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetScaleControlPoints : MonoBehaviour
{
    //References
    private CreateLine createLineScript;
    private GlobalControl globalControl;
    private PuppetStoreControlPoints puppetStoreControlPointsScript;

    //Nullpunkt der Linie
    private Transform lineStart;
    private float lerpT = 15f;

    [Header("SCALEABLE CONTROLPOINTS")]
    //enable and disable scaling
    [SerializeField] private bool affectScale;
    private bool scaleSetBack;

    //enable and disable visibility
    [SerializeField] private bool controlPointsInvisible;
    private bool invisibilitySet;

    //scale logic
    private float currentTotalWaveAmp;
    float normalDistToLine;
    private float maxDistanceZeroToLine = 0;
    [SerializeField] public float minScale = 0.05f;
    [SerializeField] public float maxScale = 0.2f;

    private GameObject[] controlPoints;
    private Vector3[] controlPointsStartScale;
    private Vector3[] controlPointsStartPos;

    // Start is called before the first frame update
    void Start()
    {
        createLineScript = CreateLine.Instance;
        globalControl = GlobalControl.Instance;
        lineStart = createLineScript.LineStart.transform;
        puppetStoreControlPointsScript = GetComponent<PuppetStoreControlPoints>();
        controlPoints = puppetStoreControlPointsScript.ControlPoints;
        InitializeStartScale();
    }

    // Update is called once per frame
    void Update()
    {
        //MultiplierToAmplitude();
        if (affectScale) Scale(controlPoints, controlPointsStartScale);
        if (affectScale == false && scaleSetBack == false) ScaleBack(controlPoints, controlPointsStartScale);
        Visibility();
    }


    private void Visibility()
    {
        bool shouldDisable = controlPointsInvisible && !invisibilitySet;
        bool shouldEnable = !controlPointsInvisible && invisibilitySet;

        // enable and disable Meshrenderer
        if (shouldDisable || shouldEnable)
        {
            invisibilitySet = controlPointsInvisible;

            for (int i = 0; i < controlPoints.Length; i++)
            {
                MeshRenderer renderer = controlPoints[i].GetComponent<MeshRenderer>();
                renderer.enabled = controlPointsInvisible ? false : true;
            }
        }
    }

    private void Scale(GameObject[] scaleableObj, Vector3[] startScale)
    {
        //mapping the current wave amplitude to the maximum amplitude (15)
        float normalWaveAmp = Mathf.InverseLerp(0, 15, globalControl.AmplitudeA + globalControl.AmplitudeB + globalControl.AmplitudeC);
        //determening what the average scale of the control points is with the current wave amplitude
        float defaultScaleAtAmp = Mathf.Lerp(minScale, maxScale, normalWaveAmp);
        //making it a vector3
        Vector3 defaultScaleVector = Vector3.one * defaultScaleAtAmp;

        //if the wave amplitude changed enter this code
        if (currentTotalWaveAmp != normalWaveAmp)
        {
            //reset max distance
            maxDistanceZeroToLine = 0;

            for (int i = 0; i < scaleableObj.Length; i++)
            {
                // y distance from wave to the control point
                float distanceZeroToLine = Mathf.Abs(lineStart.position.y - scaleableObj[i].transform.position.y);

                // Track the maximum distance
                if (distanceZeroToLine > maxDistanceZeroToLine)
                {
                    maxDistanceZeroToLine = distanceZeroToLine;
                }
            }
        }

        for (int i = 0; i < scaleableObj.Length; i++)
        {
            // y distance from wave to the control point
            float distanceZeroToLine = Mathf.Abs(lineStart.position.y - scaleableObj[i].transform.position.y);
            
            //mapping the distance from wave to control point from 0 to 1 (1 being the max distance)
            normalDistToLine = Mathf.InverseLerp(0, maxDistanceZeroToLine, distanceZeroToLine);

            //making the control points scale up and down with their default scale as maximum
            Vector3 targetScale = Vector3.Lerp(defaultScaleVector / 3, defaultScaleVector, normalDistToLine);

            //lerp between scales 
            Vector3 lerpedScale = Vector3.Lerp(scaleableObj[i].transform.localScale, targetScale, Time.deltaTime * lerpT);
            scaleableObj[i].transform.localScale = lerpedScale;
        }

        //saving the total wave amplitude
        currentTotalWaveAmp = normalWaveAmp;
    }

    private void ScaleBack(GameObject[] scaleableObj, Vector3[] startScale)
    {
        for (int i = 0; i < scaleableObj.Length; i++)
        {
            scaleableObj[i].transform.localScale = startScale[i];
        }
    }

    private void InitializeStartScale()
    {
        controlPointsStartScale = new Vector3[controlPoints.Length];
        controlPointsStartPos = new Vector3[controlPoints.Length];
        GetStartScale(controlPoints, controlPointsStartScale, controlPointsStartPos);
    }

    private void GetStartScale(GameObject[] scaledObjScale, Vector3[] scales, Vector3[] positions)
    {
        for (int i = 0; i < scaledObjScale.Length; i++)
        {
            scales[i] = scaledObjScale[i].transform.localScale;
            positions[i] = scaledObjScale[i].transform.position;
        }
    }
}
