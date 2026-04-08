using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private int totalLevels = 50;
    [SerializeField] private Sprite starFilledSprite;
    [SerializeField] private Sprite starEmptySprite;
    [SerializeField] private Sprite lockSprite;

    private void Start()
    {
        CreateLevelButtons();

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    private void CreateLevelButtons()
    {
        // Eski butonları temizle
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 1; i <= totalLevels; i++)
        {
            CreateLevelButton(i);
        }
    }

    private void CreateLevelButton(int levelNumber)
    {
        GameObject buttonObj = Instantiate(levelButtonPrefab, contentParent);
        buttonObj.name = $"Level_{levelNumber}";

        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI levelText = buttonObj.transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
        Image bgImage = buttonObj.GetComponent<Image>();

        bool isUnlocked = LevelProgress.IsLevelUnlocked(levelNumber);
        int stars = LevelProgress.GetLevelStars(levelNumber);

        if (isUnlocked)
        {
            bgImage.color = Color.white;
            button.interactable = true;
            levelText.text = levelNumber.ToString();
            levelText.color = new Color(0.12f, 0.12f, 0.18f);

            // Yıldızları güncelle
            Transform starsContainer = buttonObj.transform.Find("StarsContainer");
            if (starsContainer != null)
            {
                Image[] starImages = starsContainer.GetComponentsInChildren<Image>();
                for (int i = 0; i < starImages.Length && i < 3; i++)
                {
                    if (i < stars)
                    {
                        starImages[i].sprite = starFilledSprite;
                        starImages[i].color = new Color(1f, 0.72f, 0f);
                    }
                    else
                    {
                        starImages[i].sprite = starEmptySprite;
                        starImages[i].color = new Color(0.75f, 0.75f, 0.75f);
                    }
                }
            }

            int capturedLevel = levelNumber;
            button.onClick.AddListener(() => OnLevelSelected(capturedLevel));
        }
        else
        {
            bgImage.color = new Color(0.88f, 0.88f, 0.9f);
            button.interactable = false;
            levelText.text = "";

            // Kilit ikonu göster
            Transform starsContainer = buttonObj.transform.Find("StarsContainer");
            if (starsContainer != null)
            {
                starsContainer.gameObject.SetActive(false);
            }

            // Level numarası yerine kilit göster
            levelText.text = "?";
            levelText.color = new Color(0.6f, 0.6f, 0.65f);
        }
    }

    private void OnLevelSelected(int levelNumber)
    {
        PlayerPrefs.SetInt("SelectedLevel", levelNumber - 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Game");
    }

    public void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}