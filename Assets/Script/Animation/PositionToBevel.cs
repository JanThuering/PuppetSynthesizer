using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PositionToBevel : MonoBehaviour
{
    [SerializeField] private GameObject effector;
    private Transform effectorTransf;
    private float effectorStartingPosY;
    private BodyOfRevolution generativeMesh;
    [SerializeField] float maxBevel = 0.2f;
    [SerializeField] float minBevel = -0.2f;
    [SerializeField] float smoothness = 1;

    // Start is called before the first frame update
    void Start()
    {
        generativeMesh = GetComponent<BodyOfRevolution>();
        effectorTransf = effector.transform;
        effectorStartingPosY = effectorTransf.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        YPosToBevelValue();
        generativeMesh.UpdateMesh();
    }

    void YPosToBevelValue()
    {
        float yMove = effectorTransf.position.y - effectorStartingPosY;
        float normalYMove = Mathf.Lerp(minBevel, maxBevel, yMove);
        float t = 0;
        t += smoothness * Time.deltaTime;
        generativeMesh.bevel = Mathf.Lerp(generativeMesh.bevel, normalYMove, t);
    }

}
