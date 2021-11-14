using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class B_Spline : Spline
{
    private static Matrix4x4 constantMatrix =
        new Matrix4x4(
            new Vector4(-1f, 3f, -3f, 1f) / 6f,
            new Vector4(3f, -6f, 3f, 0f) / 6f,
            new Vector4(-3f, 0f, 3f, 0f) / 6f,
            new Vector4(1f, 4f, 1f, 0f) / 6f
        );

    [System.Serializable]
    public struct Point
    {
        public Vector3 point;
    }
    
    public List<Point> points = new List<Point>();
    
    public override Vector3 GetInterpolation(int pointIndex, float t)
    {
        t = Mathf.Clamp01(t);
        
        Vector4 tVec = new Vector4(t*t*t, t*t, t, 1f);
        Matrix4x4 pointsMatrix = new Matrix4x4(
            points[pointIndex - 3].point,
            points[pointIndex - 2].point,
            points[pointIndex - 1].point,
            points[pointIndex].point);

        return pointsMatrix * constantMatrix * tVec;
    }
    
    public override Vector3[] MakeSplinePoints(int divisionBySpline)
    {
        int totalPoint = points.Count - 3;   
        Vector3[] pointsRst = new Vector3[divisionBySpline * totalPoint + 1];
        float step = 1f / divisionBySpline;
        
        for (int i = 0; i < totalPoint; i++)
        {
            float t = 0f;
            for (int j = 0; j < divisionBySpline; j++)
            {
                pointsRst[i * divisionBySpline + j] = GetInterpolation(i + 3, t);
                t += step;
            }
        }
        
        // Inlude the last point
        pointsRst[pointsRst.Length - 1] = GetInterpolation(totalPoint + 2, 1f);

        return pointsRst;
    }
    
    public override void Save(string dst)
    {
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(dst, true);
        writer.WriteLine(JsonUtility.ToJson(points));
        writer.Close();
    }
    
    public override void Load(string src)
    {
        //Write some text to the test.txt file
        StreamReader reader = new StreamReader(src, true);
        points = JsonUtility.FromJson<List<Point>>(reader.ReadToEnd());
        reader.Close();
    }
}
