using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that moves, rotates and scales an object based on a sine wave
/// </summary>
public class SineMovement : MonoBehaviour
{
    // ----- Configuration variables  -----
    [Header("Amplitudes")]
    public bool objectSpace = false;
    public Vector3 movement;
    public Vector3 rotation;
    public Vector3 scaleChange;
    [Header("Controls")]
    [Range(0f, 5f)]
    public float strength = 1f; // New amplitude strength variable
    [Range(0.05f, 15f)]
    public float duration = 5f;
    [Range(0f, 1f)]
    public float delay = 0f;
    [Header("Gizmos")]
    public bool sinePosition = false;
    public bool drawPath = false;

    // ----- Internal state variables -----
    [NonSerialized]
    public float time;
    [NonSerialized]
    public Vector3 movementAxis;
    [NonSerialized]
    public Vector3 scaleAxis;
    [NonSerialized]
    public Vector3 rotationAxis;
    protected Vector3 lastOffset;
    protected Vector3 lastObjectSpaceOffset;
    protected Vector3 lastScaleOffset;
    protected Vector3 lastRotationOffset;
    protected Vector3 initialPosition;

    // List to store positions for drawing the movement path
    private List<Vector3> positions = new List<Vector3>();

    // ----- Sine Behaviour 3D -----
    protected void Start()
    {
        // Initialize time with delay
        time = 0; // Start time at 0
        // Duration must not be zero
        if (duration == 0)
        {
            duration = 0.1f;
        }
        // Initialize amplitudes
        movementAxis = movement;
        rotationAxis = rotation;
        scaleAxis = scaleChange;
        // Save initial position
        initialPosition = transform.position;
    }

    protected void Update()
    {
        UpdateSine();
        // Store the current position for drawing the movement path
        if (drawPath)
        {
            positions.Add(transform.position);
        }
    }

    protected void UpdateSine()
    {
        // Update time
        time += 2 * Mathf.PI * Time.deltaTime / duration;

        // Calculate sin(time) once
        float sinTime = Mathf.Sin(time - 2 * Mathf.PI * delay);

        // Calculate movement based on time and movement amplitude
        if (movementAxis != Vector3.zero) // only if axis is not 0
        {
            // Calculate movement relative to world space if objectSpace is false
            if (!objectSpace) {
                var offset = movementAxis * sinTime * strength;
                var newPosition = transform.position + offset - lastOffset;
                transform.position = newPosition;
                lastOffset = offset;
            }

            // Calculate movement relative to object space if objectSpace is true
            if (objectSpace)
            {
                var offsetRelative = Vector3.zero;
                offsetRelative += transform.right * sinTime * movement.x * strength;
                offsetRelative += transform.up * sinTime * movement.y * strength;
                offsetRelative += transform.forward * sinTime * movement.z * strength;
                var newRelativePosition = transform.position + offsetRelative - lastObjectSpaceOffset;
                transform.position = newRelativePosition;
                lastObjectSpaceOffset = offsetRelative;
            }
        }

        // Calculate rotation based on time and rotation amplitude
        if (rotationAxis != Vector3.zero) // only if rotation is not 0
        {
            var rotationOffset = Vector3.zero;

            // Sine-based rotation
            rotationOffset += rotationAxis * sinTime * strength;
            var rotationDifference = rotationOffset - lastRotationOffset;
            transform.Rotate(rotationDifference, Space.Self);
            lastRotationOffset = rotationOffset;
        }

        // Calculate scale change based on time and scale change amplitude
        if (scaleAxis != Vector3.zero) // only if size is not 0
        {
            var scaleOffset = scaleAxis * sinTime * strength;
            var newScale = transform.localScale + scaleOffset - lastScaleOffset;
            transform.localScale = newScale;
            lastScaleOffset = scaleOffset;
        }
    }

    private void OnDrawGizmos()
    {
        if (!enabled) return; // Do not draw gizmos if the script is disabled

        // Draw the gizmos if enabled
        if (sinePosition && strength != 0)      
        {
            // Calculate the sine wave offset
            float sinTime = Mathf.Sin(time - 2 * Mathf.PI * delay);
            Vector3 offset = movementAxis * sinTime * strength;

            // Calculate the position of the cross
            Vector3 crossPosition = initialPosition + offset;

            // Draw the cross
            Gizmos.color = Color.white;
            Gizmos.DrawLine(crossPosition + Vector3.left * 0.75f, crossPosition + Vector3.right * 0.75f);
            Gizmos.DrawLine(crossPosition + Vector3.up * 0.75f, crossPosition + Vector3.down * 0.75f);

            // Draw a Cross at the initial position
            Gizmos.color = Color.white;
            Gizmos.DrawLine(initialPosition + Vector3.left * 0.6f + Vector3.up * 0.6f, initialPosition + Vector3.right * 0.6f + Vector3.down * 0.6f);
            Gizmos.DrawLine(initialPosition + Vector3.right * 0.6f + Vector3.up * 0.6f, initialPosition + Vector3.left * 0.6f + Vector3.down * 0.6f);
        }

        // Draw the movement path if enabled
        if (drawPath)
        {
            Gizmos.color = Color.grey;
            for (int i = 1; i < positions.Count; i++)
            {
                Gizmos.DrawLine(positions[i - 1], positions[i]);
            }
        }
    }
}