using System.Linq;
using UnityEditor;
using UnityEngine;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(CatmullRomSpline))]
public class CatmullRomSplineEditor : SplineEditor<CatmullRomSpline>
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
        m_pointSize.floatValue = EditorGUILayout.FloatField("Point size", m_pointSize.floatValue);
        EditorGUI.BeginChangeCheck();
        m_splineDivision.intValue = EditorGUILayout.IntField("Curve precision", m_splineDivision.intValue);
        if (EditorGUI.EndChangeCheck())
        {
            m_splineDivision.intValue = Mathf.Clamp(m_splineDivision.intValue, 3, 1000);
            EditorUtility.SetDirty(target);
        }

        IncludeExtremitySetting();

        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        CloseShapeSetting();
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        Space2DSetting();
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        ImportExportSetting();
        
        serializedObject.ApplyModifiedProperties();
    }

    void IncludeExtremitySetting()
    {
        if (self.points.Count > 3)
        {
            EditorGUI.BeginChangeCheck();
            self.isExtremityAdd = EditorGUILayout.Toggle("Include extremity", self.isExtremityAdd);
            if (EditorGUI.EndChangeCheck())
            {
                IncludeExtremity();
                EditorUtility.SetDirty(target);
            }
        }
    }
    
    void IncludeExtremity()
    {
        if (self.isExtremityAdd)
        {
            self.points.Insert(0, self.points.First());
            self.points.Add(self.points.Last());
        }
        else
        {
            self.points.RemoveAt(0);
            self.points.RemoveAt(self.points.Count - 1);
        }
    }
    
    void CloseShapeSetting()
    {
        if (self.points.Count > 3)
        {
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Close shape"))
            {
                if (!self.isExtremityAdd)
                {
                    self.isExtremityAdd = true;
                    IncludeExtremity();
                }
                
                self.points[self.points.Count - 1] = self.points[self.points.Count - 2] = self.points[0] = self.points[1];
                
                EditorUtility.SetDirty(target);
            }
        }
    }
}