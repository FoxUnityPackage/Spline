using System.IO;
using UnityEngine;

public abstract class Spline : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }
    
    public abstract Vector3 GetInterpolation(int pointIndex, float t);
    public abstract Vector3[] MakeSplinePoints(int divisionBySpline);

    public abstract void Save(string dst);

    public abstract void Load(string src);
}
