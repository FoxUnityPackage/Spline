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
    private HermitianSpline self = null;

    private void OnEnable()
    {
        self = target as HermitianSpline;
    }

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
    }
}