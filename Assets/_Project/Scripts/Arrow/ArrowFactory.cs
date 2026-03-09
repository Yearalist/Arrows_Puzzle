using UnityEngine;

public class ArrowFactory : MonoBehaviour
{
    public Arrow CreateArrow(int x, int y, ArrowDirection direction, float cellSize, Vector2 gridOffset)
    {
        GameObject arrowObject = new GameObject($"Arrow_{direction}_{x}_{y}");

        // Position
        arrowObject.transform.position = new Vector3(
            x * cellSize - gridOffset.x,
            y * cellSize - gridOffset.y,
            0f
        );

        // Add components
        Arrow arrow = arrowObject.AddComponent<Arrow>();
        arrow.Initialize(x, y, direction);

        ArrowMovement movement = arrowObject.AddComponent<ArrowMovement>();
        ArrowCollision collision = arrowObject.AddComponent<ArrowCollision>();

        // Visual
        SpriteRenderer renderer = arrowObject.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateArrowSprite();
        renderer.sortingOrder = 1;
        arrowObject.transform.localScale = Vector3.one * 0.5f;

        // Rotation
        float rotation = GetRotation(direction);
        arrowObject.transform.rotation = Quaternion.Euler(0f, 0f, rotation);

        // Color
        renderer.color = GetColor(direction);

        // Add collider for click detection
        BoxCollider2D collider = arrowObject.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one * 1.5f;

        Debug.Log($"[ArrowFactory] Created {direction} arrow at ({x}, {y})");
        return arrow;
    }

    private float GetRotation(ArrowDirection direction)
    {
        switch (direction)
        {
            case ArrowDirection.Up: return 0f;
            case ArrowDirection.Right: return -90f;
            case ArrowDirection.Down: return 180f;
            case ArrowDirection.Left: return 90f;
            default: return 0f;
        }
    }

    private Color GetColor(ArrowDirection direction)
    {
        switch (direction)
        {
            case ArrowDirection.Up: return new Color(0.2f, 0.7f, 0.3f);
            case ArrowDirection.Right: return new Color(0.2f, 0.5f, 0.9f);
            case ArrowDirection.Down: return new Color(0.9f, 0.3f, 0.2f);
            case ArrowDirection.Left: return new Color(0.9f, 0.7f, 0.1f);
            default: return Color.white;
        }
    }

    private Sprite CreateArrowSprite()
    {
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.clear;
        }

        // Draw arrow pointing UP (tip at top, base at bottom)
        for (int y = 0; y < size; y++)
        {
            int invertedY = size - 1 - y;
            int halfWidth = invertedY / 2;
            int center = size / 2;

            for (int x = center - halfWidth; x <= center + halfWidth; x++)
            {
                if (x >= 0 && x < size)
                {
                    colors[y * size + x] = Color.white;
                }
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}