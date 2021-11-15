using UnityEditor;
using UnityEngine;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(PathGenerator))]
public class PathGeneratorEditor : Editor
{
    // Custom in-scene UI for when Spline script
    // component is selected.
    private PathGenerator self = null;

    private void OnEnable()
    {
        self = target as PathGenerator;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate mesh"))
            self.GenerateMesh();
    }
}