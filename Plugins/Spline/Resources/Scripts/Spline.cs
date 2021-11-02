using UnityEngine;

public abstract class Spline : MonoBehaviour
{
    private static Matrix4x4 b_splineConstantMatrix =
        new Matrix4x4(
            new Vector4(-1f, 3f, -3f, 1f),
            new Vector4(3f, -6f, 3f, 0f),
            new Vector4(-3f, 0f, 3f, 0f),
            new Vector4(1f, 4f, 1f, 0f)
        );
    
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }
    
    public abstract Vector3 GetInterpolation(int pointIndex, float t);
    public abstract Vector3[] MakeSplinePoints(int divisionBySpline);
}
