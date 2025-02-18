using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateJoints : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private CreateLine _csCreateLine;

    [Header("ROTATION VALUES")]
    [SerializeField] private float _armLDelay = 1.0f;
    [SerializeField] private float _armRDelay = 1.0f;
    [SerializeField] private float _legLDelay = 1.0f;
    [SerializeField] private float _legRDelay = 1.0f;
    [SerializeField] private float _torsoDelay = 1.0f;

    [Header("ROTATEABLE OBJECTS")]  //rotates the limbs
    //fill in the inspector with the joints
    [SerializeField] private GameObject _armLControlPoint;
    [SerializeField] private GameObject[] _armL;
    [SerializeField] private GameObject _armRControlPoint;
    [SerializeField] private GameObject[] _armR;
    [SerializeField] private GameObject _legLControlPoint;
    [SerializeField] private GameObject[] _legL;
    [SerializeField] private GameObject _legRControlPoint;
    [SerializeField] private GameObject[] _legR;
    [SerializeField] private GameObject _torsoControlPoint;
    [SerializeField] private GameObject[] _torso;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
