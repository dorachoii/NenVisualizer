using UnityEngine;

public class HighResPlaneGenerator : MonoBehaviour
{
    [Header("Plane Settings")]
    public int segmentsX = 50;  // 가로 세그먼트 수
    public int segmentsZ = 50;  // 세로 세그먼트 수
    public float width = 10f;   // 가로 크기
    public float height = 10f;  // 세로 크기
    
    void Start()
    {
        GenerateHighResPlane();
    }
    
    void GenerateHighResPlane()
    {
        Mesh mesh = new Mesh();
        mesh.name = "HighResPlane";
        
        // Vertices 생성
        Vector3[] vertices = new Vector3[(segmentsX + 1) * (segmentsZ + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        Vector3[] normals = new Vector3[vertices.Length];
        
        for (int z = 0; z <= segmentsZ; z++)
        {
            for (int x = 0; x <= segmentsX; x++)
            {
                int index = z * (segmentsX + 1) + x;
                
                float xPos = (float)x / segmentsX * width - width * 0.5f;
                float zPos = (float)z / segmentsZ * height - height * 0.5f;
                
                vertices[index] = new Vector3(xPos, 0, zPos);
                uvs[index] = new Vector2((float)x / segmentsX, (float)z / segmentsZ);
                normals[index] = Vector3.up;
            }
        }
        
        // Triangles 생성
        int[] triangles = new int[segmentsX * segmentsZ * 6];
        int triIndex = 0;
        
        for (int z = 0; z < segmentsZ; z++)
        {
            for (int x = 0; x < segmentsX; x++)
            {
                int vertexIndex = z * (segmentsX + 1) + x;
                
                // 첫 번째 삼각형
                triangles[triIndex] = vertexIndex;
                triangles[triIndex + 1] = vertexIndex + segmentsX + 1;
                triangles[triIndex + 2] = vertexIndex + 1;
                
                // 두 번째 삼각형
                triangles[triIndex + 3] = vertexIndex + 1;
                triangles[triIndex + 4] = vertexIndex + segmentsX + 1;
                triangles[triIndex + 5] = vertexIndex + segmentsX + 2;
                
                triIndex += 6;
            }
        }
        
        // Mesh에 데이터 할당
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.triangles = triangles;
        
        // Mesh Filter에 할당
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
            
        meshFilter.mesh = mesh;
        
        // Mesh Renderer 추가 (없다면)
        if (GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();
    }
}
