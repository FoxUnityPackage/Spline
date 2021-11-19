using System.Linq;
using UnityEditor;
using UnityEngine;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(BezierPatch))]
public class BezierPatchEditor : Editor
{
    // Custom in-scene UI for when Spline script
    // component is selected.
    private float pointSize = 0.1f;
    private int splineDivision = 10;
    private bool isExtremityAdd = false;
    private bool isContinues = true;
    private BezierPatch self = null;

    private void OnEnable()
    {
        self = target as BezierPatch;
    }

    protected virtual void OnSceneGUI()
    {
        if (!self.enabled)
            return;

        if (self.curves.Count > 3)
        {
            Vector3[][] patch = self.MakeSplinePoints(splineDivision);
            for (int i = 0; i < patch.Length; i++)
            {
                Handles.DrawAAPolyLine(patch[i]);
            }
            
            Vector3[] column = new Vector3[patch.Length];
            for (int i = 0; i < patch.Length; i++)
            {
                for (int j = 0; j < patch[i].Length; j++)
                {
                    for (int k = 0; k < patch.Length; k++)
                    {
                        column[k] = patch[k][j];
                    }
                    Handles.DrawAAPolyLine(column);
                }
            }
        }

        foreach (var curve in self.curves)
        {
            for (int i = 0; i < curve.points.Count; i++)
            {
                bool isVelocityHandle = isContinues && i > 4 && i % 4 - 1 == 0;
                bool isDirectionHandle = isContinues && i + 3 < curve.points.Count && i % 4 - 2 == 0;

                Handles.color = isVelocityHandle ? Color.cyan : isDirectionHandle ? Color.blue : Color.green;
                Vector3 newPos = Handles.FreeMoveHandle(curve.points[i], Quaternion.identity,
                    HandleUtility.GetHandleSize(curve.points[i]) * pointSize, Vector3.one,
                    Handles.SphereHandleCap);

                if (isVelocityHandle)
                {
                    Vector3 dir = Vector3.Normalize(curve.points[i - 2] - curve.points[i - 3]);
                    float velocity = Vector3.Magnitude(newPos - curve.points[i - 1]);
                    curve.points[i] = curve.points[i - 1] + dir * velocity;
                }
                else
                    curve.points[i] = newPos;
            }
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

        if (self.curves.Count > 3)
        {
            EditorGUI.BeginChangeCheck();
            isExtremityAdd = EditorGUILayout.Toggle("Include extremity", isExtremityAdd);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var curve in self.curves)
                {
                    if (isExtremityAdd)
                    {
                        curve.points.Insert(0, curve.points.First());
                        curve.points.Add(curve.points.Last());
                    }
                    else
                    {
                        curve.points.RemoveAt(0);
                        curve.points.RemoveAt(curve.points.Count - 1);
                    }
                }

                EditorUtility.SetDirty(target);
            }

            
            // Options available only for 2 portions curve
            if (self.curves.Count > 7)
            {
                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Link portion"))
                {
                    foreach (var curve in self.curves)
                    {
                        for (int i = 3; i < curve.points.Count - 1; i += 4)
                        {
                            curve.points[i + 1] = curve.points[i];
                        }
                    }

                    EditorUtility.SetDirty(target);
                }
                
                EditorGUI.BeginChangeCheck();
                isContinues = EditorGUILayout.Toggle("Is continuity smooth", isContinues);
            }
        }
    }
}