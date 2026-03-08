using UnityEngine;
using Cysharp.Threading.Tasks;
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GridSystem gridSystem;


    public async UniTask LoadLevel(LevelData levelData)
    {
        Debug.Log($"[LevelLoader] Loading Level {levelData.levelNumber}...");

        // Step 1: Clear old grid if exists
        gridSystem.ClearGrid();

        // Small delay for visual smoothness
        await UniTask.Delay(100);

        // Step 2: Create grid
        gridSystem.CreateGrid(levelData.gridWidth, levelData.gridHeight);

        await UniTask.Delay(100);

        // Step 3: Place arrows on grid
        PlaceArrows(levelData);

        // Step 4: Notify that level is ready
        EventBus.Publish(new LevelStartedEvent { levelNumber = levelData.levelNumber });

        Debug.Log($"[LevelLoader] Level {levelData.levelNumber} loaded successfully!");
    }


    private void PlaceArrows(LevelData levelData)
    {
        foreach (ArrowData arrowData in levelData.arrows)
        {
            Cell cell = gridSystem.GetCell(arrowData.x, arrowData.y);

            if (cell != null)
            {
                cell.SetOccupied(true);
                CreateArrowVisual(arrowData, cell);
                Debug.Log($"[LevelLoader] Placed {arrowData.direction} arrow at ({arrowData.x}, {arrowData.y})");
            }
            else
            {
                Debug.LogWarning($"[LevelLoader] Invalid position ({arrowData.x}, {arrowData.y}) for arrow!");
            }
        }
    }


    private void CreateArrowVisual(ArrowData arrowData, Cell cell)
    {
        GameObject arrowObject = new GameObject($"Arrow_{arrowData.direction}_{arrowData.x}_{arrowData.y}");
        arrowObject.transform.position = cell.transform.position;
        arrowObject.transform.localScale = Vector3.one * 0.5f;

        SpriteRenderer renderer = arrowObject.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateArrowSprite();
        renderer.sortingOrder = 1;

        // Rotate arrow based on direction
        float rotation = GetRotationForDirection(arrowData.direction);
        arrowObject.transform.rotation = Quaternion.Euler(0f, 0f, rotation);

        // Color based on direction
        renderer.color = GetColorForDirection(arrowData.direction);
    }


    private float GetRotationForDirection(ArrowDirection direction)
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

    private Color GetColorForDirection(ArrowDirection direction)
    {
        switch (direction)
        {
            case ArrowDirection.Up: return new Color(0.2f, 0.7f, 0.3f);    // Green
            case ArrowDirection.Right: return new Color(0.2f, 0.5f, 0.9f); // Blue
            case ArrowDirection.Down: return new Color(0.9f, 0.3f, 0.2f);  // Red
            case ArrowDirection.Left: return new Color(0.9f, 0.7f, 0.1f);  // Yellow
            default: return Color.white;
        }
    }


    private Sprite CreateArrowSprite()
    {
        // Create a simple triangle/arrow shape
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];

        // Fill with transparent
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.clear;
        }

        // Draw a simple arrow (triangle pointing up)
        for (int y = 0; y < size; y++)
        {
            int halfWidth = y / 2;
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


