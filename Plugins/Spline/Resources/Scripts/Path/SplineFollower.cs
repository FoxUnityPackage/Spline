using UnityEngine;

/// <summary>
/// This script allow gameObject to follow a spline with simple interpolation.
/// Speed is not uniform. Use UniformSplineFollower for uniform speed
/// </summary>
public class SplineFollower : MonoBehaviour
{
    protected float m_t;
    public Spline spline;
    public float speed;
    
    void Update()
    {
        if (spline.IsValid())
        {
            m_t += Time.deltaTime * speed;
            transform.position = spline.GetGlobalInterpolation(Mathf.PingPong(m_t, 1f));
        }
    }
}
