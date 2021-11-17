using UnityEngine;

public class UniformSplineFollower : MonoBehaviour
{
    protected int m_splineIndex;
    protected float m_t;
    protected float m_splineDistance;
    protected float m_currentDistance;

    public Spline spline;
    [Tooltip("In m/s")] public float speed;

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
                ProcessNextSplinePath();

            if (spline.IsIndexValid(m_splineIndex))
            {
                transform.position = spline.GetLocalInterpolation(m_splineIndex, m_t);
            }
            else
            {
                enabled = false;
            }
        }
    }

    private void Setup()
    {
        m_splineIndex = spline.GetMinPassagePointIndex();
        m_t = 0f;
        m_currentDistance = 0f;
        m_splineDistance = spline.GetLocalDistance(m_splineIndex, spline.splineDivision);

        isInit = true;
    }

    void ProcessNextSplinePath()
    {
        m_splineIndex += spline.GetPassagePointIndexStep();
        if (!spline.IsIndexValid(m_splineIndex))
        {
            Setup();
        }
        else
        {
            m_currentDistance -= m_splineDistance;
            m_splineDistance = spline.GetLocalDistance(m_splineIndex, spline.splineDivision);
            m_t = m_currentDistance / m_splineDistance;
        }
    }
}
