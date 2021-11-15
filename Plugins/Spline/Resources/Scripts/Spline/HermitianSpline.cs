using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public class HermitianSpline : Spline
{
    private static Matrix4x4 constantMatrix =
        new Matrix4x4(
            new Vector4(2f, -2f, 1f, 1f),
            new Vector4(-3f, 3f, -2f, -1f),
            new Vector4(0f, 0f, 1f, 0f),
            new Vector4(1f, 0f, 0f, 0f)
        );
    
    [System.Serializable]
    public struct Point
    {
        public Vector3 point;
        public Vector3 derivative;
    }
    
#if UNITY_EDITOR
    [HideInInspector] public float handleSize = 0.25f;
#endif
    
    public List<Point> points = new List<Point>();
    
    public override Vector3 GetLocalInterpolation(int pointIndex, float t)
    {
        t = Mathf.Clamp01(t);
        
        Vector4 tVec = new Vector4(t*t*t, t*t, t, 1f);
        Matrix4x4 pointsMatrix = new Matrix4x4(
            points[pointIndex].point,
            points[pointIndex + 1].point,
            points[pointIndex].derivative,
            points[pointIndex + 1].derivative);

        return pointsMatrix * constantMatrix * tVec;
    }
    
    public override Vector3[] MakeSplinePoints(int divisionBySpline)
    {
        Vector3[] pointsRst = new Vector3[divisionBySpline * (points.Count - 1) + 1];
        float step = 1f / divisionBySpline;
        
        for (int i = 0; i < points.Count - 1; i++)
        {
            float t = 0f;
            for (int j = 0; j < divisionBySpline; j++)
            {
                pointsRst[i * divisionBySpline + j] = GetLocalInterpolation(i, t);
                t += step;
            }
        }
        
        // Inlude the last point
        pointsRst[pointsRst.Length - 1] = GetLocalInterpolation(points.Count - 2, 1f);

        return pointsRst;
    }

    public override void Save(string dst)
    {
        using (StreamWriter writer = new StreamWriter(dst))
        {
            writer.WriteLine(JsonHelper.ToJson(points.ToArray(), true));
            writer.Close();
        }
    }
    
    public override void Load(string src)
    {
        using (StreamReader reader = new StreamReader(src))
        {
            points = new List<Point>(JsonHelper.FromJson<Point>(reader.ReadToEnd()));
            reader.Close();
        }
    }
    
    public override int GetMaxPassagePointIndex()
    {
        return points.Count - 1;
    }
    
    public override int GetMinPassagePointIndex()
    {
        return 0;
    }
    
    public override int GetPassagePointIndexStep()
    {
        return 1;
    }
}
