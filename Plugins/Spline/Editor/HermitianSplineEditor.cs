using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(HermitianSpline))]
public class HermitianSplineEditor : Editor
{
    // Custom in-scene UI for when Spline script
    // component is selected.
    private float pointSize = 0.1f;
    private float handleSize = 0.25f;
    private int splineDivision = 20;

    protected virtual void OnSceneGUI()
    {
        HermitianSpline self = target as HermitianSpline;

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

            var temp = new HermitianSpline.Point();
            temp.point = newPos;
            temp.derivative = newHandlePos - self.points[i].point;
            self.points[i] = temp;
        }
    }
}