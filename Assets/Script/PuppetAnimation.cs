using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Rendering;

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
    private Vector3 multiplierL;
    private Vector3 multiplierR;
    private Tween normalizerTween;
    private float normalizer = 1;

    [Header("ROTATEABLE OBJECTS")]  //rotates the limbs
    //fill in the inspector with the joints
    [SerializeField] private Transform armLControlPoint;
    [SerializeField] private Transform armLControlPointDelay;
    [SerializeField] private GameObject[] armLBones;
    private Vector3[] armLStartRotation;
    [SerializeField] private Transform armRControlPoint;
    [SerializeField] private Transform armRControlPointDelay;
    [SerializeField] private GameObject[] armRBones;
    private Vector3[] armRStartRotation;
    [SerializeField] private Transform torsoControlPoint;
    [SerializeField] private Transform torsoControlPointDelay;
    [SerializeField] private GameObject[] torsoBones;
    private Vector3[] torsoStartRotation;
    [SerializeField] private Transform baseControlPoint;
    [SerializeField] private GameObject baseBone;
    [SerializeField] private Transform legLControlPoint;
    [SerializeField] private Transform legLControlPointDelay;
    [SerializeField] private GameObject[] legLBones;
    private Vector3[] legLStartRotation;
    [SerializeField] private Transform legRControlPoint;
    [SerializeField] private Transform legRControlPointDelay;
    [SerializeField] private GameObject[] legRBones;
    private Vector3[] legRStartRotation;
    private bool isTweening = false;
    private bool notStarted = true;
    private float timeElapsed; // Keeps track of time
    private Vector3 boneRotation;


    // Start is called before the first frame update
    void Start()
    {
        lineStart = CreateLine.Instance.LineStart.transform;

        InitializeStartRotations();

        multiplierR = armRotationMultiplier;
        multiplierL = armRotationMultiplier * -1;

        normalizerTween = DOTween.To(() => normalizer, x => normalizer = x, -1, 2)
               .SetEase(Ease.InOutSine)
               .SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime; // Time tracker

        Rotate(armLBones, armLStartRotation, armLControlPoint, armLControlPointDelay, multiplierL);
        Rotate(armRBones, armRStartRotation, armRControlPoint, armRControlPointDelay, multiplierR);
        Rotate(legLBones, legLStartRotation, legLControlPoint, legLControlPointDelay, multiplierL);
        Rotate(legRBones, legRStartRotation, legRControlPoint, legRControlPointDelay, multiplierR);
        Rotate(torsoBones, torsoStartRotation, torsoControlPoint, torsoControlPointDelay, torsoMultiplier);
        // Bounce();

    }

    private void Rotate(GameObject[] bodypart, Vector3[] startRot, Transform controlPoint, Transform controlPointDelay, Vector3 multiplier)
    {
        //nehme die startposition des point aus dem createline script
        //nimm den Abstand von der startposition um den _rotaionValueArmL zu berechnen
        //evt schreibe eine funktion in den anderen Scripts die die Startposition der Punkte zurückgibt)

        // if (controlPoint == torsoControlPoint)
        // {
        //     multiplier = torsoMultiplier * normalizer;
        // }

        for (int i = 0; i < bodypart.Length; i++)
        {
            if (i == 0) distanceZeroToLine = lineStart.position.y - controlPoint.position.y;
            else distanceZeroToLine = lineStart.position.y - controlPointDelay.position.y;

            Vector3 targetRotation = (multiplier * distanceZeroToLine) + startRot[i]; //targetRotation 
            bodypart[i].transform.localEulerAngles = targetRotation;

            if (affectScale)
            {
                Vector3 scaleAdjustment = Vector3.one * distanceZeroToLine;

                bodypart[i].transform.localScale = Vector3.one + scaleAdjustment;
            }
        }
    }

    private void RotateNew(GameObject[] bodypart, Vector3[] startRot, Transform controlPoint, Vector3 multiplier)
    {
        //nehme die startposition des point aus dem createline script
        //nimm den Abstand von der startposition um den _rotaionValueArmL zu berechnen
        //evt schreibe eine funktion in den anderen Scripts die die Startposition der Punkte zurückgibt)
        distanceZeroToLine = lineStart.position.y - controlPoint.position.y;

        Vector3 targetRotation = multiplier * distanceZeroToLine;

        if (controlPoint == torsoControlPoint)
        {
            multiplier = torsoMultiplier * normalizer;
        }

        for (int i = 0; i < bodypart.Length; i++)
        {
            boneRotation = targetRotation + startRot[i];

            StartCoroutine(RotateWithDelay(bodypart[i], i, boneRotation));

            if (affectScale)
            {
                Vector3 scaleAdjustment = Vector3.one * distanceZeroToLine;

                bodypart[i].transform.localScale = Vector3.one + scaleAdjustment;
            }
        }
    }

    private IEnumerator RotateWithDelay(GameObject bone, int i, Vector3 rotation)
    {
        yield return new WaitForSeconds(i * 2f);
        //if (i > 1) Debug.Log("Bone: " + bone.name + " Rotation: " + rotation);
        bone.transform.localEulerAngles = rotation;
    }

    private void Bounce()
    {
        float t = 0;
        float speed = 20f;
        float posY = baseBone.transform.position.y;

        t += speed * Time.deltaTime;
        distanceZeroToLine = lineStart.position.y - baseControlPoint.position.y;

        if (isTweening == false)
        {
            baseBone.transform.DOShakePosition(distanceZeroToLine * -1 * speed, 0.05f, 0, 50, false, false, ShakeRandomnessMode.Harmonic)
                    .SetEase(Ease.InOutSine)
                    .OnStart(() =>
                    {
                        isTweening = true;
                    })
                    .OnComplete(() =>
                    {
                        isTweening = false;
                    });
        }
    }

    private void InitializeStartRotations()
    {
        armLStartRotation = GetStartRotations(armLBones);
        armRStartRotation = GetStartRotations(armRBones);
        torsoStartRotation = GetStartRotations(torsoBones);
        legLStartRotation = GetStartRotations(legLBones);
        legRStartRotation = GetStartRotations(legRBones);
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
