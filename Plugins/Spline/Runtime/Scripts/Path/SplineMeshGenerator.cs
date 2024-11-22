using UnityEngine;

public class SplineMeshGenerator : MonoBehaviour
{
    public Spline Spline;
    public MeshFilter MeshFilter;
    [SerializeField] public Vector3 BiTangeante = Vector3.back;
    public float Scale = 0.1f;
    [Min(3)] public int Division = 3;

    public void GenerateMesh()
    {
        Debug.Assert(MeshFilter != null, "meshFilter != null");
        Vector3[] points = Spline.MakeSplinePoints(Division);
        
        if (points != null)
        {
            Mesh mesh = MeshFilter.mesh;
            mesh.SetVertices(GenerateVertices(points));
            mesh.SetIndices(GenerateIndices(points), MeshTopology.Triangles, 0);
            mesh.SetUVs(0, GenerateUVs(points));
        }
    }

    Vector3[] GenerateVertices(Vector3[] points)
    {
        // Create quad for each point
        Vector3[] vertices = new Vector3[4 * (points.Length - 1)];
        
        Vector3 tangeante;
        tangeante = Vector3.Normalize(Vector3.Cross(BiTangeante, points[1] - points[0])) * Scale;
        vertices[0] = points[0] + tangeante;
        vertices[1] = points[0] - tangeante;
        
        int index = 1;
 
        for (int i = 2; i < vertices.Length - 6; i += 4)
        {
            tangeante = Vector3.Normalize(Vector3.Cross(BiTangeante, points[index + 1] - points[index])) * Scale;
            vertices[i + 0] = points[index] + tangeante;
            vertices[i + 1] = points[index] - tangeante;
            // Second pass for UVs
            vertices[i + 2] = points[index] + tangeante;
            vertices[i + 3] = points[index] - tangeante;
            index++;
        }
        
        tangeante = Vector3.Normalize(Vector3.Cross(BiTangeante, points[index + 1] - points[index])) * Scale;
        vertices[vertices.Length - 6] = points[index] + tangeante;
        vertices[vertices.Length - 5] = points[index] - tangeante;

        // Use the last direction
        tangeante = Vector3.Normalize(Vector3.Cross(BiTangeante, points[ points.Length - 1] - points[ points.Length - 2])) * Scale;
        vertices[vertices.Length - 4] = points[index] + tangeante;
        vertices[vertices.Length - 3] = points[index] - tangeante;
        // Second pass for UVs
        vertices[vertices.Length - 2] = points[ points.Length - 1] + tangeante;
        vertices[vertices.Length - 1] = points[ points.Length - 1] - tangeante;
        
        return vertices;
    }
    
    int[] GenerateIndices(Vector3[] points)
    {
        // Create quad for each point
        int[] indices = new int[6 * (points.Length - 1)];

        int index = 0;
        for (int i = 0; i < indices.Length; i+=6)
        {
            indices[i + 0] = index;
            indices[i + 1] = index + 1;
            indices[i + 2] = index + 2;
            indices[i + 3] = index + 2;
            indices[i + 4] = index + 1;
            indices[i + 5] = index + 3;
            index += 4;
        }
        return indices;
    }

    Vector2[] GenerateUVs(Vector3[] points, bool uvPerSection = false)
    {
        // Create quad for each point
        Vector2[] uvs = new Vector2[4 * (points.Length - 1)];

        if (uvPerSection)
        {
            for (int i = 0; i < uvs.Length; i += 4)
            {
                uvs[i + 0] = new Vector2(0f, 0f);
                uvs[i + 1] = new Vector2(1f, 0f);
                uvs[i + 2] = new Vector2(0f, 1f);
                uvs[i + 3] = new Vector2(1f, 1f);
            }
        }
        else
        {
            int sectionCount = 0;
            for (int i = 0; i < uvs.Length; i += 4)
            {
                uvs[i + 0] = new Vector2(0f, sectionCount / (float)points.Length);
                uvs[i + 1] = new Vector2(1f, sectionCount / (float)points.Length);
                uvs[i + 2] = new Vector2(0f,  (sectionCount + 1) / (float)points.Length);
                uvs[i + 3] = new Vector2(1f, (sectionCount + 1) / (float)points.Length);
                sectionCount++;
            }
        }

        return uvs;
    }
}
