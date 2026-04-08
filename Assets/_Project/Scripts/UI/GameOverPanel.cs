using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        HidePanel();

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
        EventBus.Subscribe<LivesExhaustedEvent>(OnLivesExhausted);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<LivesExhaustedEvent>(OnLivesExhausted);
    }

    private void OnLivesExhausted(LivesExhaustedEvent eventData)
    {
        ShowPanel();
    }

    public void ShowPanel()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (gameOverText != null)
        {
            gameOverText.text = "Game Over!\nNo lives remaining.";
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

    private void OnRetryClicked()
    {
        HidePanel();
        LevelManager.Instance.RestartCurrentLevel();
        GameManager.Instance.StateMachine.ChangeState(new PlayingState());
    }

    private void OnMenuClicked()
    {
        HidePanel();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelect");
    }
}