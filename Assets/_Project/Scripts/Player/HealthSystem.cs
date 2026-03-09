using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;


    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"[HealthSystem] Initialized with {currentHealth}/{maxHealth} lives");
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ArrowCollisionEvent>(OnArrowCollision);
        EventBus.Subscribe<LevelStartedEvent>(OnLevelStarted);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ArrowCollisionEvent>(OnArrowCollision);
        EventBus.Unsubscribe<LevelStartedEvent>(OnLevelStarted);
    }

    private void OnArrowCollision(ArrowCollisionEvent eventData)
    {
        TakeDamage();
    }

    private void OnLevelStarted(LevelStartedEvent eventData)
    {
        ResetHealth();
        Debug.Log($"[HealthSystem] Health reset for level {eventData.levelNumber}");
    }

    public void TakeDamage()
    {
        if (!IsAlive) return;

        currentHealth--;
        Debug.Log($"[HealthSystem] Took damage! Lives remaining: {currentHealth}/{maxHealth}");

        EventBus.Publish(new HealthChangedEvent
        {
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });

        if (!IsAlive)
        {
            Debug.Log("[HealthSystem] No lives remaining!");
            EventBus.Publish(new LivesExhaustedEvent());
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        EventBus.Publish(new HealthChangedEvent
        {
            currentHealth = currentHealth,
            maxHealth = maxHealth
        });
    }



}
