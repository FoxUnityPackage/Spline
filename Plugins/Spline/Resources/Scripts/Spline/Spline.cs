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
    [HideInInspector] public int splineDivision = 20;
    
    [HideInInspector] public ESpace2D m_space2D = ESpace2D.XY;
    [HideInInspector] public float m_base = 0f;
    [HideInInspector] public string m_path;
#endif
    
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }
    
    public abstract Vector3 GetLocalInterpolation(int pointIndex, float t);
    
    public Vector3 GetGlobalInterpolation(float t)
    {
        float interval = (GetMaxPassagePointIndex() - GetMinPassagePointIndex()) / GetPassagePointIndexStep() * t;
        int index = ((int)(interval) + GetMinPassagePointIndex()) * GetPassagePointIndexStep();
        float localT = interval % 1f;
        
        return GetLocalInterpolation(index, localT);
    }

    public abstract Vector3[] MakeSplinePoints(int divisionBySpline);

    public abstract void Save(string dst);

    public abstract void Load(string src);

    public abstract int GetMaxPassagePointIndex();
    
    public abstract int GetMinPassagePointIndex();
    
    public abstract int GetPassagePointIndexStep();
}
