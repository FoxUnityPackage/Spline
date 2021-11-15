using System;
using UnityEditor;
using UnityEngine;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(HermitianSpline))]
public class HermitianSplineEditor : SplineEditor<HermitianSpline>
{
    // Custom in-scene UI for when Spline script
    // component is selected.
    private float pointSize = 0.1f;
    private float handleSize = 0.25f;
    private int splineDivision = 20;
    
    private SplineEditorUtility.ESpace2D m_space2D = SplineEditorUtility.ESpace2D.XY;
    private float m_base = 0f;
    
    protected virtual void OnSceneGUI()
    {
        if (!self.enabled)
            return;
        
        Handles.DrawAAPolyLine(self.MakeSplinePoints(splineDivision));
        
        for (int i = 0; i < self.points.Count; i++)
        {
            Handles.color = Color.green;
            Vector3 newPos = Handles.FreeMoveHandle( self.points[i].point, Quaternion.identity,
                HandleUtility.GetHandleSize( self.points[i].point) * pointSize, Vector3.one, Handles.SphereHandleCap);
            
            Handles.color = Color.cyan;
            Handles.DrawLine(self.points[i].point, self.points[i].point + self.points[i].derivative);


            Vector3 handlePos = self.points[i].point + self.points[i].derivative;
            
            Vector3 newHandlePos = Handles.FreeMoveHandle(handlePos, Quaternion.identity,
                HandleUtility.GetHandleSize(handlePos) * handleSize, Vector3.one, Handles.SphereHandleCap);

            self.points[i] = new HermitianSpline.Point{point = newPos, derivative =  newHandlePos - self.points[i].point};
        }
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SplineEditorUtility.DrawUILine(Color.gray);
        GUILayout.Label("Editor settings :");
        pointSize = EditorGUILayout.FloatField("Point size", pointSize);
        EditorGUI.BeginChangeCheck();
        splineDivision = EditorGUILayout.IntField("Curve precision", splineDivision);
        if (EditorGUI.EndChangeCheck())
        {
            splineDivision = Mathf.Clamp(splineDivision, 3, 1000);
            EditorUtility.SetDirty(target);
        }
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        CloseShapeSetting();
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        Space2DSetting();
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        ImportExportSetting();
    }
    
    void CloseShapeSetting()
    {
        if (self.points.Count > 3)
        {
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Close shape"))
            {
                self.points[self.points.Count - 1] = self.points[0];
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
                    switch (m_space2D)
                    {
                        case SplineEditorUtility.ESpace2D.XY:
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new HermitianSpline.Point{point = new Vector3{x = self.points[i].point.x, y = self.points[i].point.y, z = m_base}, derivative = new Vector3{x = self.points[i].derivative.x, y = self.points[i].derivative.y, z = 0f}.normalized};
                            }
                            break;
                        case SplineEditorUtility.ESpace2D.XZ:
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new HermitianSpline.Point{point = new Vector3{x = self.points[i].point.x, y = m_base, z = self.points[i].point.z}, derivative = new Vector3{x = self.points[i].derivative.x, y = 0f, z = self.points[i].derivative.z}.normalized};
                            }
                            break;
                        case SplineEditorUtility.ESpace2D.YZ:
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new HermitianSpline.Point{point = new Vector3{x = m_base, y = self.points[i].point.y, z = self.points[i].point.z}, derivative = new Vector3{x = 0f, y = self.points[i].derivative.y, z = self.points[i].derivative.z}.normalized};
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
                m_space2D = (SplineEditorUtility.ESpace2D) EditorGUILayout.EnumPopup(m_space2D);
            } GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(itemWidth));
            {
                GUILayout.Label("Base");
                m_base = EditorGUILayout.FloatField(m_base);
            } GUILayout.EndVertical();
            
        } GUILayout.EndHorizontal();
    }
}