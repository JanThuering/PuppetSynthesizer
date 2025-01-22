using UnityEngine;
using UnityEditor;

public class Sinusoid : MonoBehaviour
{
    [Range(0, 50)] public float amplitude = 25f;
    [Range(0, 30)] public float frequency = 1f;
    public float phase = 0f;
}


#if UNITY_EDITOR
[CustomEditor(typeof(Sinusoid))]
public class SinusoidEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Style
        EditorStyles.label.richText = true;
        EditorStyles.label.fontStyle = FontStyle.Bold;

        // Properties
        var amplitudeProperty = serializedObject.FindProperty("amplitude");
        var frequencyProperty = serializedObject.FindProperty("frequency");
        var phaseProperty = serializedObject.FindProperty("phase");
        EditorGUILayout.PropertyField(amplitudeProperty, new GUIContent("<color=#FF4500>Amplitude</color>"));
        EditorGUILayout.PropertyField(frequencyProperty, new GUIContent("<color=#32CD32>Frequency</color>"));
        EditorGUILayout.PropertyField(phaseProperty, new GUIContent("<color=#1E90FF>Phase</color>"));
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(20);

        // Formula
        var amplitudeColor = "<color=#FF4500>amplitude</color>";
        var frequencyColor = "<color=#32CD32>frequency</color>";
        var phaseColor = "<color=#1E90FF>phase</color>";
        EditorGUILayout.LabelField($"y(t) = {amplitudeColor} * sin({frequencyColor} * t + {phaseColor})");

        // Graph
        var sinusoid = target as Sinusoid;
        var r = EditorGUILayout.GetControlRect(GUILayout.Height(150));
        var amplitude = sinusoid.amplitude;
        var frequency = sinusoid.frequency;
        var phase = sinusoid.phase;
        var count = 300;
        var step = 1f / count;
        var points = new Vector3[count];
        for (int i = 0; i < points.Length; i++)
        {
            var t = i * step;
            var x1 = r.x + t * r.width;
            var y1 = r.y + r.height / 2 - amplitude * Mathf.Sin(2 * Mathf.PI * frequency * t + phase);
            points[i] = new Vector3(x1, y1);
        }

        // Light vs dark unity theme
        var isProSkin = EditorGUIUtility.isProSkin;
        Handles.color = isProSkin ? Color.white : Color.black;

        Handles.DrawPolyLine(points);

        // Cursor hover
        var mousePosition = Event.current.mousePosition;
        if (r.Contains(mousePosition))
        {
            var t = (mousePosition.x - r.x) / r.width;
            var y = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * t + phase);
            var tValue = t.ToString("F2");
            var yValue = y.ToString("F2");

            var circlePosition = new Vector3(mousePosition.x, r.y + r.height / 2 - y);
            Handles.DrawSolidDisc(circlePosition, Vector3.forward, 2f);
            Handles.Label(circlePosition + new Vector3(10, 0), yValue);
            Handles.DrawLine(new Vector3(mousePosition.x, r.y), new Vector3(mousePosition.x, r.y + r.height));
            var tLabel = $"t = {tValue}";
            Handles.Label(new Vector3(mousePosition.x + 5, r.y + r.height - 5), tLabel);
        }

        // Repaint
        /*
        if (Event.current.type == EventType.Repaint)
        {
            HandleUtility.Repaint();
        }
        */
    }
}
#endif
