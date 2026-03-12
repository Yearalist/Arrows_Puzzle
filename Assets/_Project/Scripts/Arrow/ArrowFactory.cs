using UnityEngine;
using System.Collections.Generic;

public class ArrowFactory : MonoBehaviour
{
    [SerializeField] private float cellSize = 1.2f;

    public float CellSize => cellSize;

    public Arrow CreateArrow(ArrowData arrowData, Vector2 gridOffset)
    {
        Vector3 headWorldPos = GetWorldPosition(arrowData.headX, arrowData.headY, gridOffset);
        GameObject arrowObject = new GameObject($"Arrow_{arrowData.direction}_{arrowData.headX}_{arrowData.headY}");
        arrowObject.transform.position = headWorldPos;

        Arrow arrow = arrowObject.AddComponent<Arrow>();
        arrow.Initialize(arrowData.headX, arrowData.headY, arrowData.direction, arrowData.bodyParts);

        arrowObject.AddComponent<ArrowMovement>();
        arrowObject.AddComponent<ArrowCollision>();

        // Build the path: body parts + head in order
        List<Vector2Int> path = BuildPath(arrowData);

        // Create line visual
        CreateLineVisual(arrowObject, path, arrowData, gridOffset);

        // Create arrow head visual
        CreateHeadVisual(arrowObject, arrowData, gridOffset);

        // Add collider covering all parts
        AddArrowCollider(arrowObject, arrowData);

        Debug.Log($"[ArrowFactory] Created {arrowData.direction} arrow at ({arrowData.headX}, {arrowData.headY})");
        return arrow;
    }

    private List<Vector2Int> BuildPath(ArrowData arrowData)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        // ÖNEMLÝ: Head (Baþ) daima 0. indeks olmalý (Arrow.cs ile senkronize)
        path.Add(new Vector2Int(arrowData.headX, arrowData.headY));

        if (arrowData.bodyParts != null)
        {
            foreach (ArrowPartData part in arrowData.bodyParts)
            {
                path.Add(new Vector2Int(part.x, part.y));
            }
        }

        return path;
    }

    private void CreateLineVisual(GameObject parent, List<Vector2Int> path, ArrowData arrowData, Vector2 gridOffset)
    {
        if (path.Count < 2) return;

        LineRenderer line = parent.AddComponent<LineRenderer>();
        line.startWidth = cellSize * 0.15f;
        line.endWidth = cellSize * 0.15f;
        line.positionCount = path.Count;
        line.useWorldSpace = true; // DEÐÝÞTÝ: Artýk baðýmsýz hareket etmesi için Dünya Alanýný kullanýyor
        line.sortingOrder = 1;

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = new Color(0.12f, 0.12f, 0.18f);
        line.endColor = new Color(0.12f, 0.12f, 0.18f);

        for (int i = 0; i < path.Count; i++)
        {
            // DEÐÝÞTÝ: Gerçek dünya pozisyonlarýný hesaplayýp atýyoruz
            Vector3 worldPos = GetWorldPosition(path[i].x, path[i].y, gridOffset);
            line.SetPosition(i, worldPos);
        }

        // Arrow referansýna baðla
        parent.GetComponent<Arrow>().lineRenderer = line;
    }

    private void CreateHeadVisual(GameObject parent, ArrowData arrowData, Vector2 gridOffset)
    {
        GameObject headObj = new GameObject("Head");
        headObj.transform.parent = parent.transform;

        // DEÐÝÞTÝ: Gerçek dünya pozisyonunu ata
        headObj.transform.position = GetWorldPosition(arrowData.headX, arrowData.headY, gridOffset);

        SpriteRenderer renderer = headObj.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateHeadSprite();
        renderer.color = new Color(0.12f, 0.12f, 0.18f);
        renderer.sortingOrder = 2;

        float headSize = cellSize * 0.5f;
        headObj.transform.localScale = Vector3.one * headSize;
        headObj.transform.rotation = Quaternion.Euler(0f, 0f, GetRotation(arrowData.direction));

        // Arrow referansýna baðla
        parent.GetComponent<Arrow>().headTransform = headObj.transform;
    }

    private void AddArrowCollider(GameObject arrowObject, ArrowData arrowData)
    {
        float minX = 0f;
        float maxX = 0f;
        float minY = 0f;
        float maxY = 0f;

        if (arrowData.bodyParts != null)
        {
            foreach (ArrowPartData part in arrowData.bodyParts)
            {
                float offsetX = (part.x - arrowData.headX) * cellSize;
                float offsetY = (part.y - arrowData.headY) * cellSize;

                if (offsetX < minX) minX = offsetX;
                if (offsetX > maxX) maxX = offsetX;
                if (offsetY < minY) minY = offsetY;
                if (offsetY > maxY) maxY = offsetY;
            }
        }

        BoxCollider2D collider = arrowObject.AddComponent<BoxCollider2D>();

        float centerX = (minX + maxX) * 0.5f;
        float centerY = (minY + maxY) * 0.5f;
        float sizeX = (maxX - minX) + cellSize;
        float sizeY = (maxY - minY) + cellSize;

        collider.offset = new Vector2(centerX, centerY);
        collider.size = new Vector2(sizeX, sizeY);
    }

    public Vector3 GetWorldPosition(int x, int y, Vector2 gridOffset)
    {
        return new Vector3(
            x * cellSize - gridOffset.x,
            y * cellSize - gridOffset.y,
            0f
        );
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

    private Sprite CreateHeadSprite()
    {
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.clear;
        }

        // Draw arrow head pointing up
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