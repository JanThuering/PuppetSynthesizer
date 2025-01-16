using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Draws three sine curves and their sum.
/// </summary>
public class DrawSineWaves : MonoBehaviour
{

    [Header("Sine Wave 1")]
    [Range(0f, 1f)] // Slider
    public float amplitude1 = 1f;  // Relative Amplitude of Sine Wave 1
    [Range(1f, 20)] // Slider
    public float frequency1 = 6f;    // Frequency of Sine Wave 1
    [Range(0f, 1f)] // Slider
    public float offset1 = 0f;  // Offset of Sine Wave 1

    [Header("Sine Wave 2")]
    [Range(0f, 1f)] // Slider
    public float amplitude2 = 1f;  // Relative Amplitude of Sine Wave 2
    [Range(1f, 20)] // Slider
    public float frequency2 = 6.5f;    // Frequency of Sine Wave 2
    [Range(0f, 1f)] // Slider
    public float offset2 = 0f;  // Offset of Sine Wave 2

    [Header("Sine Wave 3")]
    [Range(0f, 1f)] // Slider    
    public float amplitude3 = 1f;  // Relative Amplitude of Sine Wave 3
    [Range(1f, 20)] // Slider
    public float frequency3 = 7f;    // Frequency of Sine Wave 3
    [Range(0f, 1f)] // Slider
    public float offset3 = 0f;  // Offset of Sine Wave 3

    [SerializeField]private int pointsCount = 1000;  // Number of points on the curves

    private float startYPositionRelative1 = 0.85f;  // Relative Y-position of Sine Wave 1 (top third of the upper half)
    private float startYPositionRelative2 = 0.65f;   // Relative Y-position of Sine Wave 2 (second third of the upper half)
    private float startYPositionRelative3 = 0.5f;  // Relative Y-position of Sine Wave 3 (middle of the screen)
    // private float startYPositionRelativeSum = 0.25f;  // Relative Y-position of the Sum Wave (bottom quarter of the screen)
    private float frequencyAvg;
    private Color highlightColor = Color.yellow;
    private Color highlightColorAvg = Color.yellow;

    public bool highlightRepetitions = false;
    public bool showSumWave = true;
    public bool showCircles = false;
    [Range(0f, 100f)] // Slider
    public float animationSpeed = 0f; // Speed of the animation

