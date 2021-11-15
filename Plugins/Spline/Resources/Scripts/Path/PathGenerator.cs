using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public Spline spline;
    public MeshFilter meshFilter;
    [SerializeField] public Vector3 biTangeante = Vector3.back;
    public float scale = 0.1f;
    public int division = 10;
    
    public void GenerateMesh()
    {
        Debug.Assert(meshFilter != null, "meshFilter != null");
        
        Mesh mesh = new Mesh();
        mesh.name = "GeneratedMesh";
        
        Vector3[] points = spline.MakeSplinePoints(division);
        
        mesh.SetVertices(GenerateVertices(points));
        mesh.SetIndices(GenerateIndices(points), MeshTopology.Triangles, 0);
        mesh.SetUVs(0, GenerateUVs(points));

        meshFilter.mesh = mesh;
    }

    Vector3[] GenerateVertices(Vector3[] points)
    {
        // Create quad for each point
        Vector3[] vertices = new Vector3[4 * (points.Length - 1)];
        
        Vector3 tangeante;
        tangeante = Vector3.Normalize(Vector3.Cross(biTangeante, points[1] - points[0])) * scale;
        vertices[0] = points[0] + tangeante;
        vertices[1] = points[0] - tangeante;
        
        int index = 1;
 
        for (int i = 2; i < vertices.Length - 6; i += 4)
        {
            tangeante = Vector3.Normalize(Vector3.Cross(biTangeante, points[index + 1] - points[index])) * scale;
            vertices[i + 0] = points[index] + tangeante;
            vertices[i + 1] = points[index] - tangeante;
            // Second pass for UVs
            vertices[i + 2] = points[index] + tangeante;
            vertices[i + 3] = points[index] - tangeante;
            index++;
        }
        
        tangeante = Vector3.Normalize(Vector3.Cross(biTangeante, points[index + 1] - points[index])) * scale;
        vertices[vertices.Length - 6] = points[index] + tangeante;
        vertices[vertices.Length - 5] = points[index] - tangeante;

        // Use the last direction
        tangeante = Vector3.Normalize(Vector3.Cross(biTangeante, points[ points.Length - 1] - points[ points.Length - 2])) * scale;
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

    Vector2[] GenerateUVs(Vector3[] points)
    {
        // Create quad for each point
        Vector2[] uvs = new Vector2[4 * (points.Length - 1)];
        
        for (int i = 0; i < uvs.Length; i += 4)
        {
            uvs[i + 0] = new Vector2(0f, 0f);
            uvs[i + 1] = new Vector2(1f, 0f);
            uvs[i + 2] = new Vector2(0f, 1f);
            uvs[i + 3] = new Vector2(1f, 1f);
        }
        return uvs;
    }
}
