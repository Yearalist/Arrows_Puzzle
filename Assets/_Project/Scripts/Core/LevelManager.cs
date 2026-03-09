using Cysharp.Threading.Tasks;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;

    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("[LevelManager] Instance not found!");
            }
            return _instance;
        }
    }

    [SerializeField] private LevelData[] allLevels;
    [SerializeField] private LevelLoader levelLoader;

    private int currentLevelIndex;

    public int CurrentLevelIndex => currentLevelIndex;
    public int TotalLevels => allLevels.Length;
    public LevelData CurrentLevelData => allLevels[currentLevelIndex];

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        currentLevelIndex = 0;
        Debug.Log($"[LevelManager] Initialized with {allLevels.Length} levels");
    }


    public void LoadCurrentLevel()
    {
        if (currentLevelIndex < allLevels.Length)
        {
            levelLoader.LoadLevel(allLevels[currentLevelIndex]).Forget();
            Debug.Log($"[LevelManager] Loading level {currentLevelIndex + 1}");
        }
        else
        {
            Debug.Log("[LevelManager] All levels completed!");
        }
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < allLevels.Length)
        {
            LoadCurrentLevel();
        }
        else
        {
            Debug.Log("[LevelManager] No more levels! You finished the game!");
            currentLevelIndex = allLevels.Length - 1;
        }
    }

    public void RestartCurrentLevel()
    {
        LoadCurrentLevel();
    }

    public void LoadLevel(int index)
    {
        if (index >= 0 && index < allLevels.Length)
        {
            currentLevelIndex = index;
            LoadCurrentLevel();
        }
    }
}