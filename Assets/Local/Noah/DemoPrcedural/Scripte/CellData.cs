using UnityEngine;

public class CellData {
    public Vector2Int position;
    public float perlinNoiseheight;
    public float height;
    public CellVisual visual;

    public CellData(int x, int y) {
        position = new Vector2Int(x,y);
    }
}