    private void OnDrawGizmos()
    {
        // Set the Gizmos color to white
        Gizmos.color = Color.white;
        if (highlightRepetitions)
        {
            highlightColor = Color.yellow;
            highlightColorAvg = Color.yellow;
        }
        else
        {
            highlightColor = Color.white;
            highlightColorAvg = Color.cyan;
        }

        // Find the main camera
        Camera mainCamera = Camera.main;

        // Check if the main camera is found
        if (mainCamera != null)
        {
            float timeFactor = Time.time * animationSpeed * 5;

            // Calculate points on Sine Wave 1
            Vector3[] points1 = new Vector3[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                float t = i / (float)(pointsCount - 1);
                float x = Mathf.Lerp(0f, Screen.width, t);
                float y1 = Mathf.Sin((x + timeFactor) * frequency1 / Screen.width * 2 * Mathf.PI + offset1 * 2 * Mathf.PI) * Screen.height * (amplitude1 / 15) + Screen.height * startYPositionRelative1;
                Vector3 point1 = mainCamera.ScreenToWorldPoint(new Vector3(x, y1, 10f));
                points1[i] = point1;
            }

            // Draw Sine Wave 1
            Gizmos.color = Color.white;
            for (int i = 0; i < points1.Length - 1; i++)
            {
                Gizmos.DrawLine(points1[i], points1[i + 1]);
            }

            // Highlight the first part of Sine Wave 1 in yellow
            if (amplitude1 > 0)
            {
                Gizmos.color = highlightColor;
            }
            for (int i = 0; i < pointsCount / frequency1; i++)
            {
                if (i < points1.Length - 1)
                {
                    Gizmos.DrawLine(points1[i], points1[i + 1]);
                }
            }

            // Calculate points on Sine Wave 2
            Vector3[] points2 = new Vector3[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                float t = i / (float)(pointsCount - 1);
                float x = Mathf.Lerp(0f, Screen.width, t);
                float y2 = Mathf.Sin((x + timeFactor) * frequency2 / Screen.width * 2 * Mathf.PI + offset2 * 2 * Mathf.PI) * Screen.height * (amplitude2 / 15) + Screen.height * startYPositionRelative2;
                Vector3 point2 = mainCamera.ScreenToWorldPoint(new Vector3(x, y2, 10f));
                points2[i] = point2;
            }

            // Draw Sine Wave 2
            Gizmos.color = Color.white;
            for (int i = 0; i < points2.Length - 1; i++)
            {
                Gizmos.DrawLine(points2[i], points2[i + 1]);
            }

            // Highlight the first part of Sine Wave 2 in yellow
            if (amplitude2 > 0)
            {
                Gizmos.color = highlightColor;
            }
            for (int i = 0; i < pointsCount / frequency2; i++)
            {
                if (i < points2.Length - 1)
                {
                    Gizmos.DrawLine(points2[i], points2[i + 1]);
                }
            }

            // Calculate points on Sine Wave 3
            Vector3[] points3 = new Vector3[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                float t = i / (float)(pointsCount - 1);
                float x = Mathf.Lerp(0f, Screen.width, t);
                float y3 = Mathf.Sin((x + timeFactor) * frequency3 / Screen.width * 2 * Mathf.PI + offset3 * 2 * Mathf.PI) * Screen.height * (amplitude3 / 15) + Screen.height * startYPositionRelative3;
                Vector3 point3 = mainCamera.ScreenToWorldPoint(new Vector3(x, y3, 10f));
                points3[i] = point3;
            }

            // Draw Sine Wave 3
            Gizmos.color = Color.white;
            for (int i = 0; i < points3.Length - 1; i++)
            {
                Gizmos.DrawLine(points3[i], points3[i + 1]);
            }

            // Highlight the first part of Sine Wave 3 in yellow
            if (amplitude3 > 0)
            {
                Gizmos.color = highlightColor;
            }
            for (int i = 0; i < pointsCount / frequency3; i++)
            {
                if (i < points3.Length - 1)
                {
                    Gizmos.DrawLine(points3[i], points3[i + 1]);
                }
            }

            // Calculate points on the sum sine curve (Sum Wave)
            Vector3[] pointsSum = new Vector3[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                pointsSum[i] = new Vector3(
                    points1[i].x,
                    points1[i].y + points2[i].y + points3[i].y,
                    points1[i].z

                ) + new Vector3(0, -19.2f, 0);
            }

            // Draw Sum Wave only if enabled
            if (showSumWave)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < pointsSum.Length - 1; i++)
                {
                    Gizmos.DrawLine(pointsSum[i], pointsSum[i + 1]);
                }

                List<int> frequencies = new List<int>();
                if (amplitude1 != 0) frequencies.Add((int)frequency1);
                if (amplitude2 != 0) frequencies.Add((int)frequency2);
                if (amplitude3 != 0) frequencies.Add((int)frequency3);

                // Highlight the first part of Sum Wave in yellow
                frequencyAvg = frequencies.Count > 0 ? MathUtilities.GCD(frequencies.ToArray()) : 0;
                Gizmos.color = highlightColorAvg;
                if (frequencyAvg != 0)
                {
                    for (int i = 0; i < pointsCount / frequencyAvg; i++)
                    {
                        if (i < pointsSum.Length - 1)
                        {
                            Gizmos.DrawLine(pointsSum[i], pointsSum[i + 1]);
                        }
                    }
                }
            }
            Debug.Log("Sine Wave 1 Y Position (First Point): " + points1[0].y);
            Debug.Log("Sine Wave 2 Y Position (First Point): " + points2[0].y);
            Debug.Log("Sine Wave 3 Y Position (First Point): " + points3[0].y);
            Debug.Log("Sum Wave Y Position (First Point): " + pointsSum[0].y);
            // Draw circles on the waves 50 pixels from the left side of the screen
            if (showCircles)
            {

                float circleX = 50f;
                float circleY1 = Mathf.Sin((circleX + timeFactor) * frequency1 / Screen.width * 2 * Mathf.PI + offset1 * 2 * Mathf.PI) * Screen.height * (amplitude1 / 15) + Screen.height * startYPositionRelative1;
                float circleY2 = Mathf.Sin((circleX + timeFactor) * frequency2 / Screen.width * 2 * Mathf.PI + offset2 * 2 * Mathf.PI) * Screen.height * (amplitude2 / 15) + Screen.height * startYPositionRelative2;
                float circleY3 = Mathf.Sin((circleX + timeFactor) * frequency3 / Screen.width * 2 * Mathf.PI + offset3 * 2 * Mathf.PI) * Screen.height * (amplitude3 / 15) + Screen.height * startYPositionRelative3;
                float circleYSum = (Mathf.Sin((circleX + timeFactor) * frequency1 / Screen.width * 2 * Mathf.PI + offset1 * 2 * Mathf.PI) * amplitude1 +
                                    Mathf.Sin((circleX + timeFactor) * frequency2 / Screen.width * 2 * Mathf.PI + offset2 * 2 * Mathf.PI) * amplitude2 +
                                    Mathf.Sin((circleX + timeFactor) * frequency3 / Screen.width * 2 * Mathf.PI + offset3 * 2 * Mathf.PI) * amplitude3) * Screen.height / 15 + Screen.height * 0.25f - 30f;

                Vector3 circlePoint1 = mainCamera.ScreenToWorldPoint(new Vector3(circleX, circleY1, 10f));
                Vector3 circlePoint2 = mainCamera.ScreenToWorldPoint(new Vector3(circleX, circleY2, 10f));
                Vector3 circlePoint3 = mainCamera.ScreenToWorldPoint(new Vector3(circleX, circleY3, 10f));
                Vector3 circlePointSum = mainCamera.ScreenToWorldPoint(new Vector3(circleX, circleYSum, 10f));

                Gizmos.color = Color.white;
                if (amplitude1 > 0)
                {
                    Gizmos.DrawSphere(circlePoint1, 0.025f); // Half size
                }
                if (amplitude2 > 0)
                {
                    Gizmos.DrawSphere(circlePoint2, 0.025f); // Full size
                }
                if (amplitude3 > 0)
                {
                    Gizmos.DrawSphere(circlePoint3, 0.025f); // Full size
                }
                if (showSumWave)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(circlePointSum, 0.025f); // Full size
                }

                //yFirstPointSineWave1 = circleY1;
            }
        }
        else
        {
            Debug.LogError("Main camera not found. Ensure the camera is tagged as 'Main Camera'.");
        }
    }
}

public class MathUtilities
{
    // Function to calculate the Greatest Common Divisor (GCD) for an array of integers
    public static int GCD(params int[] numbers)
    {
        int result = numbers[0];
        for (int i = 1; i < numbers.Length; i++)
        {
            result = GCD(result, numbers[i]);
        }

        int GCD(int x, int y) => y == 0 ? x : GCD(y, x % y);
        return result;
    }
}
