using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private int totalLevels = 50;

    private void Start()
    {
        CreateLevelButtons();
    }

    private void CreateLevelButtons()
    {
        for (int i = 1; i <= totalLevels; i++)
        {
            CreateLevelButton(i);
        }
    }

    private void CreateLevelButton(int levelNumber)
    {
        GameObject buttonObj = Instantiate(levelButtonPrefab, contentParent);
        buttonObj.name = $"Level_{levelNumber}";

        // Get components
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI levelText = buttonObj.transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI starsText = buttonObj.transform.Find("StarsText").GetComponent<TextMeshProUGUI>();
        Image bgImage = buttonObj.GetComponent<Image>();

        // Set level number
        levelText.text = levelNumber.ToString();

        // Check if unlocked
        bool isUnlocked = LevelProgress.IsLevelUnlocked(levelNumber);
        int stars = LevelProgress.GetLevelStars(levelNumber);

        if (isUnlocked)
        {
            // Unlocked - show stars
            bgImage.color = Color.white;
            button.interactable = true;

            string starDisplay = "";
            for (int i = 0; i < 3; i++)
            {
                starDisplay += i < stars ? "★" : "☆";
            }
            starsText.text = starDisplay;
            starsText.color = stars > 0 ? new Color(0.9f, 0.75f, 0.2f) : new Color(0.7f, 0.7f, 0.7f);

            int capturedLevel = levelNumber;
            button.onClick.AddListener(() => OnLevelSelected(capturedLevel));
        }
        else
        {
            // Locked
            bgImage.color = new Color(0.85f, 0.85f, 0.88f);
            button.interactable = false;
            levelText.color = new Color(0.6f, 0.6f, 0.65f);
            starsText.text = "🔒";
        }
    }

    private void OnLevelSelected(int levelNumber)
    {
        // Store selected level for Game scene to read
        PlayerPrefs.SetInt("SelectedLevel", levelNumber - 1);
        SceneManager.LoadScene("Game");
    }

    public void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}