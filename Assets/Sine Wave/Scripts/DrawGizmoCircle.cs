using UnityEngine;

/// <summary>
/// Draws a Circle with Draw Gizmos, Either as an outline or as rays from the center with an offset.
/// </summary>
public class DrawGizmoCircle : MonoBehaviour
{
    public float radius = 1f; // Radius of the circle
    public int segments = 36; // Number of segments to draw the circle
    public bool drawOutline = true; // Whether to draw the circle as an outline
    public bool drawRays = false; // Whether to draw rays from the center with an offset
    public float rayOffset = 0.1f; // Offset for the rays
    public Color circleColor = Color.green; // Color of the circle
    [Range(0f, 360f)]
    public float startAngle = 0f; // Start angle of the circle
    [Range(0f, 360f)]
    public float endAngle = 360f; // End angle of the circle

    private void OnDrawGizmos()
    {
        Gizmos.color = circleColor;

        if (drawOutline)
        {
            DrawCircleOutline();
        }

        if (drawRays)
        {
            DrawCircleRays();
        }
    }

    private void DrawCircleOutline()
    {
        float angleStep = (endAngle - startAngle) / segments;
        Vector3 prevPoint = transform.position + new Vector3(Mathf.Cos(startAngle * Mathf.Deg2Rad) * radius, Mathf.Sin(startAngle * Mathf.Deg2Rad) * radius, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector3 newPoint = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius, 0);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

    private void DrawCircleRays()
    {
        float angleStep = (endAngle - startAngle) / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * rayOffset, Mathf.Sin(angle * Mathf.Deg2Rad) * rayOffset, 0);
            Vector3 rayEnd = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius, 0);
            Gizmos.DrawRay(transform.position + offset, rayEnd - transform.position - offset);
        }
    }
}