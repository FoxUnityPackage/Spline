using System;
using UnityEngine;

[Serializable]
public enum ESplineType
{
    CatmullRom
};

[RequireComponent(typeof(PathGenerator))]
public class PathEditor : MonoBehaviour
{
    public GameObject cursorOverview;
    public GameObject cursorSelection;
    public GameObject pathOwner;
    public ESplineType splineType;
    protected CatmullRomSpline m_editedSpline;
    protected PathGenerator generator;
    
    void Start()
    {
        cursorOverview = Instantiate(cursorOverview, transform);
        cursorSelection = Instantiate(cursorSelection, transform);
        
        cursorOverview.SetActive(false);
        cursorSelection.SetActive(false);

        generator = GetComponent<PathGenerator>();
    }
 
    void Update()
    {
        RaycastHit hit;
        Debug.Assert(Camera.main != null, "Camera.main != null");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                cursorSelection.SetActive(true);
                cursorOverview.SetActive(false);
                cursorSelection.transform.position = hit.point - ray.direction;
                cursorSelection.transform.rotation = Quaternion.LookRotation(hit.normal);

                if (m_editedSpline == null)
                {
                    switch (splineType)
                    {
                        case ESplineType.CatmullRom:
                            m_editedSpline = pathOwner.GetComponent<CatmullRomSpline>();
                            if (m_editedSpline == null)
                                m_editedSpline = pathOwner.AddComponent<CatmullRomSpline>();
                            // Duplicate first point to make sur that spline pass in the first control point
                            m_editedSpline.points.Add(new CatmullRomSpline.Point {point = hit.point});
                            m_editedSpline.points.Add(new CatmullRomSpline.Point {point = hit.point});
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                m_editedSpline.points[m_editedSpline.points.Count - 1] = new CatmullRomSpline.Point {point = hit.point};
                m_editedSpline.points.Add(new CatmullRomSpline.Point {point = hit.point});
                generator.spline = m_editedSpline;
                generator.GenerateMesh();
            }
            else
            {
                cursorSelection.SetActive(false);
                cursorOverview.SetActive(true);
                cursorOverview.transform.position = hit.point - ray.direction;
                cursorOverview.transform.rotation = Quaternion.LookRotation(hit.normal);
            }
        }
        else
        {
            cursorOverview.SetActive(false);
        }
    }
}
