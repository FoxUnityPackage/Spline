using System.IO;
using UnityEditor;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

public class SplineEditor<T> : Editor
    where T : Spline
{
    protected T self = null;
    
    private void OnEnable()
    {
        self = target as T;
    }
    
    protected void ImportExportSetting()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.Label("Path : " + ((self.m_path != null && self.m_path.Length == 0) ? "None" : self.m_path));
            
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Import"))
                {
                    self.m_path = EditorUtility.OpenFilePanel("Import/Export folder", "", "json");
                    self.Load(self.m_path);
                    EditorUtility.SetDirty(target);
                }
                
                if (GUILayout.Button("Save as"))
                {
                    self.m_path = EditorUtility.SaveFilePanelInProject("Save as", "Spline", "json", "");
                    
                    // Convert absolute to relative path
                    if (self.m_path.StartsWith(Application.dataPath))
                    {
                        self.m_path = "Assets" + self.m_path.Substring(Application.dataPath.Length);
                    }
                
                    //Create Directory if it does not exist
                    if (!Directory.Exists(Path.GetDirectoryName(self.m_path)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(self.m_path));
                    }
                    
                    self.Save(self.m_path);
                    AssetDatabase.Refresh();
                }
                
                if (self.m_path != null && self.m_path.Length != 0)
                {
                    if (GUILayout.Button("Save"))
                    {
                        self.Save(self.m_path);
                        AssetDatabase.Refresh();
                    }
                }
                
            } GUILayout.EndHorizontal();
        } GUILayout.EndVertical();
    }
}
