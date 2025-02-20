using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PuppetAnimation : MonoBehaviour
{
    [Header("EXTERNAL REFERENCES")]
    [SerializeField] private Transform lineStart;
    private float distanceZeroToLine;

    [Header("ROTATION VALUES")]
    [SerializeField] private float delay = 1.0f;
    //[Range(-90f, 90f)] // Slider 
    [SerializeField] private Vector3 rotationMultiplier = new Vector3(50, 50, 50);
    [SerializeField] private Vector3 torsoMultiplier = new Vector3(50, 0, 50);
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
    private bool notStarted = true;
    private bool isCoRoutineing = false;


    // Start is called before the first frame update
    void Start()
    {
        CheckStartRotation();

        multiplierL = rotationMultiplier;
        multiplierR = rotationMultiplier * -1;

        normalizerTween = DOTween.To(() => normalizer, x => normalizer = x, -1, 2)
               .SetEase(Ease.InOutSine)
               .SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        Rotate(armLBones, armLStartRotation, armLControlPoint, multiplierL);
        Rotate(armRBones, armRStartRotation, armRControlPoint, multiplierR);
        //Rotate(torsoBones, torsoStartRotation, torsoControlPoint, torsoMultiplier);
        Pirouette();
    }

    private void Rotate(GameObject[] bodypart, Vector3[] startRot, Transform controlPoint, Vector3 multiplier)
    {
        //nehme die startposition des point aus dem createline script
        //nimm den Abstand von der startposition um den _rotaionValueArmL zu berechnen
        //evt schreibe eine funktion in den anderen Scripts die die Startposition der Punkte zurückgibt)
        if (controlPoint.name == "Torso")
        {
            multiplier = torsoMultiplier * normalizer;
        }

        for (int i = 0; i < bodypart.Length; i++)
        {
            distanceZeroToLine = lineStart.position.y - controlPoint.position.y;
            bodypart[i].transform.localEulerAngles = (multiplier * distanceZeroToLine) + startRot[i];
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


    private void CheckStartRotation()
    {
        //instantiate Array
        armLStartRotation = new Vector3[armLBones.Length];

        //fill the array with the start rotation
        for (int i = 0; i < armLBones.Length; i++)
        {
            armLStartRotation[i] = armLBones[i].transform.localEulerAngles;
        }
        //instantiate Array
        armRStartRotation = new Vector3[armRBones.Length];

        //fill the array with the start rotation
        for (int i = 0; i < armRBones.Length; i++)
        {
            armRStartRotation[i] = armRBones[i].transform.localEulerAngles;
        }
        //instantiate Array
        torsoStartRotation = new Vector3[torsoBones.Length];

        //fill the array with the start rotation
        for (int i = 0; i < torsoBones.Length; i++)
        {
            torsoStartRotation[i] = torsoBones[i].transform.localEulerAngles;
        }
    }
}
