using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private GameStateMachine stateMachine;
    public GameStateMachine StateMachine => stateMachine;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("[GameManager] Instance not found! Make sure GameManager exists in the scene.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("[GameManager] Duplicate found! Destroying this one.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeSystems();
        Debug.Log("[GameManager] Initialized successfully!");
    }

    private void InitializeSystems()
    {
        stateMachine = new GameStateMachine();
        stateMachine.ChangeState(new PlayingState());
    }

    private void Start()
    {
        LevelManager.Instance.LoadCurrentLevel();
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<AllArrowsClearedEvent>(OnAllArrowsCleared);
        EventBus.Subscribe<LivesExhaustedEvent>(OnLivesExhausted);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<AllArrowsClearedEvent>(OnAllArrowsCleared);
        EventBus.Unsubscribe<LivesExhaustedEvent>(OnLivesExhausted);
    }

    private void OnAllArrowsCleared(AllArrowsClearedEvent eventData)
    {
        Debug.Log("[GameManager] Level completed!");

        ScoreSystem scoreSystem = FindObjectOfType<ScoreSystem>();
        int stars = 1;
        int score = 0;

        if (scoreSystem != null)
        {
            stars = scoreSystem.CalculateStars(LevelManager.Instance.CurrentLevelData);
            score = scoreSystem.CurrentScore;
        }

        int levelNum = LevelManager.Instance.CurrentLevelIndex + 1;
        LevelProgress.SaveLevelResult(levelNum, stars);

        EventBus.Publish(new LevelCompletedEvent
        {
            levelNumber = levelNum,
            stars = stars,
            score = score
        });

        stateMachine.ChangeState(new LevelCompleteState());
    }

    private void OnLivesExhausted(LivesExhaustedEvent eventData)
    {
        Debug.Log("[GameManager] Game Over! No lives remaining.");
        stateMachine.ChangeState(new GameOverState());
    }

    public void StartLevel()
    {
        stateMachine.ChangeState(new PlayingState());
        LevelManager.Instance.LoadCurrentLevel();
        Debug.Log("[GameManager] Level started!");
    }

    public void PauseGame()
    {
        stateMachine.ChangeState(new PausedState());
        Debug.Log("[GameManager] Game paused!");
    }

    public void ResumeGame()
    {
        stateMachine.ChangeState(new PlayingState());
        Debug.Log("[GameManager] Game resumed!");
    }

    public void GoToMenu()
    {
        stateMachine.ChangeState(new MenuState());
        Debug.Log("[GameManager] Returned to menu!");
    }
}