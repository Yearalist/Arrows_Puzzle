using UnityEngine;
using UnityEngine.UI;

public class HeartDisplay : MonoBehaviour
{
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);




    private void OnEnable()
    {
        EventBus.Subscribe<HealthChangedEvent>(OnHealthChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<HealthChangedEvent>(OnHealthChanged);
    }

    private void OnHealthChanged(HealthChangedEvent eventData)
    {
        UpdateHearts(eventData.currentHealth, eventData.maxHealth);
    }

    private void UpdateHearts(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                if (i < currentHealth)
                {
                    heartImages[i].color = activeColor;
                }
                else
                {
                    heartImages[i].color = inactiveColor;
                }
            }
        }

        Debug.Log($"[HeartDisplay] Updated hearts: {currentHealth}/{maxHealth}");
    }

    public void InitializeHearts(int maxHealth)
    {
        UpdateHearts(maxHealth, maxHealth);
    }
}

