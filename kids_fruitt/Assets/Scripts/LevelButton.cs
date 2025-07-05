using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI levelNumberText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Image completedIcon;
    [SerializeField] private GameObject priceContainer; // Container for price display
    [SerializeField] private TextMeshProUGUI priceText; // Price text for bonus levels

    [Header("Visual States")]
    [SerializeField] private Sprite unlockedBackground;
    [SerializeField] private Sprite lockedBackground;
    [SerializeField] private Sprite selectedBackground;
    [SerializeField] private Sprite bonusLevelBackground; // Special background for bonus levels

    [Header("Bonus Level Colors")]
    [SerializeField] private Color bonusLevelColor = new Color(1f, 0.89f, 0.35f, 1f); // #FFE35A
    [SerializeField] private Color bonusLevelTextColor = new Color(0.8f, 0.6f, 0f, 1f); // Darker gold for text

    private int levelIndex;
    private LevelData levelData;
    private bool isUnlocked;
    private bool isSelected;
    private bool isCompleted;
    private bool isBonusLevel;
    private bool isBonusLevelPurchased;

    public void SetupButton(int index, LevelData data, bool unlocked)
    {
        levelIndex = index;
        levelData = data;
        isUnlocked = unlocked;
        isBonusLevel = data.isBonusLevel;

        // Set the level number text
        if (isBonusLevel)
        {
            levelNumberText.text = $"BONUS\n{levelIndex + 1}";
            levelNumberText.color = bonusLevelTextColor;

            // Check if bonus level is purchased
            isBonusLevelPurchased = PlayerPrefs.GetInt($"BonusLevelPurchased_{levelIndex}", 0) == 1;

            // Setup price display
            if (priceContainer != null)
            {
                priceContainer.SetActive(!isBonusLevelPurchased);
                if (priceText != null)
                {
                    priceText.text = data.BonusLevelPrice.ToString();
                }
            }
        }
        else
        {
            levelNumberText.text = $"level\n{levelIndex + 1}";
            levelNumberText.color = Color.white;

            if (priceContainer != null)
                priceContainer.SetActive(false);
        }

        // Check if level is completed
        isCompleted = PlayerPrefs.GetInt($"LevelCompleted_{levelIndex}", 0) == 1;

        UpdateVisualState();
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        UpdateVisualState();
    }

    public void SetCompleted(bool completed)
    {
        isCompleted = completed;
        PlayerPrefs.SetInt($"LevelCompleted_{levelIndex}", completed ? 1 : 0);
        UpdateVisualState();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();
    }

    public void SetBonusLevelPurchased(bool purchased)
    {
        isBonusLevelPurchased = purchased;
        PlayerPrefs.SetInt($"BonusLevelPurchased_{levelIndex}", purchased ? 1 : 0);

        if (priceContainer != null)
            priceContainer.SetActive(!purchased);

        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        // Handle bonus level logic
        if (isBonusLevel)
        {
            HandleBonusLevelVisuals();
            return;
        }

        // Handle regular level logic
        HandleRegularLevelVisuals();
    }

    private void HandleBonusLevelVisuals()
    {
        // For bonus levels, they're always "unlocked" but may not be purchased
        lockIcon.gameObject.SetActive(false);
        button.interactable = true;

        if (isSelected)
        {
            backgroundImage.sprite = selectedBackground;
            backgroundImage.color = bonusLevelColor;
        }
        else
        {
            backgroundImage.sprite = bonusLevelBackground != null ? bonusLevelBackground : unlockedBackground;
            backgroundImage.color = bonusLevelColor;
        }

        // Show completed icon if level is completed
        if (isCompleted)
            completedIcon.gameObject.SetActive(true);
        else
            completedIcon.gameObject.SetActive(false);
    }

    private void HandleRegularLevelVisuals()
    {
        // Reset color for regular levels
        backgroundImage.color = Color.white;

        // Handle locked state
        if (!isUnlocked)
        {
            backgroundImage.sprite = lockedBackground;
            lockIcon.gameObject.SetActive(true);
            completedIcon.gameObject.SetActive(false);
            button.interactable = false;
            return;
        }

        // Handle unlocked states
        lockIcon.gameObject.SetActive(false);
        button.interactable = true;

        if (isSelected)
        {
            backgroundImage.sprite = selectedBackground;
        }
        else
        {
            backgroundImage.sprite = unlockedBackground;
        }

        // Show completed icon if level is completed
        if (PlayerPrefs.GetInt("HighestUnlockedLevel", 0) > levelIndex)
            completedIcon.gameObject.SetActive(true);
        else
            completedIcon.gameObject.SetActive(false);
    }

    public Button GetButton()
    {
        return button;
    }

    public int GetLevelIndex()
    {
        return levelIndex;
    }

    public bool IsBonusLevel()
    {
        return isBonusLevel;
    }

    public bool IsBonusLevelPurchased()
    {
        return isBonusLevelPurchased;
    }

    public int GetBonusLevelPrice()
    {
        return levelData.BonusLevelPrice;
    }
}