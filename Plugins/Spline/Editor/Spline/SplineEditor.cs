using System.IO;
using UnityEditor;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

public class SplineEditor<T> : Editor
    where T : Spline
{
    private string m_path;

    protected T self = null;
    
    private void OnEnable()
    {
        self = target as T;
    }
    
    protected void ImportExportSetting()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.Label("Path : " + ((m_path != null && m_path.Length == 0) ? "None" : m_path));
            
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Import"))
                {
                    m_path = EditorUtility.OpenFilePanel("Import/Export folder", "", "dat");
                    self.Load(m_path);
                    EditorUtility.SetDirty(target);
                }
                
                if (GUILayout.Button("Save as"))
                {
                    m_path = EditorUtility.SaveFilePanelInProject("Save as", "Spline", "dat", "");
                    
                    // Convert absolute to relative path
                    if (m_path.StartsWith(Application.dataPath))
                    {
                        m_path = "Assets" + m_path.Substring(Application.dataPath.Length);
                    }
                
                    //Create Directory if it does not exist
                    if (!Directory.Exists(Path.GetDirectoryName(m_path)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(m_path));
                    }
                    
                    self.Save(m_path);
                    AssetDatabase.Refresh();
                }
                
                if (m_path != null && m_path.Length != 0)
                {
                    if (GUILayout.Button("Save"))
                    {
                        self.Save(m_path);
                        AssetDatabase.Refresh();
                    }
                }
                
            } GUILayout.EndHorizontal();
        } GUILayout.EndVertical();
    }
}
