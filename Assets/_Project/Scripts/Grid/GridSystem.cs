using UnityEngine;

public class GridSystem : MonoBehaviour
{
    private Cell[,] cells;
    private int width;
    private int height;
    private float cellSize = 1.2f;

    public int Width => width;
    public int Height => height;

    private int activeArrowCount;

    public int ActiveArrowCount => activeArrowCount;




    public void CreateGrid(int gridWidth, int gridHeight)
    {
        width = gridWidth;
        height = gridHeight;
        cells = new Cell[width, height];

        float offsetX = (width - 1) * cellSize * 0.5f;
        float offsetY = (height - 1) * cellSize * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateCell(x, y, offsetX, offsetY);
            }
        }

        // Kamerayý grid'e sýðdýr
        FitCameraToGrid();

        Debug.Log($"[GridSystem] Grid created: {width}x{height} ({width * height} cells)");
    }

    private void FitCameraToGrid()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float gridWorldWidth = width * cellSize;
        float gridWorldHeight = height * cellSize;

        // Kamera aspect ratio
        float screenAspect = cam.aspect;

        // Dikey ve yatay için gereken kamera boyutu
        float verticalSize = (gridWorldHeight * 0.5f) + 1.5f;
        float horizontalSize = ((gridWorldWidth * 0.5f) / screenAspect) + 1.5f;

        // Büyük olaný seç
        cam.orthographicSize = Mathf.Max(verticalSize, horizontalSize);

        Debug.Log($"[GridSystem] Camera size adjusted to: {cam.orthographicSize} (aspect: {screenAspect})");
    }


    private void CreateCell(int x, int y, float offsetX, float offsetY)
    {
        GameObject cellObject = new GameObject();
        cellObject.transform.parent = transform;
        cellObject.transform.localPosition = new Vector3(
            x * cellSize - offsetX,
            y * cellSize - offsetY,
            0f
        );
        cellObject.transform.localScale = Vector3.one * (cellSize * 0.9f);

        // Add a square sprite
        SpriteRenderer renderer = cellObject.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateSquareSprite();
        renderer.sortingOrder = 0;

        Cell cell = cellObject.AddComponent<Cell>();
        cell.Initialize(x, y);
        cells[x, y] = cell;
    }



    private void OnEnable()
    {
        EventBus.Subscribe<ArrowExitedEvent>(OnArrowExited);
        EventBus.Subscribe<LevelStartedEvent>(OnLevelStarted);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ArrowExitedEvent>(OnArrowExited);
        EventBus.Unsubscribe<LevelStartedEvent>(OnLevelStarted);
    }

    private void OnLevelStarted(LevelStartedEvent eventData)
    {
        // Count will be set by LevelLoader after placing arrows
    }

    private void OnArrowExited(ArrowExitedEvent eventData)
    {
        activeArrowCount--;
        Debug.Log($"[GridSystem] Arrow exited. Remaining arrows: {activeArrowCount}");

        if (activeArrowCount <= 0)
        {
            Debug.Log("[GridSystem] All arrows cleared!");
            EventBus.Publish(new AllArrowsClearedEvent { totalMoves = 0 });
        }
    }

    public void SetActiveArrowCount(int count)
    {
        activeArrowCount = count;
        Debug.Log($"[GridSystem] Active arrow count set to: {activeArrowCount}");
    }



    private Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }

        texture.SetPixels(colors);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return Sprite.Create(
            texture,
            new Rect(0, 0, 32, 32),
            new Vector2(0.5f, 0.5f),
            32f
        );
    }



    public Cell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return cells[x, y];
        }
        return null;
    }

    public bool IsCellOccupied(int x, int y)
    {
        Cell cell = GetCell(x, y);
        if (cell != null)
        {
            return cell.IsOccupied;
        }
        return false;
    }

    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }



    public void ClearGrid()
    {
        if (cells == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cells[x, y] != null)
                {
                    Destroy(cells[x, y].gameObject);
                }
            }
        }

        cells = null;
        Debug.Log("[GridSystem] Grid cleared.");
    }
}
