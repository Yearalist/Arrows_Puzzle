using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<LevelStartedEvent>(OnLevelStarted);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<LevelStartedEvent>(OnLevelStarted);
    }

    private void OnLevelStarted(LevelStartedEvent eventData)
    {
        UpdateLevelText(eventData.levelNumber);
    }

    private void UpdateLevelText(int levelNumber)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {levelNumber}";
        }
    }

    private void OnBackClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f;
        LevelManager.Instance.RestartCurrentLevel();
        GameManager.Instance.StateMachine.ChangeState(new PlayingState());
    }
}