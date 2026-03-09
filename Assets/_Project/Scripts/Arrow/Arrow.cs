using UnityEngine;

public class Arrow : MonoBehaviour
{
    private int gridX;
    private int gridY;
    private ArrowDirection direction;
    private bool isMoving;
    private bool isActive;

    public int GridX => gridX;
    public int GridY => gridY;
    public ArrowDirection Direction => direction;
    public bool IsMoving => isMoving;
    public bool IsActive => isActive;

    public void Initialize(int x, int y, ArrowDirection dir)
    {
        gridX = x;
        gridY = y;
        direction = dir;
        isMoving = false;
        isActive = true;
        gameObject.name = $"Arrow_{dir}_{x}_{y}";
    }

    public Vector2Int GetDirectionVector()
    {
        switch (direction)
        {
            case ArrowDirection.Up: return Vector2Int.up;
            case ArrowDirection.Down: return Vector2Int.down;
            case ArrowDirection.Left: return Vector2Int.left;
            case ArrowDirection.Right: return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }

    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }

    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    public void Activate(int x, int y, ArrowDirection dir)
    {
        gridX = x;
        gridY = y;
        direction = dir;
        isMoving = false;
        isActive = true;
        gameObject.SetActive(true);
    }
}