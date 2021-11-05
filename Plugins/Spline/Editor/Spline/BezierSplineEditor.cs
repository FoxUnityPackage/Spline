using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(BezierSpline))]
public class BezierSplineEditor : Editor
{
    // Custom in-scene UI for when Spline script
    // component is selected.
    private float pointSize = 0.1f;
    private int splineDivision = 20;
    private bool isExtremityAdd = false;
    private bool isContinues = true;
    private BezierSpline self = null;
    
    
    private void OnEnable()
    {
        self = target as BezierSpline;
    }

    protected virtual void OnSceneGUI()
    {
        if (!self.enabled)
            return;
        
        if (self.points.Count > 3)
            Handles.DrawAAPolyLine(self.MakeSplinePoints(splineDivision));
        
        for (int i = 0; i < self.points.Count; i++)
        {
            bool isVelocityHandle = isContinues && i > 4 && i % 4 - 1 == 0;
            bool isDirectionHandle = isContinues && i + 3 < self.points.Count && i % 4 - 2 == 0;
            
            Handles.color = isVelocityHandle ?  Color.cyan : isDirectionHandle ? Color.blue : Color.green;
            Vector3 newPos = Handles.FreeMoveHandle( self.points[i].point, Quaternion.identity,
                HandleUtility.GetHandleSize( self.points[i].point) * pointSize, Vector3.one, Handles.SphereHandleCap);

            if (isVelocityHandle)
            {
                Vector3 dir = Vector3.Normalize(self.points[i - 2].point - self.points[i - 3].point);
                float velocity = Vector3.Magnitude(newPos - self.points[i - 1].point);
                self.points[i] = new BezierSpline.Point
                    {point = self.points[i - 1].point + dir * velocity};
            }
            else
                self.points[i] = new BezierSpline.Point{point = newPos};

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

        if (self.points.Count > 3)
        {
            EditorGUI.BeginChangeCheck();
            isExtremityAdd = EditorGUILayout.Toggle("Include extremity", isExtremityAdd);
            if (EditorGUI.EndChangeCheck())
            {
                if (isExtremityAdd)
                {
                    self.points.Insert(0, self.points.First());
                    self.points.Add(self.points.Last());
                }
                else
                {
                    self.points.RemoveAt(0);
                    self.points.RemoveAt(self.points.Count - 1);
                }
                EditorUtility.SetDirty(target);
            }

            
            // Options available only for 2 portions curve
            if (self.points.Count > 7)
            {
                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Link portion"))
                {
                    for (int i = 3; i < self.points.Count - 1; i += 4)
                    {
                        self.points[i + 1] = self.points[i];
                    }

                    EditorUtility.SetDirty(target);
                }
                
                EditorGUI.BeginChangeCheck();
                isContinues = EditorGUILayout.Toggle("Is continuity smooth", isContinues);
            }
        }
    }
}