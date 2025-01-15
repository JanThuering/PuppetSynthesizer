using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMovement : MonoBehaviour
{
    [SerializeField] private float amplitude = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        float yValue = Mathf.Sin(Time.deltaTime) * amplitude;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, yValue, gameObject.transform.position.z);

        Debug.Log("NormalTime: " + Time.time);
    }

    private void FixedUpdate()
    {
        Debug.Log("FixedTime: " +Time.time);
    }
}
