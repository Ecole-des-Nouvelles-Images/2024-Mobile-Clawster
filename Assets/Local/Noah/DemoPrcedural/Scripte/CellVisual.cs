using UnityEngine;

public class CellVisual : MonoBehaviour
{
    public Vector2Int Cor;
    public SpriteRenderer SpriteRenderer;

    public void SetColorByHeight(float height) {
        Color col = Color.white *height;
        col.a = 1;
        SpriteRenderer.color = col;
    }
}