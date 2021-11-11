using System.Collections.Generic;
using UnityEngine;

public class CatmullRomSpline : Spline
{
    private static Matrix4x4 constantMatrix =
        new Matrix4x4(
            new Vector4(-0.5f, 1.5f, -1.5f, 0.5f),
            new Vector4(1f, -2.5f, 2f, -0.5f),
            new Vector4(-0.5f, 0f, 0.5f, 0f),
            new Vector4(0f, 1f, 0f, 0f)
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
        pointsRst[pointsRst.Length - 1] = GetInterpolation(totalPoint + 2, 1);

        return pointsRst;
    }
}
