using UnityEngine;

public class Cell : MonoBehaviour
{
    private int gridX;
    private int gridY;
    private bool isOccupied;
    private SpriteRenderer spriteRenderer;

    public int GridX => gridX;
    public int GridY => gridY;
    public bool IsOccupied => isOccupied;



    public void Initialize(int x, int y)
    {
        gridX = x;
        gridY = y;
        isOccupied = false;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        SetDefaultColor();

        gameObject.name = $"Cell ({x}, {y})";
    }



    private void SetDefaultColor()
    {
        if (spriteRenderer != null)
        {
            // Checkerboard pattern like a chess board
            bool isLight = (gridX + gridY) % 2 == 0;
            spriteRenderer.color = isLight
                ? new Color(0.9f, 0.9f, 0.95f)
                : new Color(0.8f, 0.8f, 0.88f);
        }
    }



    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }

    public void Highlight()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.7f, 0.9f, 0.7f);
        }
    }

    public void ResetColor()
    {
        SetDefaultColor();
    }
}