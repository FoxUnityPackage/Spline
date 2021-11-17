using System;
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
        
        for (int i = 0; i < self.points.Count; i++)
        {
            Handles.color = Color.green;
            Vector3 newPos = Handles.FreeMoveHandle( self.points[i].point, Quaternion.identity,
                HandleUtility.GetHandleSize( self.points[i].point) * m_pointSize.floatValue, Vector3.one, Handles.SphereHandleCap);
            
            Handles.color = Color.cyan;
            Handles.DrawLine(self.points[i].point, self.points[i].point + self.points[i].derivative);


            Vector3 handlePos = self.points[i].point + self.points[i].derivative;
            
            Vector3 newHandlePos = Handles.FreeMoveHandle(handlePos, Quaternion.identity,
                HandleUtility.GetHandleSize(handlePos) * self.handleSize, Vector3.one, Handles.SphereHandleCap);

            self.points[i] = new HermitianSpline.Point{point = newPos, derivative =  newHandlePos - self.points[i].point};
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
                EditorUtility.SetDirty(target);
            }
        }
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
                                self.points[i] = new HermitianSpline.Point{point = new Vector3{x = self.points[i].point.x, y = self.points[i].point.y, z = m_base.floatValue}, derivative = new Vector3{x = self.points[i].derivative.x, y = self.points[i].derivative.y, z = 0f}};
                            }
                            break;
                        case 1 : // XZ
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new HermitianSpline.Point{point = new Vector3{x = self.points[i].point.x, y = m_base.floatValue, z = self.points[i].point.z}, derivative = new Vector3{x = self.points[i].derivative.x, y = 0f, z = self.points[i].derivative.z}};
                            }
                            break;
                        case 2 : // YZ
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new HermitianSpline.Point{point = new Vector3{x = m_base.floatValue, y = self.points[i].point.y, z = self.points[i].point.z}, derivative = new Vector3{x = 0f, y = self.points[i].derivative.y, z = self.points[i].derivative.z}};
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