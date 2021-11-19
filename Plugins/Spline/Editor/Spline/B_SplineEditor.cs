using UnityEditor;
using UnityEngine;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(B_Spline))]
public class B_SplineEditor : SplineEditor<B_Spline>
{
    protected virtual void OnSceneGUI()
    {
        if (!self.enabled)
            return;
        
        if (self.points.Count > 3)
            Handles.DrawAAPolyLine(self.MakeSplinePoints(m_splineDivision.intValue));
        
        for (int i = 0; i < self.points.Count; i++)
        {
            Handles.color = Color.green;
            self.points[i] = Handles.FreeMoveHandle( self.points[i], Quaternion.identity,
                HandleUtility.GetHandleSize( self.points[i]) * m_pointSize.floatValue, Vector3.one, Handles.SphereHandleCap);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SplineEditorUtility.DrawUILine(Color.gray);
        GUILayout.Label("Editor settings :");
        EditorGUILayout.PropertyField(m_pointSize, new GUIContent("Point size"));
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(m_splineDivision, new GUIContent("Curve precision"));
        if (EditorGUI.EndChangeCheck())
        {
            m_splineDivision.intValue = Mathf.Clamp(m_splineDivision.intValue, 3, 1000);
            EditorUtility.SetDirty(target);
        }

        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        Space2DSetting();
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        ImportExportSetting();
        
        serializedObject.ApplyModifiedProperties();
    }
}