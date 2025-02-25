using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Rendering;
using Unity.Mathematics;

public class PuppetAnimation : MonoBehaviour
{
    //References
    private CreateLine createLineScript;
    //Nullpunkt und Abstand von der Linie
    private Transform lineStart;
    private float distanceZeroToLine;

    [Header("ROTATION VALUES")]

    [SerializeField] private float movementMultiplier = 3;
    [SerializeField] private Vector3 armRotationMultiplier = new Vector3(80, 50, 80);
    [SerializeField] private Vector3 legRotationMultiplier = new Vector3(80, 50, 80);
    [SerializeField] private Vector3 torsoMultiplier = new Vector3(20, 100, 20);
    private float lerpT = 15f;

    [Header("SCALE OBJECTS")]
    [SerializeField] private bool affectScale = false;
    [SerializeField] private float scaleMultiplier = 2;
    [SerializeField] private GameObject[] scaledObjects;
    private Vector3[] scaledObjStartScale;
    private Vector3[] scaledObjStartPos;


    [Header("ROTATEABLE OBJECTS")]  //rotates the limbs, fill in the inspector with the joints
    //ARMS
    [SerializeField] private Transform armLControlPoint;
    [SerializeField] private GameObject[] armLEffectors;
    private Vector3[] armLStartRotation;
    private Vector3[] armLStartPositions;

    [SerializeField] private Transform armRControlPoint;
    [SerializeField] private GameObject[] armREffectors;
    private Vector3[] armRStartRotation;
    private Vector3[] armRStartPositions;

    //LEGS
    [SerializeField] private Transform legLControlPoint;
    [SerializeField] private GameObject[] legLEffectors;
    private Vector3[] legLStartRotation;
    private Vector3[] legLStartPositions;

    [SerializeField] private Transform legRControlPoint;
    [SerializeField] private GameObject[] legREffectors;
    private Vector3[] legRStartRotation;
    private Vector3[] legRStartPositions;

    //TORSO
    [SerializeField] private Transform torsoControlPoint;
    [SerializeField] private GameObject[] torsoEffectors;
    private Vector3[] torsoStartRotation;
    private Vector3[] torsoStartPositions;

    //BASE
    [SerializeField] private Transform baseControlPoint;
    [SerializeField] private GameObject baseBone;

    // Start is called before the first frame update
    void Start()
    {
        createLineScript = CreateLine.Instance;
        lineStart = createLineScript.LineStart.transform;
        InitializeStartScale();
        InitializeStartTransformForRotation();
    }

    // Update is called once per frame
    void Update()
    {
        RotateLimbs();
    }

    void LateUpdate()
    {
        if (affectScale) Scale(scaledObjects, scaledObjStartScale);
        if (affectScale!) ScaleBack(scaledObjects, scaledObjStartScale);
    }

    private void RotateLimbs()
    {
        Rotate(armLEffectors, armLStartRotation, armLStartPositions, armLControlPoint, armRotationMultiplier);
        Rotate(armREffectors, armRStartRotation, armRStartPositions, armRControlPoint, armRotationMultiplier);
        Rotate(legLEffectors, legLStartRotation, legLStartPositions, legLControlPoint, legRotationMultiplier);
        Rotate(legREffectors, legRStartRotation, legRStartPositions, legRControlPoint, legRotationMultiplier);
        Rotate(torsoEffectors, torsoStartRotation, torsoStartPositions, torsoControlPoint, torsoMultiplier);
    }

    private void Rotate(GameObject[] effectors, Vector3[] startRot, Vector3[] startPos, Transform controlPoint, Vector3 rotationMultiplier)
    {
        //TODO schreibe eine funktion im movepoints Script die die Startposition der Punkte zurückgibt
        Vector3 adjustedrotationMultiplier = rotationMultiplier * movementMultiplier;

        for (int i = 0; i < effectors.Length; i++)
        {
            // y distanz von der welle zum controlpoint
            if (i == 0) distanceZeroToLine = lineStart.position.y - controlPoint.position.y;
            else distanceZeroToLine = lineStart.position.y - controlPoint.GetComponent<MoveControlPoints>().DelayPosition.y;

            // bewegungsrichtung von links und rechts gedreht
            // if (startPos[i].x < 0) multiplier = new Vector3(rotationMultiplier.x, rotationMultiplier.y * -1, rotationMultiplier.z * -1);
            // else if (effectors[i].transform.position.x > 0) multiplier = rotationMultiplier;

            //targetRotation 
            Vector3 targetEulerAngles = (adjustedrotationMultiplier * distanceZeroToLine) + startRot[i];
            Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);

            //lerp between rotations 
            Quaternion quatLerpedRotation = Quaternion.Lerp(effectors[i].transform.localRotation, targetRotation, Time.deltaTime * lerpT);
            Vector3 eulerLerpedRotation = quatLerpedRotation.eulerAngles;
            effectors[i].transform.localEulerAngles = eulerLerpedRotation;
        }
    }


    //!Is forced in lateupdate after animation rigging has applied constraints!
    private void Scale(GameObject[] scaleableObj, Vector3[] startScale)
    {
        Vector3 scaleMultiplierVec = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);

        for (int i = 0; i < scaleableObj.Length; i++)
        {
            // y distanz von der welle zum controlpoint
            distanceZeroToLine = MathF.Abs(lineStart.position.y - scaleableObj[i].transform.position.y);

            print(distanceZeroToLine);

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
            if (i == 0) print("current scale " + scaleableObj[i].transform.localScale + "start scale " + startScale[i]);
            Vector3 lerpedScale = Vector3.Lerp(scaleableObj[i].transform.localScale, startScale[i], Time.deltaTime * lerpT);
            scaleableObj[i].transform.localScale = lerpedScale;
        }
    }

    //
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
            print(scaledObjScale[i].transform.position);
        }
    }

    //Start position und rotation der effectors
    private void InitializeStartTransformForRotation()
    {
        armLStartPositions = new Vector3[armLEffectors.Length];
        armLStartRotation = new Vector3[armLEffectors.Length];
        GetStartTransformsForRotation(armLEffectors, armLStartPositions, armLStartRotation);

        armRStartPositions = new Vector3[armREffectors.Length];
        armRStartRotation = new Vector3[armREffectors.Length];
        GetStartTransformsForRotation(armREffectors, armRStartPositions, armRStartRotation);

        torsoStartPositions = new Vector3[torsoEffectors.Length];
        torsoStartRotation = new Vector3[torsoEffectors.Length];
        GetStartTransformsForRotation(torsoEffectors, torsoStartPositions, torsoStartRotation);

        legLStartPositions = new Vector3[legLEffectors.Length];
        legLStartRotation = new Vector3[legLEffectors.Length];
        GetStartTransformsForRotation(legLEffectors, legLStartPositions, legLStartRotation);

        legRStartPositions = new Vector3[legREffectors.Length];
        legRStartRotation = new Vector3[legREffectors.Length];
        GetStartTransformsForRotation(legREffectors, legRStartPositions, legRStartRotation);
    }

    private void GetStartTransformsForRotation(GameObject[] bodypart, Vector3[] positions, Vector3[] rotations)
    {
        for (int i = 0; i < bodypart.Length; i++)
        {
            positions[i] = bodypart[i].transform.position;
            rotations[i] = bodypart[i].transform.localEulerAngles;
        }
    }
}
