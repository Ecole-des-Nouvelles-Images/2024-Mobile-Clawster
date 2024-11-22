using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class DemoGridManager : MonoBehaviour
{
    
    [FormerlySerializedAs("gridSize")] public Vector2Int GridSize;
    private CellData[,] grid;
    
    [Space(5), Header("PerlinNoize")]
    [SerializeField] private bool _doPerlinNoiseInRunTime = true;
    [SerializeField] private Vector2 _perlinNoiseOffset;
    [SerializeField] private float _perlinNoiseScale;

    [Space(5), Header("Borders")] 
    [SerializeField] private bool _doBorders;
    [SerializeField] private int _borderSize = 1;
    [SerializeField, Range(0, 1)] private float _borderValue;
    
    [Space(5), Header("Fade")]
    [SerializeField] private bool _doFade;
    [SerializeField, Range(0, 1)] private float _fadeValue;
    [SerializeField] private AnimationCurve _fadeAnimationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    public CellVisual PrefabsCell;
    [Space(5), Header("Do Thresholds")]
    [SerializeField] private bool _doDoThresholds;
    [SerializeField, Range(0, 1)] private float _treasholdValue = 0.5f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Intalize();
        PerlinNoise1();
        Populate();
    }

    // Update is called once per frame
    void Update() {
        if (_doPerlinNoiseInRunTime) {
            PerlinNoise1();
        }

        if (_doBorders) {
            DoBorder();
        }
        if(_doFade)DoFade();
        
        if(_doDoThresholds)DoThreshold();
        
        RecolorGrid();
    }

    [ContextMenu("Process")]
    public void Process() {
        Intalize();
        PerlinNoise1();
        Populate();
    }

    
    [ContextMenu("Clear")]
    public void Clear() {
        foreach (var cell in grid) {
            Destroy(cell.visual);
        }
    }

    private void Intalize() {
        grid = new CellData[GridSize.x, GridSize.y];
        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                grid[x, y] = new CellData(x, y);
            }
        }
    }

    private void PerlinNoise1() {
        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                grid[x, y] .height = Mathf.PerlinNoise(
                    x*_perlinNoiseScale +_perlinNoiseOffset.x,
                    y*_perlinNoiseScale +_perlinNoiseOffset.y);
                grid[x, y].perlinNoiseheight = grid[x, y].height;
            }
        }
    }
    
    private void DoBorder() {
        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                if ((x < _borderSize || x > GridSize.x - _borderSize-1) ||
                    (y < _borderSize || y > GridSize.y - _borderSize-1)) {
                    grid[x, y].height = _borderValue;
                }
            }
        }
    }
    
    private void DoFade() {
        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                float time = (float)y / GridSize.y;
                grid[x, y].height = Mathf.Lerp(grid[x, y].perlinNoiseheight, _fadeValue, _fadeAnimationCurve.Evaluate(time)); 
            }
        }
        
    }
    
    private void DoThreshold() {
        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                if (grid[x, y].height > _treasholdValue) {
                    grid[x, y].height = 1;
                }
                else {
                    grid[x, y].height = 0;
                }
            }
        }
        
    }
    
    private void RecolorGrid() {
        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                grid[x, y].visual.SetColorByHeight(grid[x, y].height);
            }
        }
    }
    

    private void Populate() {
        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                CellVisual visual =Instantiate(PrefabsCell, new Vector3(x, 0, y), quaternion.identity);
                visual.transform.SetParent(transform);
                
                grid[x, y].visual = visual;
                visual.SetColorByHeight(grid[x, y].height);
            }
        }
    }
}