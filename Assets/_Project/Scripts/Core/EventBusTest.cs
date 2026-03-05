using UnityEngine;

public class EventBusTest : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus.Subscribe<ArrowClickedEvent>(OnArrowClicked);
        EventBus.Subscribe<HealthChangedEvent>(OnHealthChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ArrowClickedEvent>(OnArrowClicked);
        EventBus.Unsubscribe<HealthChangedEvent>(OnHealthChanged);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventBus.Publish(new ArrowClickedEvent { gridX = 2, gridY = 3 });
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            EventBus.Publish(new HealthChangedEvent { currentHealth = 2, maxHealth = 3 });
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            EventBus.Publish(new LivesExhaustedEvent());
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            EventBus.Publish(new AllArrowsClearedEvent { totalMoves = 5 });
        }
    }

    private void OnArrowClicked(ArrowClickedEvent eventData)
    {
        Debug.Log($"[EventBus Test] Arrow clicked at ({eventData.gridX}, {eventData.gridY})");
    }

    private void OnHealthChanged(HealthChangedEvent eventData)
    {
        Debug.Log($"[EventBus Test] Health: {eventData.currentHealth}/{eventData.maxHealth}");
    }
}