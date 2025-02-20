using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateJoints : MonoBehaviour
{
    [Header("ROTATION VALUES")]
    [SerializeField] private float _armLDelay = 1.0f;
    //[Range(-90f, 90f)] // Slider 
    [SerializeField] private Vector3 _rotationValueArmL = new Vector3(1, 0, 0);
    private CreateLine _csCreateLine;
    private MovePuppet _csMovePuppet;
    


    /*
    [SerializeField] private float _armRDelay = 1.0f;
    [SerializeField] private float _legLDelay = 1.0f;
    [SerializeField] private float _legRDelay = 1.0f;
    [SerializeField] private float _torsoDelay = 1.0f;
    */

    [Header("ROTATEABLE OBJECTS")]  //rotates the limbs
    //fill in the inspector with the joints
    [SerializeField] private Transform _armLControlPoint;
    [SerializeField] private GameObject[] _armLBones;
    private Vector3 [] _armLStartRotation;

    /*
    [SerializeField] private GameObject _armRControlPoint;
    [SerializeField] private GameObject[] _armR;
    [SerializeField] private GameObject _legLControlPoint;
    [SerializeField] private GameObject[] _legL;
    [SerializeField] private GameObject _legRControlPoint;
    [SerializeField] private GameObject[] _legR;
    [SerializeField] private GameObject _torsoControlPoint;
    [SerializeField] private GameObject[] _torso;
    */



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

        for(int i = 0; i < _armLBones.Length; i++){
            _armLBones[i].transform.localEulerAngles = (_rotationValueArmL * _armLControlPoint.position.y) + _armLStartRotation[i];
            Debug.Log(_armLControlPoint.position.y);
            
        }

    }

    private void CheckStartRotation(){
        //instantiate Array
        _armLStartRotation = new Vector3[_armLBones.Length];

        //fill the array with the start rotation
        for(int i = 0; i < _armLBones.Length; i++){
            _armLStartRotation[i] = _armLBones[i].transform.localEulerAngles;
        }

    }
}
