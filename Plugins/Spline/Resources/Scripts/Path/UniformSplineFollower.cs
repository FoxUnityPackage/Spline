using UnityEngine;

/// <summary>
/// This script allow gameObject to follow a spline with a uniform speed
/// </summary>
public class UniformSplineFollower : MonoBehaviour
{
    protected int m_splineIndex;
    public float m_t;
    protected float m_splineDistance;
    protected float m_currentDistance;
    protected Vector3[] m_checkPoints;
    protected int m_currentCheckPointIndex =- 0;

    [Min(2), Tooltip("the division of each spline. More this value is, more the spline have precision")]
    public int prescision;
    public Spline spline;
    [Min(0f), Tooltip("In m/s")] public float speed;

    private bool isInit = false;

    void Update()
    {
        if (spline.IsValid())
        {
            if (!isInit)
                Setup();

            m_currentDistance += Time.deltaTime * speed;
            m_t = m_currentDistance / m_splineDistance;

            while (m_t >= 1f)
                ProcessNextCheckPoint();

            if (spline.IsIndexValid(m_splineIndex))
            {
                transform.position = Vector3.Lerp(m_checkPoints[m_currentCheckPointIndex], m_checkPoints[m_currentCheckPointIndex + 1], m_t);
            }
            else
            {
                enabled = false;
            }
        }
    }

    private void Setup()
    {
        m_splineIndex = spline.GetMinIndex();
        m_currentDistance = 0f;
        m_checkPoints = spline.MakeLocalSplinePoints(m_splineIndex, prescision, true);
        m_currentCheckPointIndex = 0;
        m_splineDistance = (m_checkPoints[1] - m_checkPoints[0]).magnitude;
        m_t = 0f;
        
        isInit = true;
    }

    void ProcessNextCheckPoint()
    {
        ++m_currentCheckPointIndex;
        if (m_currentCheckPointIndex > m_checkPoints.Length - 2) //Do not considerate the last point
        {
            ProcessNextSplinePath();
        }
        else
        {
            m_currentDistance -= m_splineDistance;
            m_splineDistance = (m_checkPoints[m_currentCheckPointIndex + 1] - m_checkPoints[m_currentCheckPointIndex]).magnitude;
            m_t = m_currentDistance / m_splineDistance;
        }
    }

    void ProcessNextSplinePath()
    {
        m_currentCheckPointIndex = 0;
        m_splineIndex += spline.GetIndexStep();
        if (!spline.IsIndexValid(m_splineIndex))
        {
            Setup();
        }
        else
        {
            m_checkPoints = spline.MakeLocalSplinePoints(m_splineIndex, prescision, true);
            m_currentDistance -= m_splineDistance;
            m_splineDistance = (m_checkPoints[1] - m_checkPoints[0]).magnitude;
            m_t = m_currentDistance / m_splineDistance;
        }
    }
}
