using System.Collections.Generic;
using UnityEngine;

public class BezierPatch : Patch
{
    public List<BezierSpline> curves = new List<BezierSpline>();

    public override Vector3 GetInterpolation(int pointIndexU, int pointIndexV, float s, float t)
    {
        s = Mathf.Clamp01(s);
        t = Mathf.Clamp01(t);

        BezierSpline splineRst = new BezierSpline();
        splineRst.points = new List<Vector3>(4);

        for (int i = pointIndexU; i < 4; i++)
        {
            splineRst.points.Add(curves[i].GetLocalInterpolation(pointIndexV, s));
        }

        return splineRst.GetLocalInterpolation(0, t);
    }

    public override Vector3[][] MakeSplinePoints(int curveDivision)
    {
        int row = (curves.Count - curves.Count % 4) / 4;
        int column = (curves[0].points.Count - curves[0].points.Count % 4) / 4;
        Vector3[][] pointsRst = new Vector3[row * curveDivision + 1][];
        float step = 1f / curveDivision;
        
        for (int i = 0; i < pointsRst.Length; i++)
        {
            pointsRst[i] = new Vector3[column * curveDivision + 1];
        }
        
        for (int i = 0; i < row; i++)
        {
            float t = 0f;
            for (int j = 0; j < curveDivision; j++)
            {
                for (int k = 0; k < column; k++)
                {
                    float s = 0f;
                    for (int l = 0; l < curveDivision; l++)
                    {
                        pointsRst[i * curveDivision + j][k * curveDivision + l] = GetInterpolation(i * 4, k * 4, s, t);
                        s += step;
                    }
                }
                // Inlude the last point
                pointsRst[i * curveDivision + j][pointsRst[i].Length - 1] = GetInterpolation(i * 4, (column - 1) * 4, 1f, t);
                t += step;
            }
        }
        
        // Inlude the last point
        for (int k = 0; k < column; k++)
        {
            float s = 0f;
            for (int l = 0; l < curveDivision; l++)
            {
                pointsRst[pointsRst.Length - 1][k * curveDivision + l] = GetInterpolation((row - 1) * 4, k * 4, s, 1f);
                s += step;
            }
        }

        pointsRst[pointsRst.Length - 1][pointsRst[pointsRst.Length - 1].Length - 1] = GetInterpolation((row - 1) * 4, (column - 1) * 4, 1f, 1f);
        
        return pointsRst;
    }
}