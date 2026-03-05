// All game events defined as structs
// Each struct carries the data that listeners need
// Using structs instead of strings = type-safe, no typos, autocomplete works

public struct ArrowClickedEvent
{
    public int gridX;
    public int gridY;
}

public struct ArrowMovedEvent
{
    public int gridX;
    public int gridY;
}

public struct ArrowExitedEvent
{
    public int gridX;
    public int gridY;
    public int pointsEarned;
}

public struct ArrowCollisionEvent
{
    public int arrow1X;
    public int arrow1Y;
    public int arrow2X;
    public int arrow2Y;
}

public struct HealthChangedEvent
{
    public int currentHealth;
    public int maxHealth;
}

public struct LivesExhaustedEvent { }

public struct AllArrowsClearedEvent
{
    public int totalMoves;
}

public struct LevelStartedEvent
{
    public int levelNumber;
}

public struct LevelCompletedEvent
{
    public int levelNumber;
    public int stars;
    public int score;
}

public struct GameOverEvent { }