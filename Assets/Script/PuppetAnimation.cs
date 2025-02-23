using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Rendering;
using Unity.Mathematics;

public class PuppetAnimation : MonoBehaviour
{
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
    [SerializeField] private Transform armLControlPointDelay;
    [SerializeField] private GameObject[] armLEffectors;
    private Vector3[] armLStartRotation;
    [SerializeField] private Transform armRControlPoint;
    [SerializeField] private Transform armRControlPointDelay;
    [SerializeField] private GameObject[] armREffectors;
    private Vector3[] armRStartRotation;

    //LEGS
    [SerializeField] private Transform legLControlPoint;
    [SerializeField] private Transform legLControlPointDelay;
    [SerializeField] private GameObject[] legLEffectors;
    private Vector3[] legLStartRotation;
    [SerializeField] private Transform legRControlPoint;
    [SerializeField] private Transform legRControlPointDelay;
    [SerializeField] private GameObject[] legREffectors;
    private Vector3[] legRStartRotation;

    //TORSO
    [SerializeField] private Transform torsoControlPoint;
    [SerializeField] private Transform torsoControlPointDelay;
    [SerializeField] private GameObject[] torsoEffectors;
    private Vector3[] torsoStartRotation;

    //BASE
    [SerializeField] private Transform baseControlPoint;
    [SerializeField] private GameObject baseBone;

    [Header("SCALE OBJECTS")]

    //ALL OBJECTS WHICH WILL BE SCALED
    [SerializeField] private GameObject[] bones;

    // Start is called before the first frame update
    void Start()
    {
        lineStart = CreateLine.Instance.LineStart.transform;

        InitializeStartRotations();

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
        if (affectScale) Scale(bones, armLControlPoint, armLControlPointDelay);
    }

    private void RotateLimbs()
    {
        Rotate(armLEffectors, armLStartRotation, armLControlPoint, armLControlPointDelay, armRotationMultiplier);
        Rotate(armREffectors, armRStartRotation, armRControlPoint, armRControlPointDelay, armRotationMultiplier);
        Rotate(legLEffectors, legLStartRotation, legLControlPoint, legLControlPointDelay, legRotationMultiplier);
        Rotate(legREffectors, legRStartRotation, legRControlPoint, legRControlPointDelay, legRotationMultiplier);
        Rotate(torsoEffectors, torsoStartRotation, torsoControlPoint, torsoControlPointDelay, torsoMultiplier);
    }

    private void Rotate(GameObject[] effectors, Vector3[] startRot, Transform controlPoint, Transform controlPointDelay, Vector3 rotationMultiplier)
    {
        //TODO schreibe eine funktion im movepoints Script die die Startposition der Punkte zurückgibt

        // torso auf beide seiten der x achse drehen lassen 
        // TODO ohne normalizer (bewegt sich von links nach rechts mit dem value und nicht natürlich)
        if (controlPoint == torsoControlPoint) multiplier = torsoMultiplier * normalizer;

        for (int i = 0; i < effectors.Length; i++)
        {
            // y distanz von der welle zum controlpoint
            if (i == 0) distanceZeroToLine = lineStart.position.y - controlPoint.position.y;
            else distanceZeroToLine = lineStart.position.y - controlPointDelay.position.y;

            //TODO sollte mit startPos gemacht werden, nicht mit bodypart pos
            // bewegungsrichtung von links und rechts gedreht
            if (effectors[i].transform.position.x < 0) multiplier = new Vector3(rotationMultiplier.x, rotationMultiplier.y * -1, rotationMultiplier.z * -1);
            else if (effectors[i].transform.position.x > 0) multiplier = rotationMultiplier;

            //targetRotation 
            Vector3 targetRotation = (multiplier * distanceZeroToLine) + startRot[i];
            effectors[i].transform.localEulerAngles = targetRotation;
        }
    }

    //!Is forced in lateupdate after animation rigging has applied constraints
    private void Scale(GameObject[] bones, Transform controlPoint, Transform controlPointDelay)
    {
        for (int i = 0; i < bones.Length; i++)
        {
            // y distanz von der welle zum controlpoint
            distanceZeroToLine = Math.Abs(lineStart.position.y - controlPoint.position.y);
            // mapped
            distanceZeroToLine = Mathf.Lerp(0.3f, 1, Mathf.InverseLerp(0, 0.2f, distanceZeroToLine));

            Vector3 targetScale = Vector3.one * distanceZeroToLine;
            bones[i].transform.localScale = targetScale;
        }
    }

    //Start position der controlpoints
    private void InitializeStartRotations()
    {
        armLStartRotation = GetStartRotations(armLEffectors);
        armRStartRotation = GetStartRotations(armREffectors);
        torsoStartRotation = GetStartRotations(torsoEffectors);
        legLStartRotation = GetStartRotations(legLEffectors);
        legRStartRotation = GetStartRotations(legREffectors);
    }

    private Vector3[] GetStartRotations(GameObject[] bodypart)
    {
        Vector3[] rotations = new Vector3[bodypart.Length];
        for (int i = 0; i < bodypart.Length; i++)
        {
            rotations[i] = bodypart[i].transform.localEulerAngles;
        }
        return rotations;
    }
}
