using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Script that generates a cylinder mesh and updates it dynamically
/// </summary>

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BodyOfRevolution : MonoBehaviour
{
    // ----- Configuration variables  -----
    public float topRadius = 0.5f;
    public float bottomRadius = 0.5f;
    public float height = 1.5f;
    public int rotationSegments = 32;
    public float bevel = 0.01f;
    public float connectionOffset = 0.1f;

    // ----- Internal state variables -----
    private bool simpleMesh = false; // if true, only 2 vertices and 1 triangle
    private Mesh mesh;
    private Vector3[] normals;
    private bool smoothShading = false;
    private int heightSegments = 9;
    private float[] radii = new float[10];
    private float[] yPos = new float[10];
    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (simpleMesh)
        {
            heightSegments = 1;
            radii = new float[2];
            yPos = new float[2];
        }
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        InitializeRadiiAndPositions();
        CreateCylinder();
        // AssignMeshCollider();
        transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        // Assign mass to Rigidbody based on dimensions
        AssignMass();

        // Position connections if they exist
        PositionConnections();
    }

    private void InitializeRadiiAndPositions()
    {
        if (simpleMesh)
        {
            yPos[0] = 0;
            yPos[1] = height;
            radii[0] = bottomRadius;
            radii[1] = topRadius;
        }
        else
        {
            // Positions
            yPos[0] = 0;
            yPos[1] = yPos[0];
            yPos[2] = yPos[1] + bevel * 1f;
            yPos[3] = yPos[2] + bevel * 2.5f;
            yPos[4] = yPos[3] + bevel * 4f;
            yPos[9] = height;
            yPos[8] = yPos[9];
            yPos[7] = yPos[8] - bevel * 1f;
            yPos[6] = yPos[7] - bevel * 2.5f;
            yPos[5] = yPos[6] - bevel * 4f;

            // Radii
            radii[4] = bottomRadius;
            radii[3] = radii[4] - bevel;
            radii[2] = radii[3] - bevel * 2.5f;
            radii[1] = radii[2] - bevel * 4;
            radii[0] = 0.02f;
            radii[5] = topRadius;
            radii[6] = radii[5] - bevel;
            radii[7] = radii[6] - bevel * 2.5f;
            radii[8] = radii[7] - bevel * 4;
            radii[9] = 0.02f;
        }
    }

    private void CreateCylinder()
    {
        Vector3[] vertices = new Vector3[(rotationSegments + 1) * (heightSegments + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[rotationSegments * heightSegments * 6];
        normals = new Vector3[vertices.Length];

        for (int i = 0, y = 0; y < radii.Length; y++)
        {
            float currentRadius = radii[y];
            float yPosition = yPos[y];

            for (int x = 0; x <= rotationSegments; x++, i++)
            {
                float angle = x * Mathf.PI * 2f / rotationSegments;
                Vector3 vertex = new Vector3(Mathf.Cos(angle) * currentRadius, yPosition, Mathf.Sin(angle) * currentRadius);
                vertices[i] = vertex;
                normals[i] = vertex.normalized;

                // Calculate UV coordinates
                uvs[i] = new Vector2((float)x / rotationSegments, (float)y / heightSegments * height);
            }
        }

        for (int ti = 0, vi = 0, y = 0; y < heightSegments; y++, vi++)
        {
            for (int x = 0; x < rotationSegments; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + rotationSegments + 1;
                triangles[ti + 5] = vi + rotationSegments + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;

        ApplyShadingType(vertices, triangles);
    }

    private void ApplyShadingType(Vector3[] vertices, int[] triangles)
    {
        if (smoothShading)
        {
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = normals[i].normalized;
            }
        }
        else
        {
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 normal = Vector3.Cross(vertices[triangles[i + 1]] - vertices[triangles[i]],
                                               vertices[triangles[i + 2]] - vertices[triangles[i]]).normalized;

                normals[triangles[i]] = normals[triangles[i + 1]] = normals[triangles[i + 2]] = normal;
            }
        }

        mesh.normals = normals;
    }

    private void AssignMeshCollider()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();

            // If there isn't already a MeshCollider, add one
            if (meshCollider == null)
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
            }
            meshCollider.convex = true;
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }
        else
        {
            Debug.LogError("MeshFilter not found on the GameObject");
        }
    }

    private void AssignMass()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculate the average radius
            float averageRadius = (topRadius + bottomRadius) / 2f;

            // Calculate the mass based on the formula: mass = 0.2 * (averageRadius / 1) * (height / 1)
            float mass = 0.2f * averageRadius * height;

            // Assign the calculated mass to the Rigidbody
            rb.mass = mass;
        }
    }

    private void PositionConnections()
    {
        Transform topConnection = transform.Find("TopConnection");
        Transform bottomConnection = transform.Find("BottomConnection");

        if (topConnection != null)
        {
            topConnection.localPosition = new Vector3(0, height + connectionOffset, 0);
        }

        if (bottomConnection != null)
        {
            bottomConnection.localPosition = new Vector3(0, -connectionOffset, 0);
        }
    }

    private void OnValidate()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        if (mesh == null)
        {
            mesh = new Mesh();
            meshFilter.mesh = mesh;
        }

        InitializeRadiiAndPositions();
        CreateCylinder();

        // Reassign mass to Rigidbody based on new dimensions
        AssignMass();

        // Position connections if they exist
        PositionConnections();
    }

    public void UpdateMesh()
    {
        InitializeRadiiAndPositions();
        CreateCylinder();
        AssignMass();
        PositionConnections();
    }
}