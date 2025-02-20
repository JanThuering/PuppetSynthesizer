using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetAnimation : MonoBehaviour
{
    [Header("ROTATION VALUES")]
    [SerializeField] private float armLDelay = 1.0f;
    //[Range(-90f, 90f)] // Slider 
    [SerializeField] private Vector3 rotationValueArmL = new Vector3(1, 0, 0);

    [Header("ROTATEABLE OBJECTS")]  //rotates the limbs
    //fill in the inspector with the joints
    [SerializeField] private Transform armLControlPoint;
    [SerializeField] private GameObject[] armLBones;
    private Vector3 [] armLStartRotation;


    // Start is called before the first frame update
    void Start()
    {
        CheckStartRotation();
    }

    // Update is called once per frame
    void Update()
    {
        RotateArmL();
        
    }

    private void RotateArmL(){
        //nehme die startposition des point aus dem createline script
        //nimm den Abstand von der startposition um den _rotaionValueArmL zu berechnen
        //evt schreibe eine funktion in den anderen Scripts die die Startposition der Punkte zurückgibt

        for(int i = 0; i < armLBones.Length; i++){
            armLBones[i].transform.localEulerAngles = (rotationValueArmL * armLControlPoint.position.y) + armLStartRotation[i];
            Debug.Log(armLControlPoint.position.y);
            
        }

    }

    private void CheckStartRotation(){
        //instantiate Array
        armLStartRotation = new Vector3[armLBones.Length];

        //fill the array with the start rotation
        for(int i = 0; i < armLBones.Length; i++){
            armLStartRotation[i] = armLBones[i].transform.localEulerAngles;
        }

    }
}
