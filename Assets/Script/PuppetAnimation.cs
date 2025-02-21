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
    [SerializeField] private float delay = 0.1f;
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
    [SerializeField] private GameObject[] armLBones;
    private Vector3[] armLStartRotation;
    [SerializeField] private Transform armRControlPoint;
    [SerializeField] private GameObject[] armRBones;
    private Vector3[] armRStartRotation;
    [SerializeField] private Transform torsoControlPoint;
    [SerializeField] private GameObject[] torsoBones;
    private Vector3[] torsoStartRotation;
    [SerializeField] private Transform baseControlPoint;
    [SerializeField] private GameObject baseBone;
    [SerializeField] private Transform legLControlPoint;
    [SerializeField] private GameObject[] legLBones;
    private Vector3[] legLStartRotation;
    [SerializeField] private Transform legRControlPoint;
    [SerializeField] private GameObject[] legRBones;
    private Vector3[] legRStartRotation;
    private bool isTweening = false;
    private bool notStarted = true;
    private float timeElapsed; // Keeps track of time


    // Start is called before the first frame update
    void Start()
    {
        lineStart = CreateLine.Instance.LineStart.transform;

        InitializeStartRotations();

        multiplierL = armRotationMultiplier;
        multiplierR = armRotationMultiplier * -1;

        normalizerTween = DOTween.To(() => normalizer, x => normalizer = x, -1, 2)
               .SetEase(Ease.InOutSine)
               .SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime; // Time tracker

        Rotate(armLBones, armLStartRotation, armLControlPoint, multiplierL);
        Rotate(armRBones, armRStartRotation, armRControlPoint, multiplierR);
        Rotate(legLBones, legLStartRotation, legLControlPoint, multiplierL);
        Rotate(legRBones, legRStartRotation, legRControlPoint, multiplierR);
        Rotate(torsoBones, torsoStartRotation, torsoControlPoint, torsoMultiplier);
        Bounce();
        //Pirouette();
    }

    private void Rotate(GameObject[] bodypart, Vector3[] startRot, Transform controlPoint, Vector3 multiplier)
    {
        //nehme die startposition des point aus dem createline script
        //nimm den Abstand von der startposition um den _rotaionValueArmL zu berechnen
        //evt schreibe eine funktion in den anderen Scripts die die Startposition der Punkte zurückgibt)
        distanceZeroToLine = lineStart.position.y - controlPoint.position.y;

        if (controlPoint == torsoControlPoint)
        {
            multiplier = torsoMultiplier * normalizer;
        }

        float smoothFactor = 20f; // Controls the smoothness of delay

        for (int i = 0; i < bodypart.Length; i++)
        {
            float offsetTime = timeElapsed - (i * delay); // Continuous delay per bone
            float smoothDelay = Mathf.Lerp(1f, 0.9f, Mathf.PingPong(offsetTime * smoothFactor, 1f)); // Smooth motion

            Vector3 targetRotation = (multiplier * distanceZeroToLine * smoothDelay) + startRot[i];

            bodypart[i].transform.localEulerAngles = targetRotation;
            //bodypart[i].transform.localEulerAngles = (multiplier * distanceZeroToLine) + startRot[i];
            if (affectScale)
            {
                Vector3 scaleAdjustment = Vector3.one * distanceZeroToLine;

                bodypart[i].transform.localScale = Vector3.one + scaleAdjustment;
            }
        }
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


    private void Pirouette()
    {
        distanceZeroToLine = lineStart.position.y - baseControlPoint.position.y;

        if (distanceZeroToLine < -1.006f && notStarted == true)
        {
            float yRot = 30;
            // if (baseBone.transform.localEulerAngles.y == 0) yRot = 360;
            // else yRot = 0;

            {
                baseBone.transform.DORotate(new Vector3(0, baseBone.transform.localEulerAngles.y + yRot, 0), 1);
            }
            baseBone.transform.DORotate(new Vector3(0, 0, 0), 1)
            .OnStart(() =>
            {
                notStarted = false;
                print("start");
            })
            .OnComplete(() =>
            {
                notStarted = true;
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
