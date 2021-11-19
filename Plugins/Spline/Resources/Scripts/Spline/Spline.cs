using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Base class of spline with lot of feature to define and interact with it.
/// Spline is a special function defined piecewise by polynomials
/// </summary>
public abstract class Spline : MonoBehaviour
{
    #region Editor 
    // Variable for editor only useful for the editor. Don't use it, for editor only !
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
    #endregion Editor

    [Tooltip("The list of point that composed the spline. For compatibility, point can be a derivative of a tangent depending on the type of spline")]
    public List<Vector3> points = new List<Vector3>();
    
    /// <summary>
    /// Get the point based on t in a specific spline indicate by the index
    /// </summary>
    /// <param name="pointIndex">This index need to be valid using the function IsIndexValid and correspondent to a specific spline in the while curve
    /// You can iterate on it thank to loop like that :
    /// <code>
    /// for (int i = GetMinIndex(); i &#60; GetMaxIndex(); i+= GetIndexStep()){[...]}
    /// </code>
    /// </param>
    /// <param name="t"> between 0 and 1. This value is clamped internally</param>
    /// <returns></returns>
    public abstract Vector3 GetLocalInterpolation(int pointIndex, float t);
    
    /// <summary>
    /// Get the point based on t in the whole curve
    /// </summary>
    /// <param name="t"> between 0 and 1. This value is clamped internally</param>
    /// <returns></returns>
    public Vector3 GetGlobalInterpolation(float t)
    {
        float interval = (GetMaxIndex() - GetMinIndex()) / GetIndexStep() * t;
        int index = ((int)(interval) + GetMinIndex()) * GetIndexStep();
        float localT = interval % 1f;
        
        return GetLocalInterpolation(index, localT);
    }

    /// <summary>
    /// Get array of point based on the whole curve
    /// </summary>
    /// <param name="divisionBySpline"> the division of each spline. More this value is, more the spline have precision</param>
    /// <returns></returns>
    public abstract Vector3[] MakeSplinePoints(int divisionBySpline);
    
    /// <summary>
    /// Get array of point based on a specific spline
    /// </summary>
    /// <param name="pointIndex">This index need to be valid using the function IsIndexValid and correspondent to a specific spline in the while curve
    /// You can iterate on it thank to loop like that :
    /// <code>
    /// for (int i = GetMinIndex(); i &#60; GetMaxIndex(); i+= GetIndexStep()){[...]}
    /// </code>
    /// </param>
    /// <param name="t"> between 0 and 1. This value is clamped internally</param>
    /// <returns></returns>
    public abstract Vector3[] MakeLocalSplinePoints(int pointIndex, int divisionBySpline, bool addLastValue = false);
    
    /// <summary>
    /// Get the max spline index. Useful to iterate on curves
    /// </summary>
    /// <returns></returns>
    public abstract int GetMaxIndex();
    
    /// <summary>
    /// Get the min spline index. Useful to iterate on curves
    /// </summary>
    /// <returns></returns>
    public abstract int GetMinIndex();
    
    /// <summary>
    /// Get the spline step index. Useful to iterate on curves
    /// </summary>
    /// <returns></returns>
    public abstract int GetIndexStep();
    
    /// <summary>
    /// Return true if the index correspond to a existing spline in the curve
    /// </summary>
    /// <param name="index">the index of the spline</param>
    /// <returns></returns>
    public bool IsIndexValid(int index)
    {
        return index < GetMaxIndex() && index >= GetMinIndex();
    }
    
    /// <summary>
    /// Return true if the curve contain a minimum number of point to represent a spline
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        return GetMaxIndex() > GetMinIndex();
    }

    /// <summary>
    /// The the distance of a the spline in meter (according to the unity distance convention) for a specific spline
    /// </summary>
    /// <param name="pointIndex">This index need to be valid using the function IsIndexValid and correspondent to a specific spline in the while curve
    /// You can iterate on it thank to loop like that :
    /// <code>
    /// for (int i = GetMinIndex(); i &#60; GetMaxIndex(); i+= GetIndexStep()){[...]}
    /// </code>
    /// </param>
    /// <param name="divisionBySpline"> the division of each spline. More this value is, more the spline have precision</param>
    /// <returns></returns>
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

    /// <summary>
    /// The the distance of a the spline in meter (according to the unity distance convention) for the whole spline
    /// </summary>
    /// <param name="pointIndex">This index need to be valid using the function IsIndexValid and correspondent to a specific spline in the while curve
    /// You can iterate on it thank to loop like that :
    /// <code>
    /// for (int i = GetMinIndex(); i &#60; GetMaxIndex(); i+= GetIndexStep()){[...]}
    /// </code>
    /// </param>
    /// <param name="divisionBySpline"> the division of each spline. More this value is, more the spline have precision</param>
    /// <returns></returns>
    public float GetGlobalDistance(int divisionBySpline)
    {
        float rst = 0;
        for (int i = GetMinIndex(); i < GetMaxIndex(); i+= GetIndexStep())
        {
            rst += GetLocalDistance(i, divisionBySpline);
        }
        return rst;
    }
    
    /// <summary>
    /// Serialize the curve to json in a specific path
    /// </summary>
    /// <param name="dst">The path of the destination file. This path is not checked</param>
    public void Save(string dst)
    {
        using (StreamWriter writer = new StreamWriter(dst))
        {
            writer.WriteLine(JsonHelper.ToJson(points.ToArray(), true));
            writer.Close();
        }
    }
    
    /// <summary>
    /// Import the curve from json in a specific path
    /// </summary>
    /// <param name="dst">The path of the source file. This path is checked</param>
    public void Load(string src)
    {
        if (File.Exists(src))
        {
            using (StreamReader reader = new StreamReader(src))
            {
                points = new List<Vector3>(JsonHelper.FromJson<Vector3>(reader.ReadToEnd()));
                reader.Close();
            }
        }
    }
}
