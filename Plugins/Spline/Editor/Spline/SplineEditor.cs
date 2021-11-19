using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

public class SplineEditor<T> : Editor
    where T : Spline
{
    protected T self = null;
    
    protected SerializedProperty m_splineDivision;
    protected SerializedProperty m_pointSize;
    protected SerializedProperty m_space2D;
    protected SerializedProperty m_base;
    protected SerializedProperty m_path;
    
    private void OnEnable()
    {
        self = target as T;
        
        m_splineDivision = serializedObject.FindProperty("m_splineDivision");
        m_pointSize = serializedObject.FindProperty("m_pointSize");
        m_space2D = serializedObject.FindProperty("m_space2D");
        m_base = serializedObject.FindProperty("m_base");
        m_path = serializedObject.FindProperty("m_path");
    }
    
    protected void ImportExportSetting()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.Label("Path : " + ((m_path.stringValue != null && m_path.stringValue.Length == 0) ? "None" : m_path.stringValue));
            
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Import"))
                {
                    m_path.stringValue = EditorUtility.OpenFilePanel("Import/Export folder", "", "json");
                    self.Load(m_path.stringValue);
                    EditorUtility.SetDirty(target);
                }
                
                if (GUILayout.Button("Save as"))
                {
                    m_path.stringValue = EditorUtility.SaveFilePanelInProject("Save as", "Spline", "json", "");
                    
                    // Convert absolute to relative path
                    if (m_path.stringValue.StartsWith(Application.dataPath))
                    {
                        m_path.stringValue = "Assets" + m_path.stringValue.Substring(Application.dataPath.Length);
                    }
                
                    //Create Directory if it does not exist
                    if (!Directory.Exists(Path.GetDirectoryName(m_path.stringValue)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(m_path.stringValue));
                    }
                    
                    self.Save(m_path.stringValue);
                    AssetDatabase.Refresh();
                }
                
                if (m_path.stringValue != null && m_path.stringValue.Length != 0)
                {
                    if (GUILayout.Button("Save"))
                    {
                        self.Save(m_path.stringValue);
                        AssetDatabase.Refresh();
                    }
                }
                
            } GUILayout.EndHorizontal();
        } GUILayout.EndVertical();
    }
    
    protected void Space2DSetting()
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
                                self.points[i] = new Vector3{x = self.points[i].x, y = self.points[i].y, z = m_base.floatValue};
                            }
                            break;
                        case 1 : // XZ
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new Vector3{x = self.points[i].x, y = m_base.floatValue, z = self.points[i].z};
                            }
                            break;
                        case 2 : // YZ
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new Vector3{x = m_base.floatValue, y = self.points[i].y, z = self.points[i].z};
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