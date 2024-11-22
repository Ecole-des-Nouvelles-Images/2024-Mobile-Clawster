using System;
using UnityEngine;

namespace Local.Noah.Scripts
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class BeachGenerator : MonoBehaviour
    {
        [Header("Terrain Settings")]
        [SerializeField] private int _width = 50; 
        [SerializeField] private int _depth = 50; 
        [SerializeField] private float _scale = 5f; 
        [SerializeField] private float _maxHeight = 2f;
        [SerializeField] private int _seed; 
        [SerializeField] private int _octaves = 4;
        [SerializeField] private float _lacunarity = 2f;
        private Mesh _mesh;

        
        private void OnValidate()
        {
            GenerateMesh();
            ApplyMesh();
        }
        
        [ContextMenu("Generate Beach")]
        private void GenerateBeach()
        {
            GenerateMesh();
            ApplyMesh();
        }

        private void ApplyMesh()
        {
            if (_mesh != null)
            {
                GetComponent<MeshFilter>().sharedMesh = _mesh;
            }
        }

        private void GenerateMesh()
        {
            _mesh = new Mesh();

            float[,] noiseMap = Noise.GenerateNoiseMap(
                _width + 1, 
                _depth + 1, 
                _seed, 
                _scale, 
                _octaves, 
                _lacunarity
            );

            Vector3[] vertices = new Vector3[(_width + 1) * (_depth + 1)];
            for (int z = 0; z <= _depth; z++)
            {
                for (int x = 0; x <= _width; x++)
                {
                    float y = noiseMap[x, z] * _maxHeight;
                    vertices[z * (_width + 1) + x] = new Vector3(x, y, z);
                }
            }

            int[] triangles = new int[_width * _depth * 6];
            int vert = 0;
            int tris = 0;

            for (int z = 0; z < _depth; z++)
            {
                for (int x = 0; x < _width; x++)
                {
                    triangles[tris] = vert;
                    triangles[tris + 1] = vert + _width + 1;
                    triangles[tris + 2] = vert + 1;

                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + _width + 1;
                    triangles[tris + 5] = vert + _width + 2;

                    vert++;
                    tris += 6;
                }
                vert++;
            }

            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();
        }
    }
}
