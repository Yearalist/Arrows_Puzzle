using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI totalStarsText;

    private void Start()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }

        UpdateStarsDisplay();
    }

    private void UpdateStarsDisplay()
    {
        int totalStars = LevelProgress.GetTotalStars();

        if (totalStarsText != null)
        {
            totalStarsText.text = $"★ {totalStars}";
        }
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}