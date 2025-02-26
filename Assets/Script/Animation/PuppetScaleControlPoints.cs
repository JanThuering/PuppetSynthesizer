using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetScaleControlPoints : MonoBehaviour
{
    //References
    private CreateLine createLineScript;

    //Nullpunkt und Abstand von der Linie
    private Transform lineStart;
    private float distanceZeroToLine;

     private float lerpT = 15f;

    [Header("SCALEABLE CONTROLPOINTS")]
    [SerializeField] private bool affectScale = false;
    [SerializeField] public float ScaleMultiplier = 2;
    [SerializeField] private GameObject[] scaledObjects;
    private Vector3[] scaledObjStartScale;
    private Vector3[] scaledObjStartPos;

    // Start is called before the first frame update
    void Start()
    {
        createLineScript = CreateLine.Instance;
        lineStart = createLineScript.LineStart.transform;
        InitializeStartScale();
    }

    // Update is called once per frame
    void Update()
    {
        if (affectScale) Scale(scaledObjects, scaledObjStartScale);
        if (affectScale!) ScaleBack(scaledObjects, scaledObjStartScale);
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
            //lerp between scales 
            //if (i == 0) print("current scale " + scaleableObj[i].transform.localScale + "start scale " + startScale[i]);
            Vector3 lerpedScale = Vector3.Lerp(scaleableObj[i].transform.localScale, startScale[i], Time.deltaTime * lerpT);
            scaleableObj[i].transform.localScale = lerpedScale;
        }
    }

    private void InitializeStartScale()
    {
        scaledObjStartScale = new Vector3[scaledObjects.Length];
        scaledObjStartPos = new Vector3[scaledObjects.Length];
        GetStartScale(scaledObjects, scaledObjStartScale, scaledObjStartPos);
    }

    private void GetStartScale(GameObject[] scaledObjScale, Vector3[] scales, Vector3[] positions)
    {
        for (int i = 0; i < scaledObjScale.Length; i++)
        {
            scales[i] = scaledObjScale[i].transform.localScale;
            positions[i] = scaledObjScale[i].transform.position;
            // print(scaledObjScale[i].transform.position);
        }
    }
}
