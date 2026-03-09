using UnityEngine;

public class ScoreSystem : MonoBehaviour
{

    private int currentScore;
    private int moveCount;
    private int arrowsExited;
    private int collisionCount;

    public int CurrentScore => currentScore;
    public int MoveCount => moveCount;
    public int ArrowsExited => arrowsExited;
    public int CollisionCount => collisionCount;


    private void OnEnable()
    {
        EventBus.Subscribe<ArrowClickedEvent>(OnArrowClicked);
        EventBus.Subscribe<ArrowExitedEvent>(OnArrowExited);
        EventBus.Subscribe<ArrowCollisionEvent>(OnArrowCollision);
        EventBus.Subscribe<LevelStartedEvent>(OnLevelStarted);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ArrowClickedEvent>(OnArrowClicked);
        EventBus.Unsubscribe<ArrowExitedEvent>(OnArrowExited);
        EventBus.Unsubscribe<ArrowCollisionEvent>(OnArrowCollision);
        EventBus.Unsubscribe<LevelStartedEvent>(OnLevelStarted);
    }

    private void OnArrowClicked(ArrowClickedEvent eventData)
    {
        moveCount++;
        Debug.Log($"[ScoreSystem] Move #{moveCount}");
    }

    private void OnArrowExited(ArrowExitedEvent eventData)
    {
        arrowsExited++;
        currentScore += eventData.pointsEarned;
        Debug.Log($"[ScoreSystem] Arrow exited! +{eventData.pointsEarned} points. Total: {currentScore}. Arrows exited: {arrowsExited}");
    }

    private void OnArrowCollision(ArrowCollisionEvent eventData)
    {
        collisionCount++;
        Debug.Log($"[ScoreSystem] Collision #{collisionCount}");
    }

    private void OnLevelStarted(LevelStartedEvent eventData)
    {
        ResetScore();
        Debug.Log($"[ScoreSystem] Score reset for level {eventData.levelNumber}");
    }

    public int CalculateStars(LevelData levelData)
    {
        if (collisionCount == 0 && moveCount <= levelData.threeStarMoves)
        {
            return 3;
        }
        else if (collisionCount <= 1 && moveCount <= levelData.twoStarMoves)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
    public void ResetScore()
    {
        currentScore = 0;
        moveCount = 0;
        arrowsExited = 0;
        collisionCount = 0;
    }

}
