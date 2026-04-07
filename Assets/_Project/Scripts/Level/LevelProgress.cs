using UnityEngine;

public static class LevelProgress
{
    private const string LEVEL_STARS_KEY = "LevelStars_";
    private const string HIGHEST_LEVEL_KEY = "HighestUnlockedLevel";

    public static void SaveLevelResult(int levelNumber, int stars)
    {
        int previousStars = GetLevelStars(levelNumber);

        if (stars > previousStars)
        {
            PlayerPrefs.SetInt(LEVEL_STARS_KEY + levelNumber, stars);
        }

        int currentHighest = GetHighestUnlockedLevel();
        if (levelNumber >= currentHighest)
        {
            PlayerPrefs.SetInt(HIGHEST_LEVEL_KEY, levelNumber + 1);
        }

        PlayerPrefs.Save();
    }

    public static int GetLevelStars(int levelNumber)
    {
        return PlayerPrefs.GetInt(LEVEL_STARS_KEY + levelNumber, 0);
    }

    public static int GetHighestUnlockedLevel()
    {
        return PlayerPrefs.GetInt(HIGHEST_LEVEL_KEY, 1);
    }

    public static bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber <= GetHighestUnlockedLevel();
    }

    public static int GetTotalStars()
    {
        int total = 0;
        for (int i = 1; i <= 100; i++)
        {
            total += GetLevelStars(i);
        }
        return total;
    }

    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}