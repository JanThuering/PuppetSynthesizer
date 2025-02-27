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
    private PuppetScaleControlPoints puppetScaleControlPoints;
    public float ScaleMultiplier;

    //Nullpunkt und Abstand von der Linie
    private Transform lineStart;
    private float distanceZeroToLine;

    [Header("ROTATION VALUES")]

    [SerializeField] public float MovementMultiplier = 3;
    [SerializeField] private Vector3 armRotationMultiplier = new Vector3(0, 20, 160);
    [SerializeField] private Vector3 legRotationMultiplier = new Vector3(80, 10, 80);
    [SerializeField] private Vector3 torsoMultiplier = new Vector3(-10, 50, -10);
    private float lerpT = 5f;

    [Header("ROTATEABLE OBJECTS")]  //animates/rotates the limbs - fill in the inspector with the control points and effectors
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
        InitializeStartTransformForRotation();
        puppetScaleControlPoints = GetComponent<PuppetScaleControlPoints>();
        ScaleMultiplier = puppetScaleControlPoints.ScaleMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        MultiplierToAmplitude();
        RotateLimbs();
    }
    private void MultiplierToAmplitude()
    {
        MovementMultiplier = Mathf.Lerp(3, 20, Mathf.InverseLerp(0, 15, createLineScript.MaxAmplitudeClamper));
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
        Vector3 adjustedrotationMultiplier = rotationMultiplier * MovementMultiplier;

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
