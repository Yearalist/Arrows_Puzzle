using UnityEngine;
using System.Collections.Generic;

public class Arrow : MonoBehaviour
{
    private int headX;
    private int headY;
    private ArrowDirection direction;
    private bool isMoving;
    private bool isActive;
    private List<Vector2Int> allOccupiedCells;
    private List<GameObject> bodyVisuals;

    // YENÝ EKLENEN REFERANSLAR
    public LineRenderer lineRenderer;
    public Transform headTransform;

    public int HeadX => headX;
    public int HeadY => headY;
    public ArrowDirection Direction => direction;
    public bool IsMoving => isMoving;
    public bool IsActive => isActive;
    public List<Vector2Int> AllOccupiedCells => allOccupiedCells;

    public void Initialize(int hx, int hy, ArrowDirection dir, ArrowPartData[] bodyParts)
    {
        headX = hx;
        headY = hy;
        direction = dir;
        isMoving = false;
        isActive = true;

        allOccupiedCells = new List<Vector2Int>();
        bodyVisuals = new List<GameObject>();

        // Ýndeks 0 her zaman Baþ (Head) kýsmý olacak.
        allOccupiedCells.Add(new Vector2Int(headX, headY));

        if (bodyParts != null)
        {
            foreach (ArrowPartData part in bodyParts)
            {
                allOccupiedCells.Add(new Vector2Int(part.x, part.y));
            }
        }

        gameObject.name = $"Arrow_{dir}_{hx}_{hy}";
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

    // GÜNCELLENDÝ: Yýlan (Snake) hareketi mantýðý
    public void MoveAllCells(Vector2Int dirVector)
    {
        // Kuyruk kýsýmlarý bir öndekinin pozisyonunu alýr (Sondan baþa doðru kopyalýyoruz)
        for (int i = allOccupiedCells.Count - 1; i > 0; i--)
        {
            allOccupiedCells[i] = allOccupiedCells[i - 1];
        }

        // Baþ kýsmý (Index 0) yön vektörüne doðru hareket eder
        allOccupiedCells[0] = new Vector2Int(
            allOccupiedCells[0].x + dirVector.x,
            allOccupiedCells[0].y + dirVector.y
        );

        headX = allOccupiedCells[0].x;
        headY = allOccupiedCells[0].y;
    }

    public void AddBodyVisual(GameObject visual)
    {
        bodyVisuals.Add(visual);
    }

    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        isActive = true;
        isMoving = false;
        gameObject.SetActive(true);

        foreach (GameObject visual in bodyVisuals)
        {
            if (visual != null) visual.SetActive(true);
        }
    }
}