using UnityEngine;

[ExecuteAlways] // Permet d'exécuter ce script même en mode Édition
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BeachGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int width = 50; 
    public int depth = 50; 
    public float scale = 5f; 
    public float maxHeight = 2f; 

    private Mesh mesh;

    void OnValidate()
    {
        GenerateMesh();
        ApplyMesh();
    }
    
    void ApplyMesh()
    {
        if (mesh != null)
        {
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }
    }

    void GenerateMesh()
    {
        mesh = new Mesh();

        Vector3[] vertices = new Vector3[(width + 1) * (depth + 1)];
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = 0;
                float normalizedX = (float)x / width;
                float normalizedZ = (float)z / depth;
                y = Mathf.PerlinNoise(normalizedX * scale, normalizedZ * scale) * maxHeight;
                vertices[z * (width + 1) + x] = new Vector3(x, y, z);
            }
        }

        int[] triangles = new int[width * depth * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[tris] = vert;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
