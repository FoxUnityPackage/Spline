using System;
using UnityEngine;

[Serializable]
public class CatmullRomSpline : Spline
{
    private static Matrix4x4 constantMatrix =
        new Matrix4x4(
            new Vector4(-0.5f, 1.5f, -1.5f, 0.5f),
            new Vector4(1f, -2.5f, 2f, -0.5f),
            new Vector4(-0.5f, 0f, 0.5f, 0f),
            new Vector4(0f, 1f, 0f, 0f)
        );
    
    
#if UNITY_EDITOR
    [HideInInspector] public bool isExtremityAdd = false;
#endif

    public override Vector3 GetLocalInterpolation(int pointIndex, float t)
    {
        t = Mathf.Clamp01(t);
        
        Vector4 tVec = new Vector4(t*t*t, t*t, t, 1f);
        Matrix4x4 pointsMatrix = new Matrix4x4(
            points[pointIndex - 3],
            points[pointIndex - 2],
            points[pointIndex - 1],
            points[pointIndex]);

        return pointsMatrix * constantMatrix * tVec;
    }
    
    public override Vector3[] MakeSplinePoints(int divisionBySpline)
    {
        if (points.Count < 4)
            return null;
        
        int totalPoint = points.Count - 3;
        Vector3[] pointsRst = new Vector3[divisionBySpline * totalPoint + 1];
        float step = 1f / divisionBySpline;
        
        for (int i = 0; i < totalPoint; i++)
        {
            float t = 0f;
            for (int j = 0; j < divisionBySpline; j++)
            {
                pointsRst[i * divisionBySpline + j] = GetLocalInterpolation(i + 3, t);
                t += step;
            }
        }
        
        // Include the last point
        if (totalPoint > 0)
            pointsRst[pointsRst.Length - 1] = GetLocalInterpolation(totalPoint + 2, 1);

        return pointsRst;
    }

    public override Vector3[] MakeLocalSplinePoints(int pointIndex, int divisionBySpline, bool addLastPoint = false)
    {
        if (!IsIndexValid(pointIndex))
            return null;
        
        Vector3[] pointsRst = new Vector3[divisionBySpline + 1];
        float step = 1f / divisionBySpline;

        float t = 0f;
        for (int j = 0; j < pointsRst.Length; j++)
        {
            pointsRst[j] = GetLocalInterpolation(pointIndex, t);
            t += step;
        }
        
        // Include the last point
        if (addLastPoint)
            pointsRst[pointsRst.Length - 1] = GetLocalInterpolation(pointIndex, 1);

        return pointsRst;
    }

    public override int GetMaxIndex()
    {
        return points.Count;
    }
    
    public override int GetMinIndex()
    {
        return 3;
    }
    
    public override int GetIndexStep()
    {
        return 1;
    }
}
