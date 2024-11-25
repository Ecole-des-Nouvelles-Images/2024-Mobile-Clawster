using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Local.Noah.Scripts
{
    public enum Direction
    {
        Top,
        Bot,
        Left,
        Right,
        All
    }

    public class BeachMeshGenerator : MonoBehaviour
    {
        [Space(5), Header("Grid")]
        [SerializeField]  private CellVisual _prefabsCell;
        public Vector2Int GridSize;
        [SerializeField] private float _maxHeight = 10;
    
        [Space(5), Header("PerlinNoise")]
        [SerializeField] private bool _doPerlinNoiseInRunTime = true;
        [SerializeField] private Vector2 _perlinNoiseOffset;
        [SerializeField] private float _perlinNoiseScale;

        [Space(5), Header("Borders")] 
        [SerializeField] private bool _doBorders;
        [SerializeField] private Direction _borderDirection;
        [SerializeField] private int _borderSize = 1;
        [SerializeField, Range(0, 1)] private float _borderMaxValue;
        [SerializeField] private bool _doFading;
        [SerializeField] private AnimationCurve _fadingCurve = AnimationCurve.EaseInOut(0,0,1,1);
        
        [Space(5), Header("Fade")]
        [SerializeField] private bool _doFade;
        [SerializeField, Range(0, 1)] private float _fadeValue;
        [SerializeField] private AnimationCurve _fadeAnimationCurve = AnimationCurve.EaseInOut(0,0,1,1);
        
        [Space(5), Header("Do Thresholds")]
        [SerializeField] private bool _doDoThresholds;
        [SerializeField, Range(0, 1)] private float _thresholdValue = 0.5f;
        
        
        private CellData[,] _grid;
        private MeshFilter _meshFilter;

        private void Awake() {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            Initialize();
        }

        void Update() {
            if (_doPerlinNoiseInRunTime) {
                DoPerlinNoise();
            }

            if (_doBorders) {
                DoBorder(_borderDirection);
            }
            if(_doFade)DoFade();
        
            if(_doDoThresholds)DoThreshold();
        
            RecolorGrid();
        }

        [ContextMenu("Process")]
        public void Process() {
            Initialize();
            DoPerlinNoise();
            Populate();
        }
        
        [ContextMenu("Generate Mesh")]
        public void GenerateProceduralMesh() {
            GenerateMeshFromGrid();
        }
        
        private void Initialize() {
            _grid = new CellData[GridSize.x, GridSize.y];
            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    _grid[x, y] = new CellData(x, y);
                }
            }
        }

        private void DoPerlinNoise() {
            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    _grid[x, y] .height = Mathf.PerlinNoise(
                        x*_perlinNoiseScale +_perlinNoiseOffset.x,
                        y*_perlinNoiseScale +_perlinNoiseOffset.y);
                    _grid[x, y].perlinNoiseheight = _grid[x, y].height;
                }
            }
        }
    
        private void DoBorder(Direction direction) {
            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    switch (direction)
                    {
                        case Direction.Top:
                            if (y < _borderSize) { 
                                float fadeFactor = (float)y / _borderSize;
                                _grid[x, y].height = Mathf.Lerp(_grid[x, y].perlinNoiseheight, _borderMaxValue, _fadingCurve.Evaluate(fadeFactor)); 
                            }
                            break;
                        case Direction.Bot:
                            if (y > GridSize.y - _borderSize - 1) {
                                _grid[x, y].height = _borderMaxValue;
                            }
                            break;

                        case Direction.Left:
                            if (x < _borderSize) {
                                _grid[x, y].height = _borderMaxValue;
                            }
                            break;

                        case Direction.Right:
                            if (x > GridSize.x - _borderSize - 1) {
                                _grid[x, y].height = _borderMaxValue;
                            }
                            break;

                        case Direction.All:
                            if ((x < _borderSize || x > GridSize.x - _borderSize - 1) || (y < _borderSize || y > GridSize.y - _borderSize - 1)) {
                                _grid[x, y].height = _borderMaxValue;
                            }
                            break;
                    }
                }
            }
        }
        
        private void DoFade() {
            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    float time = (float)y / GridSize.y;
                    _grid[x, y].height = Mathf.Lerp(_grid[x, y].perlinNoiseheight, _fadeValue, _fadeAnimationCurve.Evaluate(time)); 
                }
            }
        }
    
        private void DoThreshold() {
            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    if (_grid[x, y].height > _thresholdValue) {
                        _grid[x, y].height = 1;
                    }
                    else {
                        _grid[x, y].height = 0;
                    }
                }
            }
        }
    
        private void RecolorGrid() {
            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    _grid[x, y].visual.SetColorByHeight(_grid[x, y].height);
                }
            }
        }
    

        private void Populate() {
            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    CellVisual visual =Instantiate(_prefabsCell, new Vector3(x, 0, y), quaternion.identity);
                    visual.transform.SetParent(transform);
                
                    _grid[x, y].visual = visual;
                    visual.SetColorByHeight(_grid[x, y].height);
                }
            }
        }
        
        
        private void GenerateMeshFromGrid() {
            // Vérifie si la height map est générée
            if (_grid == null || _grid.Length == 0) {
                Debug.LogError("Height map non générée. Assurez-vous d'appeler PerlinNoise1() avant de générer le mesh.");
                return;
            }

            int width = GridSize.x;
            int height = GridSize.y;

            // Crée un tableau de sommets et de triangles
            Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
            int[] triangles = new int[width * height * 6];

            // Remplit le tableau des sommets
            for (int y = 0; y <= height; y++) {
                for (int x = 0; x <= width; x++) {
                    float heightValue = _grid[x % width, y % height].height; // Hauteur depuis la height map
                    vertices[y * (width + 1) + x] = new Vector3(x, heightValue * _maxHeight, y); // 10f : facteur de hauteur
                }
            }

            // Remplit le tableau des triangles
            int vert = 0;
            int tris = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
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

            // Crée un mesh et assigne les sommets et triangles
            Mesh mesh = new Mesh {
                vertices = vertices,
                triangles = triangles
            };
            mesh.RecalculateNormals(); // Recalcule les normales pour l'éclairage

            // Applique le mesh au MeshFilter
            _meshFilter.sharedMesh = mesh;
        }
    }
}