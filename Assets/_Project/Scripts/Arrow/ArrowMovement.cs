using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
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
        if (arrow == null || !arrow.IsActive || arrow.IsMoving)
        {
            return;
        }

        arrow.SetMoving(true);
        moveCancellation = new CancellationTokenSource();

        Vector2Int directionVector = arrow.GetDirectionVector();
        int currentX = arrow.GridX;
        int currentY = arrow.GridY;

        Debug.Log($"[ArrowMovement] Arrow at ({currentX}, {currentY}) moving {arrow.Direction}");

        // Clear the starting cell
        Cell startCell = gridSystem.GetCell(currentX, currentY);
        if (startCell != null)
        {
            startCell.SetOccupied(false);
        }

        bool collisionDetected = false;

        while (true)
        {
            int nextX = currentX + directionVector.x;
            int nextY = currentY + directionVector.y;

            // Check if next position is outside grid = arrow escaped!
            if (!gridSystem.IsInsideGrid(nextX, nextY))
            {
                // Move to the edge and exit
                Vector3 exitPosition = new Vector3(
                    nextX * cellSize - (gridSystem.Width - 1) * cellSize * 0.5f,
                    nextY * cellSize - (gridSystem.Height - 1) * cellSize * 0.5f,
                    0f
                );

                await MoveToPosition(exitPosition, moveCancellation.Token);

                // Arrow successfully exited
                arrow.Deactivate();
                EventBus.Publish(new ArrowExitedEvent
                {
                    gridX = currentX,
                    gridY = currentY,
                    pointsEarned = 10
                });

                Debug.Log($"[ArrowMovement] Arrow exited the grid!");
                break;
            }

            // Check if next cell is occupied = collision!
            if (gridSystem.IsCellOccupied(nextX, nextY))
            {
                collisionDetected = true;

                // Mark current cell as occupied since arrow stops here
                Cell stopCell = gridSystem.GetCell(currentX, currentY);
                if (stopCell != null)
                {
                    stopCell.SetOccupied(true);
                }

                EventBus.Publish(new ArrowCollisionEvent
                {
                    arrow1X = currentX,
                    arrow1Y = currentY,
                    arrow2X = nextX,
                    arrow2Y = nextY
                });

                Debug.Log($"[ArrowMovement] Collision at ({nextX}, {nextY})! Arrow stopped at ({currentX}, {currentY})");
                break;
            }

            // Move to next cell
            Vector3 targetPosition = new Vector3(
                nextX * cellSize - (gridSystem.Width - 1) * cellSize * 0.5f,
                nextY * cellSize - (gridSystem.Height - 1) * cellSize * 0.5f,
                0f
            );

            await MoveToPosition(targetPosition, moveCancellation.Token);

            currentX = nextX;
            currentY = nextY;
            arrow.SetGridPosition(currentX, currentY);
        }

        arrow.SetMoving(false);
    }


    private async UniTask MoveToPosition(Vector3 target, CancellationToken token)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            await UniTask.Yield(token);
        }

        transform.position = target;
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

    private void OnDestroy()
    {
        CancelMovement();
    }
}






