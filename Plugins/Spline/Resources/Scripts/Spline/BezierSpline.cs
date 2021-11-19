using UnityEngine;

public class BezierSpline : Spline
{
    private static Matrix4x4 constantMatrix =
        new Matrix4x4(
            new Vector4(-1f, 3f, -3f, 1f),
            new Vector4(3f, -6f, 3f, 0f),
            new Vector4(-3f, 3f, 0f, 0f),
            new Vector4(1f, 0f, 0f, 0f)
        );
    
#if UNITY_EDITOR
    [HideInInspector] public bool isExtremityAdd = false;
    [HideInInspector] public bool isContinues = true;
#endif
    public override Vector3 GetLocalInterpolation(int pointIndex, float t)
    {
        t = Mathf.Clamp01(t);

        Vector4 tVec = new Vector4(t*t*t, t*t, t, 1f);
        Matrix4x4 pointsMatrix = new Matrix4x4(
            points[pointIndex],
            points[pointIndex + 1],
            points[pointIndex + 2],
            points[pointIndex + 3]);

        return pointsMatrix * constantMatrix * tVec;
    }

    public override Vector3[] MakeSplinePoints(int divisionBySpline)
    {
        if (points.Count < 3)
            return null;
        
        int totalPoint = (points.Count - points.Count % 4) / 4;
        Vector3[] pointsRst = new Vector3[divisionBySpline * totalPoint + 1];
        float step = 1f / divisionBySpline;

        for (int i = 0; i < totalPoint; i++)
        {
            float t = 0f;
            for (int j = 0; j < divisionBySpline; j++)
            {
                pointsRst[i * divisionBySpline + j] = GetLocalInterpolation(i * 4, t);
                t += step;
            }
        }
        
        // Include the last point
        pointsRst[pointsRst.Length - 1] = GetLocalInterpolation((totalPoint - 1) * 4, 1f);

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
        return points.Count;
    }
    
    public override int GetMinIndex()
    {
        return 0;
    }
    
    public override int GetIndexStep()
    {
        return 4;
    }
}