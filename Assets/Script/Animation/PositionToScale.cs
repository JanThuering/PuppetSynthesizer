using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionToScale : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private GameObject effector;
    [SerializeField] private Transform linePos;
    [SerializeField] private GameObject scaledObject1;
    private Transform effectorTransf;
    private float startingPosY;
    private Transform scaledObjTransf1;
    [Header("Animation Values")]
    [SerializeField] private float minScale = -0.5f;
    [SerializeField] private float maxScale = 0.5f;
    [SerializeField] private float smoothness = 1f;

    // Start is called before the first frame update
    void Start()
    {
        scaledObjTransf1 = scaledObject1.transform;
        effectorTransf = effector.transform;
        startingPosY = linePos.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        float yMove = effectorTransf.position.y - startingPosY;
        float normalYMove = Mathf.Lerp(minScale, maxScale, yMove);
        float t = 0;
        t += smoothness * Time.deltaTime;

        float scaleX = Mathf.Lerp(scaledObjTransf1.localScale.x, normalYMove, t);
        float scaleY = Mathf.Lerp(scaledObjTransf1.localScale.y, normalYMove, t);
        float scaleZ = Mathf.Lerp(scaledObjTransf1.localScale.z, normalYMove, t);
        scaledObject1.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
}
