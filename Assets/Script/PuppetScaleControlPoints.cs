using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetScaleControlPoints : MonoBehaviour
{
    //References
    private CreateLine createLineScript;
    private PuppetStoreControlPoints puppetStoreControlPointsScript;

    //Nullpunkt und Abstand von der Linie
    private Transform lineStart;
    private float distanceZeroToLine;

    private float lerpT = 15f;

    [Header("SCALEABLE CONTROLPOINTS")]
    [SerializeField] private bool affectScale;
    private bool scaleSetBack;
    [SerializeField] private bool controlPointsInvisible;
    private bool invisibilitySet;
    [SerializeField] public float ScaleMultiplier = 2;
    private GameObject[] controlPoints;
    private Vector3[] controlPointsStartScale;
    private Vector3[] controlPointsStartPos;

    // Start is called before the first frame update
    void Start()
    {
        createLineScript = CreateLine.Instance;
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

    private void MultiplierToAmplitude()
    {
        ScaleMultiplier = Mathf.Lerp(0.5f, 0.52f, Mathf.InverseLerp(0, 15, createLineScript.MaxAmplitudeClamper));
        print(ScaleMultiplier);
    }
    private void Visibility()
    {
        bool shouldDisable = controlPointsInvisible && !invisibilitySet;
        bool shouldEnable = !controlPointsInvisible && invisibilitySet;

        // enable and disable meshrenderer

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
        Vector3 scaleMultiplierVec = new Vector3(ScaleMultiplier, ScaleMultiplier, ScaleMultiplier);

        for (int i = 0; i < scaleableObj.Length; i++)
        {
            // y distanz von der welle zum controlpoint
            distanceZeroToLine = MathF.Abs(lineStart.position.y - scaleableObj[i].transform.position.y);

            //print(distanceZeroToLine);

            Vector3 targetScale = (scaleMultiplierVec * distanceZeroToLine) + startScale[i];
            //Vector3 mappedScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 1.5f, Mathf.InverseLerp(0, 0.2f, distanceZeroToLine));

            //lerp between scales 
            Vector3 lerpedScale = Vector3.Lerp(scaleableObj[i].transform.localScale, targetScale, Time.deltaTime * lerpT);
            scaleableObj[i].transform.localScale = lerpedScale;
        }
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
