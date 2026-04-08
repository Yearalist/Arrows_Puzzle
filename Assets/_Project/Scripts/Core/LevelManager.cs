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

        currentLevelIndex = PlayerPrefs.GetInt("SelectedLevel", 0);

        Debug.Log($"[LevelManager] Initialized with {allLevels.Length} levels. Starting at index {currentLevelIndex}");
    }

    public void LoadCurrentLevel()
    {
        currentLevelIndex = PlayerPrefs.GetInt("SelectedLevel", 0);

        // Her seferinde sahnedeki güncel LevelLoader'ý bul
        LevelLoader loader = FindObjectOfType<LevelLoader>();

        if (loader == null)
        {
            Debug.LogError("[LevelManager] LevelLoader not found in scene!");
            return;
        }

        if (currentLevelIndex < allLevels.Length)
        {
            loader.LoadLevel(allLevels[currentLevelIndex]).Forget();
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
        PlayerPrefs.SetInt("SelectedLevel", currentLevelIndex);
        PlayerPrefs.Save();

        if (currentLevelIndex < allLevels.Length)
        {
            LoadCurrentLevel();
        }
        else
        {
            Debug.Log("[LevelManager] No more levels! Game complete!");
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