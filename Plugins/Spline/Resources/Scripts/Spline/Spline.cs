using UnityEngine;

public abstract class Spline : MonoBehaviour
{
#if UNITY_EDITOR
    [System.Serializable]
    public enum ESpace2D
    {
        XY, 
        XZ,
        YZ
    };
    
    [HideInInspector] public float pointSize = 0.1f;
    
    [HideInInspector] public ESpace2D m_space2D = ESpace2D.XY;
    [HideInInspector] public float m_base = 0f;
    [HideInInspector] public string m_path;
#endif
    [HideInInspector] public int splineDivision = 20;

    public abstract Vector3 GetLocalInterpolation(int pointIndex, float t);
    
    public Vector3 GetGlobalInterpolation(float t)
    {
        float interval = (GetMaxPassagePointIndex() - GetMinPassagePointIndex()) / GetPassagePointIndexStep() * t;
        int index = ((int)(interval) + GetMinPassagePointIndex()) * GetPassagePointIndexStep();
        float localT = interval % 1f;
        
        return GetLocalInterpolation(index, localT);
    }

    public abstract Vector3[] MakeSplinePoints(int divisionBySpline);
    public abstract Vector3[] MakeLocalSplinePoints(int pointIndex, int divisionBySpline);

    public abstract void Save(string dst);

    public abstract void Load(string src);

    public abstract int GetMaxPassagePointIndex();
    
    public abstract int GetMinPassagePointIndex();
    
    public abstract int GetPassagePointIndexStep();
    public bool IsIndexValid(int index)
    {
        return index < GetMaxPassagePointIndex() && index >= GetMinPassagePointIndex();
    }
    
    public bool IsValid()
    {
        return GetMaxPassagePointIndex() > GetMinPassagePointIndex();
    }


    public float GetLocalDistance(int pointIndex, int divisionBySpline)
    {
        Vector3[] points = MakeLocalSplinePoints(pointIndex, divisionBySpline);
        float rst = 0;
        if (points != null && points.Length > 1)
        {
            for (int i = 1; i < points.Length; i++)
            {
                rst += (points[i] - points[i - 1]).magnitude;
            }
        }
        
        return rst;
    }

    public float GetGlobalDistance(int divisionBySpline)
    {
        float rst = 0;
        for (int i = GetMinPassagePointIndex(); i < GetMaxPassagePointIndex(); i+= GetPassagePointIndexStep())
        {
            rst += GetLocalDistance(i, divisionBySpline);
        }
        return rst;
    }
}
