using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetAnimation : MonoBehaviour
{
    [Header("EXTERNAL REFERENCES")]
    [SerializeField] private Transform lineStart;
    private float distanceZeroToLine;

    [Header("ROTATION VALUES")]
    [SerializeField] private float armLDelay = 1.0f;
    //[Range(-90f, 90f)] // Slider 
    [SerializeField] private Vector3 rotationMultiplier = new Vector3(50, 50, 50);
    private Vector3 multiplierL;
    private Vector3 multiplierR;

    [Header("ROTATEABLE OBJECTS")]  //rotates the limbs
    //fill in the inspector with the joints
    [SerializeField] private Transform armLControlPoint;
    [SerializeField] private GameObject[] armLBones;
    private Vector3 [] armLStartRotation;
    [SerializeField] private Transform armRControlPoint;
    [SerializeField] private GameObject[] armRBones;
    private Vector3 [] armRStartRotation;


    // Start is called before the first frame update
    void Start()
    {
        CheckStartRotation();
        multiplierL = rotationMultiplier;
        multiplierR = rotationMultiplier * -1;
    }

    // Update is called once per frame
    void Update()
    {
        RotateArmL();
        RotateArmR();
        
    }

    private void RotateArmL(){
        //nehme die startposition des point aus dem createline script
        //nimm den Abstand von der startposition um den _rotaionValueArmL zu berechnen
        //evt schreibe eine funktion in den anderen Scripts die die Startposition der Punkte zurückgibt
        for(int i = 0; i < armLBones.Length; i++){
            distanceZeroToLine = lineStart.position.y - armLControlPoint.position.y;
            armLBones[i].transform.localEulerAngles = (multiplierL * distanceZeroToLine) + armLStartRotation[i];
            //Debug.Log(armLControlPoint.position.y);
            
        }

    }

    private void RotateArmR(){
        //nehme die startposition des point aus dem createline script
        //nimm den Abstand von der startposition um den _rotaionValueArmL zu berechnen
        //evt schreibe eine funktion in den anderen Scripts die die Startposition der Punkte zurückgibt
        for(int i = 0; i < armLBones.Length; i++){
            distanceZeroToLine = lineStart.position.y - armRControlPoint.position.y;
            armRBones[i].transform.localEulerAngles = (multiplierR * distanceZeroToLine) - armRStartRotation[i] ;
            //Debug.Log(armLControlPoint.position.y);
        }

    }

    private void CheckStartRotation(){
        //instantiate Array
        armLStartRotation = new Vector3[armLBones.Length];

        //fill the array with the start rotation
        for(int i = 0; i < armLBones.Length; i++){
            armLStartRotation[i] = armLBones[i].transform.localEulerAngles;
        }
        //instantiate Array
        armRStartRotation = new Vector3[armRBones.Length];

        //fill the array with the start rotation
        for(int i = 0; i < armRBones.Length; i++){
            armRStartRotation[i] = armRBones[i].transform.localEulerAngles;
        }

    }
}
