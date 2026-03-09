using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    private Arrow arrow;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        arrow = GetComponent<Arrow>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ArrowCollisionEvent>(OnCollision);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ArrowCollisionEvent>(OnCollision);
    }

    private void OnCollision(ArrowCollisionEvent eventData)
    {
        // Check if this arrow is involved in the collision
        bool isArrow1 = eventData.arrow1X == arrow.GridX && eventData.arrow1Y == arrow.GridY;
        bool isArrow2 = eventData.arrow2X == arrow.GridX && eventData.arrow2Y == arrow.GridY;

        if (isArrow1 || isArrow2)
        {
            PlayCollisionEffect();
        }
    }

    private void PlayCollisionEffect()
    {
        Debug.Log($"[ArrowCollision] Arrow at ({arrow.GridX}, {arrow.GridY}) involved in collision!");

        // Flash red briefly
        if (spriteRenderer != null)
        {
            FlashRed().Forget();
        }
    }

    private async Cysharp.Threading.Tasks.UniTaskVoid FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        await Cysharp.Threading.Tasks.UniTask.Delay(300);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}