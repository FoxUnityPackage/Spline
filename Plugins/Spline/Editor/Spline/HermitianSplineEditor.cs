using UnityEditor;
using UnityEngine;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(HermitianSpline))]
public class HermitianSplineEditor : SplineEditor<HermitianSpline>
{
    protected virtual void OnSceneGUI()
    {
        if (!self.enabled)
            return;
        
        if (self.points.Count > 1)
            Handles.DrawAAPolyLine(self.MakeSplinePoints(m_splineDivision.intValue));
        
        for (int i = self.GetMinIndex(); i <= self.GetMaxIndex(); i += self.GetIndexStep())
        {
            Handles.color = Color.green;
            Vector3 newPos = Handles.FreeMoveHandle( self.points[i], Quaternion.identity,
                HandleUtility.GetHandleSize( self.points[i]) * m_pointSize.floatValue, Vector3.one, Handles.SphereHandleCap);
            
            Handles.color = Color.cyan;
            Handles.DrawLine(self.points[i], self.points[i] + self.points[i + 1]);


            Vector3 handlePos = self.points[i] + self.points[i + 1];
            
            Vector3 newHandlePos = Handles.FreeMoveHandle(handlePos, Quaternion.identity,
                HandleUtility.GetHandleSize(handlePos) * self.handleSize, Vector3.one, Handles.SphereHandleCap);

            self.points[i] = newPos;
            self.points[i + 1] = newHandlePos - self.points[i];
        }
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (self.points.Count > 1)
        {
            GUILayout.Label("Portion control");
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add"))
                {
                    Vector3 deriv = self.points[self.points.Count - 1];
                    Vector3 pt = self.points[self.points.Count - 2];
                    
                    self.points.Add(pt);
                    self.points.Add(deriv);

                    EditorUtility.SetDirty(target);
                }

                if (GUILayout.Button("Remove last"))
                {
                    self.points.RemoveAt(self.points.Count - 1);
                    self.points.RemoveAt(self.points.Count - 1);
                    EditorUtility.SetDirty(target);
                }

            }
            EditorGUILayout.EndHorizontal();
        }

        SplineEditorUtility.DrawUILine(Color.gray);
        GUILayout.Label("Editor settings :");
        m_pointSize.floatValue = EditorGUILayout.FloatField("Point size", m_pointSize.floatValue);
        EditorGUI.BeginChangeCheck();
        m_splineDivision.intValue = EditorGUILayout.IntField("Curve precision", m_splineDivision.intValue);
        if (EditorGUI.EndChangeCheck())
        {
            m_splineDivision.intValue = Mathf.Clamp(m_splineDivision.intValue, 3, 1000);
            EditorUtility.SetDirty(target);
        }
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        CloseShapeSetting();
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        Space2DSetting();
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        ImportExportSetting();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    void CloseShapeSetting()
    {
        if (self.points.Count > 3)
        {
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Close shape"))
            {
                self.points.Add(self.points[0]);
                self.points.Add(self.points[1]);
                EditorUtility.SetDirty(target);
            }
        }
    }
}