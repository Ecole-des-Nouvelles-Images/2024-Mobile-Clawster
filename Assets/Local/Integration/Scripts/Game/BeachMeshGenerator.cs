using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Local.Integration.Scripts.Game
{
    public class BeachMeshGenerator : MonoBehaviour
    {
        [Space(5), Header("Grid")] [SerializeField]
        private CellVisual _prefabsCell;

        public Vector2Int GridSize;
        [SerializeField] private float _maxHeight = 10;
        [SerializeField] private int _seed;

        [Space(5), Header("PerlinNoise")] [SerializeField]
        private bool _doPerlinNoiseInRunTime = true;

        [SerializeField, Range(0, 1)] private float _perlinMaxValue = 1;
        [SerializeField, Range(0, 1)] private float _perlinMinValue = 0f;

        [SerializeField] private Vector2 _perlinNoiseOffset;
        [SerializeField] private float _perlinNoiseScale;
        [SerializeField] private int _octaves = 1;
        [SerializeField] private float _lacunarity = 1f;

        [Space(5), Header("Borders")] [SerializeField]
        private bool _doBorders;

        [SerializeField] private bool _doFading;
        [SerializeField] private Direction _borderDirection;
        [SerializeField] private int _borderSize = 1;
        [SerializeField, Range(0, 1)] private float _borderMaxValue = 1;

        [Space(5), Header("Fade")] [SerializeField]
        private bool _doFade;

        [SerializeField, Range(0, 1)] private float _fadeValue;
        [SerializeField] private AnimationCurve _fadeAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Space(5), Header("Do Thresholds")] [SerializeField]
        private bool _doDoThresholds;

        [SerializeField, Range(0, 1)] private float _thresholdValue = 0.5f;
        
                
        [SerializeField]
        private GameObject[] _objectsToSpawn; 

        [SerializeField]
        private int _objectCount = 50; 

        [SerializeField]
        private float _heightOffset = 0.1f;

        private CellData[,] _grid;
        private MeshFilter _meshFilter;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            CreateRandomMap();
        }

        void Update()
        {
            // if (_doPerlinNoiseInRunTime) DoPerlinNoise();
            // if (_doBorders) DoBorder(_borderDirection);
            // if (_doFade) DoFade();
            // if (_doDoThresholds) DoThreshold();
            // if (_prefabsCell != null) RecolorGrid();
        }

        private void CreateRandomMap()
        {
            _seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            Initialize();
            DoPerlinNoise();
            DoBorder(_borderDirection);
            GenerateMeshFromGrid();
            SpawnObjectsOnBeach();
        }

        [ContextMenu("Process")]
        public void Process()
        {
            Initialize();
            DoPerlinNoise();
            Populate();
        }

        [ContextMenu("Generate Mesh")]
        public void GenerateProceduralMesh()
        {
            GenerateMeshFromGrid();
        }

        private void Initialize()
        {
            _grid = new CellData[GridSize.x, GridSize.y];
            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    _grid[x, y] = new CellData(x, y);
                }
            }
        }

        private void DoPerlinNoise()
        {
            System.Random random = new System.Random(_seed);
            Vector2[] octaveOffsets = new Vector2[_octaves];

            for (int i = 0; i < _octaves; i++)
            {
                octaveOffsets[i] = new Vector2(random.Next(-100000, 100000), random.Next(-100000, 100000));
            }

            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    float noiseValue = 0f;
                    float amplitude = 1f;
                    float frequency = 1f;
                    float maxAmplitude = 0f;

                    for (int i = 0; i < _octaves; i++)
                    {
                        float sampleX = (x * frequency * _perlinNoiseScale) + _perlinNoiseOffset.x + octaveOffsets[i].x;
                        float sampleY = (y * frequency * _perlinNoiseScale) + _perlinNoiseOffset.y + octaveOffsets[i].y;

                        noiseValue += Mathf.PerlinNoise(sampleX, sampleY) * amplitude;

                        maxAmplitude += amplitude;
                        amplitude *= 0.5f;
                        frequency *= _lacunarity;
                    }

                    noiseValue /= maxAmplitude;
                    noiseValue = Mathf.Clamp(noiseValue, 0f, 1f);

                    noiseValue = Mathf.Clamp(noiseValue, _perlinMinValue, 1f);

                    _grid[x, y].height = noiseValue * _perlinMaxValue;
                    _grid[x, y].perlinNoiseheight = _grid[x, y].height;
                }
            }
        }

        private void DoBorder(Direction direction)
        {
            int bufferZone = _borderSize / 2;

            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    float fadeFactor = 0;
                    bool isBorder = false;

                    switch (direction)
                    {
                        case Direction.Top:
                            if (y < _borderSize)
                            {
                                isBorder = true;
                                if (_doFading)
                                {
                                    fadeFactor = CalculateFadeFactor(y, bufferZone, _borderSize);
                                }
                                else
                                {
                                    _grid[x, y].height = _borderMaxValue;
                                }
                            }

                            break;
                        case Direction.Bot:
                            if (y >= GridSize.y - _borderSize)
                            {
                                isBorder = true;
                                if (_doFading)
                                {
                                    fadeFactor = CalculateFadeFactor(GridSize.y - 1 - y, bufferZone, _borderSize);
                                }
                                else
                                {
                                    _grid[x, y].height = _borderMaxValue;
                                }
                            }

                            break;
                        case Direction.Left:
                            if (x < _borderSize)
                            {
                                isBorder = true;
                                if (_doFading)
                                {
                                    fadeFactor = CalculateFadeFactor(x, bufferZone, _borderSize);
                                }
                                else
                                {
                                    _grid[x, y].height = _borderMaxValue;
                                }
                            }

                            break;
                        case Direction.Right:
                            if (x >= GridSize.x - _borderSize)
                            {
                                isBorder = true;
                                if (_doFading)
                                {
                                    fadeFactor = CalculateFadeFactor(GridSize.x - 1 - x, bufferZone, _borderSize);
                                }
                                else
                                {
                                    _grid[x, y].height = _borderMaxValue;
                                }
                            }

                            break;
                    }

                    if (isBorder && _doFading)
                    {
                        _grid[x, y].height = Mathf.Lerp(_borderMaxValue, _grid[x, y].perlinNoiseheight, fadeFactor);
                    }
                }
            }
        }

        private float CalculateFadeFactor(int distance, int bufferZone, int borderSize)
        {
            if (distance < bufferZone)
            {
                return 0;
            }
            else
            {
                return (float)(distance - bufferZone) / (borderSize - bufferZone);
            }
        }

        #region Non used functions
        private void DoFade()
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    float time = (float)y / GridSize.y;
                    _grid[x, y].height = Mathf.Lerp(_grid[x, y].perlinNoiseheight, _fadeValue,
                        _fadeAnimationCurve.Evaluate(time));
                }
            }
        }

        private void DoThreshold()
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    if (_grid[x, y].height > _thresholdValue)
                    {
                        _grid[x, y].height = 1;
                    }
                    else
                    {
                        _grid[x, y].height = 0;
                    }
                }
            }
        }

        private void RecolorGrid()
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    _grid[x, y].visual.SetColorByHeight(_grid[x, y].height);
                }
            }
        }
        #endregion

        private void Populate()
        {
            Vector3 offset = new Vector3(GridSize.x / 2f, 0, GridSize.y / 2f);

            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    Vector3 position = new Vector3(x, 0, y) - offset;

                    CellVisual visual = Instantiate(_prefabsCell, position, quaternion.identity);
                    visual.transform.SetParent(transform);

                    _grid[x, y].visual = visual;
                    visual.SetColorByHeight(_grid[x, y].height);
                }
            }
        }

        private void GenerateMeshFromGrid()
        {
            int width = GridSize.x;
            int height = GridSize.y;

            Vector3 offset = new Vector3(width / 2f, 0, height / 2f);
            Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[width * height * 6];

            for (int y = 0; y <= height; y++)
            {
                for (int x = 0; x <= width; x++)
                {
                    int gridX = Mathf.Clamp(x, 0, width - 1);
                    int gridY = Mathf.Clamp(y, 0, height - 1);

                    float heightValue = _grid[gridX, gridY].height;
                    vertices[y * (width + 1) + x] = new Vector3(x, heightValue * _maxHeight, y) - offset;

                    uvs[y * (width + 1) + x] = new Vector2((float)x / width, (float)y / height);
                }
            }

            int vert = 0;
            int tris = 0;
            for (int y = 0; y < height; y++)
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

            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                uv = uvs
            };
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            _meshFilter.sharedMesh = mesh;

            MeshCollider meshCollider = GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
            }
            meshCollider.sharedMesh = mesh;
        }
        
        private void SpawnObjectsOnBeach()
        {
            if (_meshFilter.sharedMesh == null || _objectsToSpawn == null || _objectsToSpawn.Length == 0)
            {
                Debug.LogWarning("Mesh or objects to spawn not set up correctly.");
                return;
            }

            Mesh mesh = _meshFilter.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            int yRot;

            for (int i = 0; i < _objectCount; i++)
            {
                yRot = Random.Range(0, 360);
                int randomIndex = UnityEngine.Random.Range(0, vertices.Length);
                Vector3 localPosition = vertices[randomIndex];
                Vector3 worldPosition = transform.TransformPoint(localPosition);
                worldPosition.y += _heightOffset;

                Vector3 localNormal = normals[randomIndex];
                Vector3 worldNormal = transform.TransformDirection(localNormal);

                Quaternion normQuat = Quaternion.FromToRotation(Vector3.up, worldNormal);
                Quaternion rotQuat = Quaternion.Euler(0, yRot, 0);

                GameObject objectToSpawn = _objectsToSpawn[UnityEngine.Random.Range(0, _objectsToSpawn.Length)];
                Instantiate(objectToSpawn, worldPosition, normQuat * rotQuat, transform);
            }
        }

    }
}
