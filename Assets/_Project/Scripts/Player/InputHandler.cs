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

        // Tek bir hit yerine tüm hit'leri al
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition2D, Vector2.zero);

        Arrow closestArrow = null;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                Arrow arrow = hit.collider.GetComponent<Arrow>();

                if (arrow != null && arrow.IsActive && !arrow.IsMoving)
                {
                    // Týklanan noktaya en yakýn oku bul
                    float dist = Vector2.Distance(mousePosition2D, hit.point);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestArrow = arrow;
                    }
                }
            }
        }

        if (closestArrow != null)
        {
            OnArrowClicked(closestArrow);
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