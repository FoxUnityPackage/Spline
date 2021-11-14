using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

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
    
    private SplineEditorUtility.ESpace2D m_space2D = SplineEditorUtility.ESpace2D.XY;
    private float m_base = 0f;

    private string m_path;
    
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
            
            self.points[i] = new CatmullRomSpline.Point{point = newPos};
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
                self.points[self.points.Count - 1] = self.points[self.points.Count - 2] = self.points[0] = self.points[1];
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
                                self.points[i] = new CatmullRomSpline.Point{point = new Vector3{x = self.points[i].point.x, y = self.points[i].point.y, z = m_base}};
                            }
                            break;
                        case SplineEditorUtility.ESpace2D.XZ:
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new CatmullRomSpline.Point{point = new Vector3{x = self.points[i].point.x, y = m_base, z = self.points[i].point.z}};
                            }
                            break;
                        case SplineEditorUtility.ESpace2D.YZ:
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new CatmullRomSpline.Point{point = new Vector3{x = m_base, y = self.points[i].point.y, z = self.points[i].point.z}};
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

    public void ImportExportSetting()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button((m_path != null && m_path.Length == 0) ? "Select path" : m_path))
            {
                m_path = EditorUtility.OpenFolderPanel("Import/Export folder", "", "");
                
                // Convert absolute to relative path
                if (m_path.StartsWith(Application.dataPath))
                {
                    m_path=  "Assets" + m_path.Substring(Application.dataPath.Length);
                }
                
                //Create Directory if it does not exist
                if (!Directory.Exists(Path.GetDirectoryName(m_path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(m_path));
                }

            }

            if (GUILayout.Button("Import"))
            {
                Import();
            }

            if (m_path != null && m_path.Length != 0)
            {
                if (GUILayout.Button("Export"))
                {
                    Export();
                }
            }
        } GUILayout.EndHorizontal();
    }

    public void Import()
    {
        self.Load(EditorUtility.OpenFilePanel("Import/Export folder", "", "dat"));
        EditorUtility.SetDirty(target);
    }

    public void Export()
    {
        string dst;

        int id = -1;
        //Looking for a new default file name
        do
        {
            dst = Path.Combine(m_path, (++id == 0 ? "spline" : $"spline{id}") + ".dat");
        } while (File.Exists(dst));
        
        self.Save(dst);
    }
}