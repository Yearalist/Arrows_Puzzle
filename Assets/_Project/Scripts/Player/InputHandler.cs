using Cysharp.Threading.Tasks;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private Camera mainCamera;

    private bool inputEnabled = true;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (!inputEnabled) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

        if (hit.collider != null)
        {
            Arrow arrow = hit.collider.GetComponent<Arrow>();

            if (arrow != null && arrow.IsActive && !arrow.IsMoving)
            {
                OnArrowClicked(arrow);
            }
        }
    }

    private void OnArrowClicked(Arrow arrow)
    {
        Debug.Log($"[InputHandler] Arrow clicked at ({arrow.HeadX}, {arrow.HeadY}) facing {arrow.Direction}");

        EventBus.Publish(new ArrowClickedEvent
        {
            gridX = arrow.HeadX,
            gridY = arrow.HeadY
        });

        ArrowMovement movement = arrow.GetComponent<ArrowMovement>();

        if (movement != null)
        {
            movement.MoveArrow(gridSystem).Forget();
        }
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }
}