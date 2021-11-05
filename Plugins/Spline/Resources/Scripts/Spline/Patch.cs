using UnityEngine;

public abstract class Patch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }
    
    public abstract Vector3 GetInterpolation(int pointIndexU, int pointIndexV, float s, float t);
    public abstract Vector3[][] MakeSplinePoints(int divisionBySpline);
}
