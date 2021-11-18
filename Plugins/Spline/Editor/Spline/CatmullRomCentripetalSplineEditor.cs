using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(CatmullRomCentripetalSpline))]
public class CatmullRomCentripetalSplineEditor : SplineEditor<CatmullRomCentripetalSpline>
{
    protected virtual void OnSceneGUI()
    {
        if (!self.enabled)
            return;
        
        if (self.points.Count > 3)
            Handles.DrawAAPolyLine(self.MakeSplinePoints(m_splineDivision.intValue));
        
        for (int i = 0; i < self.points.Count; i++)
        {
            Handles.color = Color.green;
            Vector3 newPos = Handles.FreeMoveHandle( self.points[i].point, Quaternion.identity,
                HandleUtility.GetHandleSize( self.points[i].point) * m_pointSize.floatValue, Vector3.one, Handles.SphereHandleCap);
            
            self.points[i] = new CatmullRomCentripetalSpline.Point{point = newPos};
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Portion control");
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Add"))
            {
                CatmullRomCentripetalSpline.Point pt1 = self.points[self.points.Count - 1];
                CatmullRomCentripetalSpline.Point pt2 = self.points[self.points.Count - 2];
                CatmullRomCentripetalSpline.Point pt3 = self.points[self.points.Count - 3];
                CatmullRomCentripetalSpline.Point pt4 = self.points[self.points.Count - 4];
                Vector3 dir = pt1.point - pt4.point;
                
                self.points.Add(pt1);
                self.points.Add(new CatmullRomCentripetalSpline.Point{ point = pt3.point + dir});
                self.points.Add(new CatmullRomCentripetalSpline.Point{ point = pt2.point + dir});
                self.points.Add(new CatmullRomCentripetalSpline.Point{ point = pt1.point + dir});
                
                EditorUtility.SetDirty(target);
            }
            
            if (GUILayout.Button("Remove last"))
            {
                self.points.RemoveAt(self.points.Count - 1);
                self.points.RemoveAt(self.points.Count - 1);
                self.points.RemoveAt(self.points.Count - 1);
                self.points.RemoveAt(self.points.Count - 1);
                EditorUtility.SetDirty(target);
            }
            
        } EditorGUILayout.EndHorizontal();

        SplineEditorUtility.DrawUILine(Color.gray);
        GUILayout.Label("Editor settings :");
        m_pointSize.floatValue = EditorGUILayout.FloatField("Point size", m_pointSize.floatValue);
        EditorGUI.BeginChangeCheck();
        m_splineDivision.intValue = EditorGUILayout.IntField("Curve precision", m_splineDivision.intValue);
        if (EditorGUI.EndChangeCheck())
        {
            m_splineDivision.intValue = Mathf.Clamp(m_splineDivision.intValue, 3, 1000);
            EditorUtility.SetDirty(target);
        }

        if (self.points.Count > 3)
        {
            EditorGUI.BeginChangeCheck();
            self.isExtremityAdd = EditorGUILayout.Toggle("Include extremity", self.isExtremityAdd);
            if (EditorGUI.EndChangeCheck())
            {
                if (self.isExtremityAdd)
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

            
            // Options available only for 2 portions curve
            if (self.points.Count > 7)
            {
                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Link portion"))
                {
                    for (int i = 3; i < self.points.Count - 1; i += 4)
                    {
                        self.points[i + 1] = self.points[i];
                    }

                    EditorUtility.SetDirty(target);
                }
            }
        }
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        CloseShapeSetting();
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        Space2DSetting();
        
        SplineEditorUtility.DrawUILine(Color.gray, 1, 5);
        ImportExportSetting();
        
        serializedObject.ApplyModifiedProperties();
    }

    void CloseShapeSetting()
    {
        if (self.points.Count > 3)
        {
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Close shape"))
            {
                self.points[self.points.Count - 1] = self.points[0];
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
                    switch (m_space2D.enumValueIndex)
                    {
                        case 0 : // XY
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new CatmullRomCentripetalSpline.Point{point = new Vector3{x = self.points[i].point.x, y = self.points[i].point.y, z = m_base.floatValue}};
                            }
                            break;
                        case 1 : // XZ
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new CatmullRomCentripetalSpline.Point{point = new Vector3{x = self.points[i].point.x, y = m_base.floatValue, z = self.points[i].point.z}};
                            }
                            break;
                        case 2 : // YZ
                            for (int i = 0; i < self.points.Count; i++)
                            {
                                self.points[i] = new CatmullRomCentripetalSpline.Point{point = new Vector3{x = m_base.floatValue, y = self.points[i].point.y, z = self.points[i].point.z}};
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