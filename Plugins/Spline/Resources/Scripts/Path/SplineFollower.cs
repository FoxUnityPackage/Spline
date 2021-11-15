using UnityEngine;

public class SplineFollower : MonoBehaviour
{
    protected float m_t;
    protected int m_checkPointIndex = 0;
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
