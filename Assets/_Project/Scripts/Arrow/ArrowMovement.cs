using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

public class ArrowMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float cellSize = 1.2f;

    private Arrow arrow;
    private CancellationTokenSource moveCancellation;

    private void Awake()
    {
        arrow = GetComponent<Arrow>();
    }

    public async UniTask MoveArrow(GridSystem gridSystem)
    {
        if (arrow == null || !arrow.IsActive || arrow.IsMoving) return;

        arrow.SetMoving(true);
        moveCancellation = new CancellationTokenSource();
        Vector2Int dirVector = arrow.GetDirectionVector();

        ClearArrowCells(gridSystem);

        while (true)
        {
            // GÜNCELLENDÝ: Yýlan mantýðýna göre bir sonraki pozisyonlar
            List<Vector2Int> nextPositions = new List<Vector2Int>();

            // 0. Ýndeks (Baþ) yöne doðru gider
            nextPositions.Add(new Vector2Int(arrow.AllOccupiedCells[0].x + dirVector.x, arrow.AllOccupiedCells[0].y + dirVector.y));

            // Diðer parçalar bir öncekini takip eder
            for (int i = 1; i < arrow.AllOccupiedCells.Count; i++)
            {
                nextPositions.Add(arrow.AllOccupiedCells[i - 1]);
            }

            // Izgara dýþýna çýkma ve çarpýþma kontrolleri (Ayný kalýyor)
            bool allOutside = true;
            foreach (Vector2Int pos in nextPositions)
            {
                if (gridSystem.IsInsideGrid(pos.x, pos.y)) { allOutside = false; break; }
            }

            bool collision = false;
            foreach (Vector2Int pos in nextPositions)
            {
                if (gridSystem.IsInsideGrid(pos.x, pos.y) && gridSystem.IsCellOccupied(pos.x, pos.y))
                {
                    collision = true;
                    EventBus.Publish(new ArrowCollisionEvent { arrow1X = arrow.HeadX, arrow1Y = arrow.HeadY, arrow2X = pos.x, arrow2Y = pos.y });
                    break;
                }
            }

            if (collision)
            {
                MarkArrowCells(gridSystem);
                break;
            }

            // GÜNCELLENDÝ: Görsel hedefleri belirle
            Vector3[] startVisualPositions = new Vector3[arrow.AllOccupiedCells.Count];
            Vector3[] targetVisualPositions = new Vector3[arrow.AllOccupiedCells.Count];

            for (int i = 0; i < arrow.AllOccupiedCells.Count; i++)
            {
                startVisualPositions[i] = arrow.lineRenderer.GetPosition(i);
            }

            // Baþýn hedefi
            targetVisualPositions[0] = startVisualPositions[0] + new Vector3(dirVector.x * cellSize, dirVector.y * cellSize, 0f);

            // Gövdenin hedefi (önündeki parçanýn baþladýðý yer)
            for (int i = 1; i < arrow.AllOccupiedCells.Count; i++)
            {
                targetVisualPositions[i] = startVisualPositions[i - 1];
            }

            // Parçalarý hareket ettir
            await MoveStepPoints(startVisualPositions, targetVisualPositions, moveCancellation.Token);

            // Mantýksal grid hücrelerini güncelle
            arrow.MoveAllCells(dirVector);

            // Tüm ok çýktý mý kontrolü
            bool fullyExited = true;
            foreach (Vector2Int cell in arrow.AllOccupiedCells)
            {
                if (gridSystem.IsInsideGrid(cell.x, cell.y)) { fullyExited = false; break; }
            }

            if (fullyExited)
            {
                arrow.Deactivate();
                EventBus.Publish(new ArrowExitedEvent { gridX = arrow.HeadX, gridY = arrow.HeadY, pointsEarned = 10 });
                break;
            }
        }

        arrow.SetMoving(false);
    }

    private void ClearArrowCells(GridSystem gridSystem)
    {
        foreach (Vector2Int cell in arrow.AllOccupiedCells)
        {
            Cell gridCell = gridSystem.GetCell(cell.x, cell.y);
            if (gridCell != null) gridCell.SetOccupied(false);
        }
    }

    private void MarkArrowCells(GridSystem gridSystem)
    {
        foreach (Vector2Int cell in arrow.AllOccupiedCells)
        {
            Cell gridCell = gridSystem.GetCell(cell.x, cell.y);
            if (gridCell != null) gridCell.SetOccupied(true);
        }
    }

    private async UniTask MoveStepPoints(Vector3[] startPos, Vector3[] targetPos, CancellationToken token)
    {
        if (arrow.lineRenderer == null) return;

        float distance = Vector3.Distance(startPos[0], targetPos[0]);
        if (distance <= 0.001f) return;

        float traveled = 0f;

        while (traveled < distance)
        {
            float move = moveSpeed * Time.deltaTime;
            traveled += move;
            float t = Mathf.Clamp01(traveled / distance);

            for (int i = 0; i < startPos.Length; i++)
            {
                Vector3 newPos = Vector3.Lerp(startPos[i], targetPos[i], t);

                if (i < arrow.lineRenderer.positionCount)
                {
                    arrow.lineRenderer.SetPosition(i, newPos);
                }

                if (i == 0 && arrow.headTransform != null)
                {
                    arrow.headTransform.position = newPos;
                }
            }

            await UniTask.Yield(token);
        }

        // Final pozisyonlarý tam olarak ata
        for (int i = 0; i < targetPos.Length; i++)
        {
            if (i < arrow.lineRenderer.positionCount)
            {
                arrow.lineRenderer.SetPosition(i, targetPos[i]);
            }

            if (i == 0 && arrow.headTransform != null)
            {
                arrow.headTransform.position = targetPos[i];
            }
        }
    }

    public void CancelMovement()
    {
        if (moveCancellation != null)
        {
            moveCancellation.Cancel();
            moveCancellation.Dispose();
            moveCancellation = null;
        }
    }

    private void OnDestroy() { CancelMovement(); }
}