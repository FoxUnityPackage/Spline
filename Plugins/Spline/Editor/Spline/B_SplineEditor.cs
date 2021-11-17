using System;
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
            Vector3 newPos = Handles.FreeMoveHandle( self.points[i].point, Quaternion.identity,
                HandleUtility.GetHandleSize( self.points[i].point) * m_pointSize.floatValue, Vector3.one, Handles.SphereHandleCap);
            
            self.points[i] = new B_Spline.Point{point = newPos};
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

    void Space2DSetting()
    {
                float itemWidth =  EditorGUIUtility.currentViewWidth / 3f - 10f;
        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical(GUILayout.Width(itemWidth));
            {
                GUILayout.Label("");
                if (GUILayout.Button("Apply 2D"))
                {
                    switch (m_space2D.enumValueIndex)
                    {
                        case 0 : // XY
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new B_Spline.Point{point = new Vector3{x = self.points[i].point.x, y = self.points[i].point.y, z = m_base.floatValue}};
                            }
                            break;
                        case 1 : // XZ
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new B_Spline.Point{point = new Vector3{x = self.points[i].point.x, y = m_base.floatValue, z = self.points[i].point.z}};
                            }
                            break;
                        case 2 : // YZ
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new B_Spline.Point{point = new Vector3{x = m_base.floatValue, y = self.points[i].point.y, z = self.points[i].point.z}};
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    EditorUtility.SetDirty(target);
                }
            } GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(itemWidth));
            {
                GUILayout.Label("Space");
                EditorGUILayout.PropertyField(m_space2D, GUIContent.none, false, GUILayout.Width(itemWidth));
            } GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(itemWidth));
            {
                GUILayout.Label("Base");
                EditorGUILayout.PropertyField(m_base, GUIContent.none, false, GUILayout.Width(itemWidth));
            } GUILayout.EndVertical();
            
        } GUILayout.EndHorizontal();
    }
}