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
        splineRst.points = new List<BezierSpline.Point>(4);

        for (int i = pointIndexU; i < 4; i++)
        {
            splineRst.points.Add(new BezierSpline.Point {point = curves[i].GetInterpolation(pointIndexV, s)});
        }

        return splineRst.GetInterpolation(0, t);
    }

    public override Vector3[][] MakeSplinePoints(int curveDivision)
    {
        int row = (curves.Count - curves.Count % 4) / 4;
        int column = (curves[0].points.Count - curves[0].points.Count % 4) / 4;
        Vector3[][] pointsRst = new Vector3[row * curveDivision][];
        float step = 1f / (curveDivision - 1);
        
        for (int i = 0; i < pointsRst.Length; i++)
        {
            pointsRst[i] = new Vector3[column * curveDivision];
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
                t += step;
            }
        }

        return pointsRst;
    }
}