using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(CatmullRomSpline))]
public class CatmullRomSplineEditor : Editor
{
    // Custom in-scene UI for when Spline script
    // component is selected.
    private float pointSize = 0.1f;
    private int splineDivision = 20;
    private bool isExtremityAdd = false;
    private CatmullRomSpline self = null;
    
    
    private void OnEnable()
    {
        self = target as CatmullRomSpline;
    }

    protected virtual void OnSceneGUI()
    {
        if (!self.enabled)
            return;
        
        if (self.points.Count > 3)
            Handles.DrawAAPolyLine(self.MakeSplinePoints(splineDivision));
        
        for (int i = 0; i < self.points.Count; i++)
        {
            Handles.color = Color.green;
            Vector3 newPos = Handles.FreeMoveHandle( self.points[i].point, Quaternion.identity,
                HandleUtility.GetHandleSize( self.points[i].point) * pointSize, Vector3.one, Handles.SphereHandleCap);
            
            var temp = new CatmullRomSpline.Point();
            temp.point = newPos;
            self.points[i] = temp;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawUILine(Color.gray);

        if (self.points.Count > 3)
        {
            bool previousValue = isExtremityAdd;
            isExtremityAdd = GUILayout.Toggle(isExtremityAdd, "Include extremity");

            if (previousValue != isExtremityAdd)
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
        }

    }
    
    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y +=padding / 2f;
        EditorGUI.DrawRect(r, color);
    }

}