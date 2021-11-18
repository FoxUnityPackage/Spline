using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class CatmullRomCentripetalSpline : Spline
{
#if UNITY_EDITOR
    [HideInInspector] public bool isExtremityAdd = false;
#endif
    
    public float alpha;
    
    [Serializable]
    public struct Point
    {
        [SerializeField]
        public Vector3 point;
    }
    
    public List<Point> points = new List<Point>();

    public override Vector3 GetLocalInterpolation(int pointIndex, float t)
    {
        // calculate knots
        const float k0 = 0;
        float k1 = GetKnotInterval( points[pointIndex].point, points[pointIndex + 1].point);
        float k2 = GetKnotInterval( points[pointIndex + 1].point, points[pointIndex + 2].point) + k1;
        float k3 = GetKnotInterval( points[pointIndex + 2].point, points[pointIndex + 3].point ) + k2;

        // evaluate the point
        float u = Mathf.LerpUnclamped( k1, k2, t );
        Vector3 A1 = Remap( k0, k1, points[pointIndex].point, points[pointIndex + 1].point, u );
        Vector3 A2 = Remap( k1, k2, points[pointIndex + 1].point, points[pointIndex + 2].point, u );
        Vector3 A3 = Remap( k2, k3, points[pointIndex + 2].point, points[pointIndex + 3].point, u );
        Vector3 B1 = Remap( k0, k2, A1, A2, u );
        Vector3 B2 = Remap( k1, k3, A2, A3, u );
        return Remap( k1, k2, B1, B2, u );
    }
    
    static Vector3 Remap( float a, float b, Vector3 c, Vector3 d, float u )
    {
        return Vector3.LerpUnclamped( c, d, ( u - a ) / ( b - a ) );
    }

    float GetKnotInterval(Vector3 a, Vector3 b)
    {
        return Mathf.Pow(Vector3.SqrMagnitude(a - b), 0.5f * alpha);
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
                pointsRst[i * divisionBySpline + j] = GetLocalInterpolation(i, t);
                t += step;
            }
        }
        
        // Inlude the last point
        pointsRst[pointsRst.Length - 1] = GetLocalInterpolation(totalPoint - 1, 1f);

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
    
    public override int GetMaxIndex()
    {
        return points.Count - 3;
    }
    
    public override int GetMinIndex()
    {
        return 0;
    }
    
    public override int GetIndexStep()
    {
        return 1;
    }
}
