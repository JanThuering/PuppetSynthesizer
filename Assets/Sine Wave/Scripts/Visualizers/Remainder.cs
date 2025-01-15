using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Remainder : MonoBehaviour
{
    [Range(1f, 10f)] public float divisor = 5f;
}


#if UNITY_EDITOR
[CustomEditor(typeof(Remainder))]
public class RemainderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Style
        EditorStyles.label.richText = true;
        EditorStyles.label.fontStyle = FontStyle.Bold;

        // Property
        var divisorProperty = serializedObject.FindProperty("divisor");
        EditorGUILayout.PropertyField(divisorProperty, new GUIContent("<color=#FFD700>Divisor</color>"));
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(20);

        // Function
        var divisorColor = "<color=#FFD700>divisor</color>";
        EditorGUILayout.LabelField($"y(t) = t % {divisorColor}");

        // Graph
        var remainderFunction = target as Remainder;
        var r = EditorGUILayout.GetControlRect(GUILayout.Height(150));
        var divisor = remainderFunction.divisor;
        var count = 300;
        var step = 10f / count;
        var points = new List<Vector3>();
        float previousY = 0f;
        bool isDiscontinuity = false;

        for (int i = 0; i <= count; i++)
        {
            var t = i * step;
            var x1 = r.x + (t / 10f) * r.width;
            var yValue = t % divisor;
            var yNormalized = yValue / divisor;
            var y1 = r.y + r.height - yNormalized * r.height;

            var currentPoint = new Vector3(x1, y1);

            // Discontinuities
            if (i > 0 && Mathf.Abs(yValue - previousY) > divisor / 2)
            {
                isDiscontinuity = true;
            }
            else
            {
                if (isDiscontinuity)
                {
                    points.Add(new Vector3((points[^1].x + currentPoint.x) / 2, r.y + r.height));
                    isDiscontinuity = false;
                }
                points.Add(currentPoint);
            }
            previousY = yValue;
        }

        // Light vs dark unity theme
        var isProSkin = EditorGUIUtility.isProSkin;
        Handles.color = isProSkin ? Color.white : Color.black;

        Handles.DrawPolyLine(points.ToArray());

        // Cursor hover
        var mousePosition = Event.current.mousePosition;
        if (r.Contains(mousePosition))
        {
            var t = ((mousePosition.x - r.x) / r.width) * 10f;
            var yValue = t % divisor;
            var yNormalized = yValue / divisor;
            var tValue = t.ToString("F2");
            var yDisplayValue = yValue.ToString("F2");

            var circlePosition = new Vector3(mousePosition.x, r.y + r.height - yNormalized * r.height);
            Handles.DrawSolidDisc(circlePosition, Vector3.forward, 2f);
            Handles.Label(circlePosition + new Vector3(10, 0), $"y = {yDisplayValue}");
            Handles.DrawLine(new Vector3(mousePosition.x, r.y), new Vector3(mousePosition.x, r.y + r.height));
            Handles.Label(new Vector3(mousePosition.x + 5, r.y + r.height - 5), $"t = {tValue}");
        }

        // Repaint
        if (Event.current.type == EventType.Repaint)
        {
            HandleUtility.Repaint();
        }
    }
}
#endif
