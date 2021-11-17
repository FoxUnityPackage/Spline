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
}