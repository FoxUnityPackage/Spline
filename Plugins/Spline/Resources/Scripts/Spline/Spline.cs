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
    
    [SerializeField, HideInInspector] private int m_splineDivision = 20;
    [SerializeField, HideInInspector] private float m_pointSize = 0.1f;
    [SerializeField, HideInInspector] private ESpace2D m_space2D = ESpace2D.XY;
    [SerializeField, HideInInspector] private float m_base = 0f;
    [SerializeField, HideInInspector] private string m_path;
#endif

    public abstract Vector3 GetLocalInterpolation(int pointIndex, float t);
    
    public Vector3 GetGlobalInterpolation(float t)
    {
        float interval = (GetMaxIndex() - GetMinIndex()) / GetIndexStep() * t;
        int index = ((int)(interval) + GetMinIndex()) * GetIndexStep();
        float localT = interval % 1f;
        
        return GetLocalInterpolation(index, localT);
    }

    public abstract Vector3[] MakeSplinePoints(int divisionBySpline);
    public abstract Vector3[] MakeLocalSplinePoints(int pointIndex, int divisionBySpline);

    public abstract void Save(string dst);

    public abstract void Load(string src);

    public abstract int GetMaxIndex();
    
    public abstract int GetMinIndex();
    
    public abstract int GetIndexStep();
    public bool IsIndexValid(int index)
    {
        return index < GetMaxIndex() && index >= GetMinIndex();
    }
    
    public bool IsValid()
    {
        return GetMaxIndex() > GetMinIndex();
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
        for (int i = GetMinIndex(); i < GetMaxIndex(); i+= GetIndexStep())
        {
            rst += GetLocalDistance(i, divisionBySpline);
        }
        return rst;
    }
}
