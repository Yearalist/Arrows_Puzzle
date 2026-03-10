using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompletePanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        HidePanel();

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextClicked);
        }
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryClicked);
        }
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuClicked);
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
    }

    private void OnLevelCompleted(LevelCompletedEvent eventData)
    {
        ShowPanel(eventData.levelNumber, eventData.score, eventData.stars);
    }

    public void ShowPanel(int levelNumber, int score, int stars)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (levelText != null)
        {
            levelText.text = $"Level {levelNumber} Complete!";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }

        if (starsText != null)
        {
            string starDisplay = "";
            for (int i = 0; i < 3; i++)
            {
                starDisplay += i < stars ? "★ " : "☆ ";
            }
            starsText.text = starDisplay;
        }
    }

    public void HidePanel()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnNextClicked()
    {
        HidePanel();
        LevelManager.Instance.LoadNextLevel();
        GameManager.Instance.StateMachine.ChangeState(new PlayingState());
    }

    private void OnRetryClicked()
    {
        HidePanel();
        LevelManager.Instance.RestartCurrentLevel();
        GameManager.Instance.StateMachine.ChangeState(new PlayingState());
    }

    private void OnMenuClicked()
    {
        HidePanel();
        GameManager.Instance.GoToMenu();
    }
}