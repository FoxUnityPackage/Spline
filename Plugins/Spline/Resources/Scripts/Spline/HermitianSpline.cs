using System.Collections.Generic;
using System.IO;
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
    
#if UNITY_EDITOR
    [HideInInspector] public float handleSize = 0.25f;
#endif
    
    
    public override Vector3 GetLocalInterpolation(int pointIndex, float t)
    {
        t = Mathf.Clamp01(t);
        
        Vector4 tVec = new Vector4(t*t*t, t*t, t, 1f);
        Matrix4x4 pointsMatrix = new Matrix4x4(
            points[pointIndex],
            points[pointIndex + 2],
            points[pointIndex + 1],
            points[pointIndex + 3]);

        return pointsMatrix * constantMatrix * tVec;
    }
    
    public override Vector3[] MakeSplinePoints(int divisionBySpline)
    {
        if (points.Count < 4)
            return null;
        
        int totalPoint = (GetMaxIndex() - points.Count % GetIndexStep()) / GetIndexStep();
        Vector3[] pointsRst = new Vector3[divisionBySpline * totalPoint + 1];
        float step = 1f / divisionBySpline;

        for (int i = 0; i < totalPoint; i++)
        {
            float t = 0f;
            for (int j = 0; j < divisionBySpline; j++)
            {
                pointsRst[i * divisionBySpline + j] = GetLocalInterpolation(i * GetIndexStep(), t);
                t += step;
            }
        }
        
        // Include the last point
        pointsRst[pointsRst.Length - 1] = GetLocalInterpolation((totalPoint - 1) * GetIndexStep(), 1f);

        return pointsRst;
    }

    public override Vector3[] MakeLocalSplinePoints(int pointIndex, int divisionBySpline)
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

        return pointsRst;
    }
    
    public override int GetMaxIndex()
    {
        return points.Count - 2;
    }
    
    public override int GetMinIndex()
    {
        return 0;
    }
    
    public override int GetIndexStep()
    {
        return 2;
    }
}
