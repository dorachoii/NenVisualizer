using UnityEngine;

public class MeshTessellator : MonoBehaviour
{
    [Header("Tessellation Settings")]
    public int subdivisionLevel = 3;  // 분할 레벨 (높을수록 더 조밀)
    
    void Start()
    {
        TessellateMesh();
    }
    
    void TessellateMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }
        
        Mesh originalMesh = meshFilter.mesh;
        Mesh tessellatedMesh = SubdivideMesh(originalMesh, subdivisionLevel);
        
        meshFilter.mesh = tessellatedMesh;
    }
    
    Mesh SubdivideMesh(Mesh mesh, int levels)
    {
        Mesh result = mesh;
        
        for (int i = 0; i < levels; i++)
        {
            result = SubdivideOnce(result);
        }
        
        return result;
    }
    
    Mesh SubdivideOnce(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = mesh.uv;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
        
        // 새로운 vertex 배열 생성 (각 삼각형이 4개로 분할됨)
        Vector3[] newVertices = new Vector3[vertices.Length + triangles.Length / 3];
        Vector2[] newUVs = new Vector2[newVertices.Length];
        Vector3[] newNormals = new Vector3[newVertices.Length];
        
        // 기존 vertex 복사
        for (int i = 0; i < vertices.Length; i++)
        {
            newVertices[i] = vertices[i];
            newUVs[i] = uvs[i];
            newNormals[i] = normals[i];
        }
        
        // 새로운 삼각형 배열 생성
        int[] newTriangles = new int[triangles.Length * 4];
        
        int newVertexIndex = vertices.Length;
        int newTriangleIndex = 0;
        
        // 각 삼각형을 4개로 분할
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v1 = triangles[i];
            int v2 = triangles[i + 1];
            int v3 = triangles[i + 2];
            
            // 삼각형의 중점 계산
            Vector3 midPoint = (vertices[v1] + vertices[v2] + vertices[v3]) / 3f;
            Vector2 midUV = (uvs[v1] + uvs[v2] + uvs[v3]) / 3f;
            Vector3 midNormal = (normals[v1] + normals[v2] + normals[v3]) / 3f;
            
            // 새로운 중점 vertex 추가
            newVertices[newVertexIndex] = midPoint;
            newUVs[newVertexIndex] = midUV;
            newNormals[newVertexIndex] = midNormal.normalized;
            
            // 4개의 새로운 삼각형 생성
            // 삼각형 1: v1, v2, mid
            newTriangles[newTriangleIndex] = v1;
            newTriangles[newTriangleIndex + 1] = v2;
            newTriangles[newTriangleIndex + 2] = newVertexIndex;
            
            // 삼각형 2: v2, v3, mid
            newTriangles[newTriangleIndex + 3] = v2;
            newTriangles[newTriangleIndex + 4] = v3;
            newTriangles[newTriangleIndex + 5] = newVertexIndex;
            
            // 삼각형 3: v3, v1, mid
            newTriangles[newTriangleIndex + 6] = v3;
            newTriangles[newTriangleIndex + 7] = v1;
            newTriangles[newTriangleIndex + 8] = newVertexIndex;
            
            // 삼각형 4: v1, mid, v1 (원래 삼각형 유지)
            newTriangles[newTriangleIndex + 9] = v1;
            newTriangles[newTriangleIndex + 10] = newVertexIndex;
            newTriangles[newTriangleIndex + 11] = v1;
            
            newVertexIndex++;
            newTriangleIndex += 12;
        }
        
        // 새로운 mesh 생성
        Mesh newMesh = new Mesh();
        newMesh.vertices = newVertices;
        newMesh.uv = newUVs;
        newMesh.normals = newNormals;
        newMesh.triangles = newTriangles;
        
        return newMesh;
    }
}
