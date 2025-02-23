using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBackGroundDots : MonoBehaviour
{
    GameObject[] dots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private int amountOfDots = 1000;
    [SerializeField] private float size = 0.1f;
    [SerializeField] private Vector3 distribution = new Vector3(10, 10, 10);

    // Start is called before the first frame update
    void Start()
    {
        CreateDots();
    }

    // Update is called once per frame
    void Update()
    {
        //ManipulateDots();
    }

    private void CreateDots(){
        dots = new GameObject[amountOfDots];

        Vector3 startDistributionPos = new(-distribution.x / 2, -distribution.y / 2, -distribution.z / 2);

        int pointsPerAxis = Mathf.CeilToInt(Mathf.Pow(amountOfDots, 1f / 3f));
        float stepX = distribution.x / pointsPerAxis;
        float stepY = distribution.y / pointsPerAxis;
        float stepZ = distribution.z / pointsPerAxis;

        for (int i = 0; i < amountOfDots; i++)
        {
            int x = i % pointsPerAxis;
            int y = i / pointsPerAxis % pointsPerAxis;
            int z = i / (pointsPerAxis * pointsPerAxis);

            Vector3 pos = new(startDistributionPos.x + x * stepX, startDistributionPos.y + y * stepY, startDistributionPos.z + z * stepZ);
            dots[i] = Instantiate(dotPrefab, pos, Quaternion.identity);
            dots[i].transform.localScale = new Vector3(size, size, size);
        }

        // Old code
        /*
        for (int i = 0; i < amountOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), Quaternion.identity);
            dots[i].transform.localScale = new Vector3(size, size, size);
        }
        */
    }

    /*
    private void ManipulateDots(){
        for(int i = 0; i < amountOfDots; i++){
            dots[i].transform.position = new Vector3(dots[i].transform.position.x, height, dots[i].transform.position.z);
        }
    }
    */
}
