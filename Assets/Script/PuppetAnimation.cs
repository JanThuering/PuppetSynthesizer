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
    [SerializeField] private float delay = 0.2f;
    //[Range(-90f, 90f)] // Slider 
    [SerializeField] private Vector3 armRotationMultiplier = new Vector3(80, 50, 80);
    [SerializeField] private Vector3 legRotationMultiplier = new Vector3(80, 50, 80);
    [SerializeField] private Vector3 torsoMultiplier = new Vector3(20, 100, 20);
    [SerializeField] private bool affectScale = false;
    private Vector3 multiplier;
    private Tween normalizerTween;
    private float normalizer = 1;
    private bool isTweening = false;

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

    [Header("SCALE OBJECTS")]

    //ALL OBJECTS WHICH WILL BE SCALED
    [SerializeField] private GameObject[] bones;

    // Start is called before the first frame update
    void Start()
    {
        createLineScript = CreateLine.Instance;
        lineStart = createLineScript.LineStart.transform;
        
        InitializeStartTransforms();

        normalizerTween = DOTween.To(() => normalizer, x => normalizer = x, -1, 2)
               .SetEase(Ease.InOutSine)
               .SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        RotateLimbs();
    }

    void LateUpdate()
    {
        if (affectScale) Scale(bones, armLControlPoint);
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

        // torso auf beide seiten der x achse drehen lassen 
        // TODO ohne normalizer (bewegt sich von links nach rechts mit dem value und nicht natürlich)
        //if (controlPoint == torsoControlPoint) multiplier = torsoMultiplier * normalizer;

        for (int i = 0; i < effectors.Length; i++)
        {
            // y distanz von der welle zum controlpoint
            if (i == 0) distanceZeroToLine = lineStart.position.y - controlPoint.position.y;
            else distanceZeroToLine = lineStart.position.y - controlPoint.GetComponent<MoveControlPoints>().DelayPosition.y;

            // bewegungsrichtung von links und rechts gedreht
            // if (startPos[i].x < 0) multiplier = new Vector3(rotationMultiplier.x, rotationMultiplier.y * -1, rotationMultiplier.z * -1);
            // else if (effectors[i].transform.position.x > 0) multiplier = rotationMultiplier;

            multiplier = rotationMultiplier;

            //targetRotation 
            Vector3 targetRotation = (multiplier * distanceZeroToLine) + startRot[i];
            effectors[i].transform.localEulerAngles = targetRotation;
        }
    }

    //!Is forced in lateupdate after animation rigging has applied constraints!
    private void Scale(GameObject[] bones, Transform controlPoint)
    {
        for (int i = 0; i < bones.Length; i++)
        {
            // y distanz von der welle zum controlpoint
            distanceZeroToLine = Math.Abs(lineStart.position.y - controlPoint.position.y);
            // mapped
            distanceZeroToLine = Mathf.Lerp(0.5f, 1, Mathf.InverseLerp(0, 0.2f, distanceZeroToLine));

            Vector3 targetScale = Vector3.one * distanceZeroToLine;
            bones[i].transform.localScale = targetScale;
        }
    }

    //TODO initialize rotation und position in einer funktion mergen
    //Start position der controlpoints

    private void InitializeStartTransforms()
    {
        armLStartPositions = new Vector3[armLEffectors.Length];
        armLStartRotation = new Vector3[armLEffectors.Length];
        GetStartTransforms(armLEffectors, armLStartPositions, armLStartRotation);

        armRStartPositions = new Vector3[armREffectors.Length];
        armRStartRotation = new Vector3[armREffectors.Length];
        GetStartTransforms(armREffectors, armRStartPositions, armRStartRotation);

        torsoStartPositions = new Vector3[torsoEffectors.Length];
        torsoStartRotation = new Vector3[torsoEffectors.Length];
        GetStartTransforms(torsoEffectors, torsoStartPositions, torsoStartRotation);

        legLStartPositions = new Vector3[legLEffectors.Length];
        legLStartRotation = new Vector3[legLEffectors.Length];
        GetStartTransforms(legLEffectors, legLStartPositions, legLStartRotation);

        legRStartPositions = new Vector3[legREffectors.Length];
        legRStartRotation = new Vector3[legREffectors.Length];
        GetStartTransforms(legREffectors, legRStartPositions, legRStartRotation);
    }

    private void GetStartTransforms(GameObject[] bodypart, Vector3[] positions, Vector3[] rotations)
    {
        for (int i = 0; i < bodypart.Length; i++)
        {
            positions[i] = bodypart[i].transform.position;
            rotations[i] = bodypart[i].transform.localEulerAngles;
        }
    }
}
