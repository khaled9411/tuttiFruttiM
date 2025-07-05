using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform levelButtonsContainer;

    [Header("UI Settings")]
    [SerializeField] private Button playButton;
    [SerializeField] private string gameplaySceneName = "LevelScene";
    [SerializeField] private string bonusLevelSceneName = "BonusLevelScene";
    [SerializeField] private int levelsPerPage = 15;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Transform pageIndicatorsContainer;
    [SerializeField] private GameObject pageIndicatorPrefab;
    [SerializeField] private Color activePageColor = Color.white;
    [SerializeField] private Color inactivePageColor = new Color(0.7f, 0.7f, 0.9f, 1f);

    [Header("Bonus Level Purchase")]
    [SerializeField] private BonusPurchaseDialog bonusPurchaseDialog;

    private int selectedLevelIndex = -1;
    private int highestUnlockedLevel = 0;
    private int currentPageIndex = 0;
    private int totalPages;
    private List<GameObject> pageIndicators = new List<GameObject>();
    private List<GameObject> levelButtons = new List<GameObject>();

    private void Start()
    {
        LoadPlayerProgress();
        InitializeLevelButtons();
        InitializePageIndicators();
        SetupNavigationButtons();
        SelectLatestUnlockedLevel();
        ShowCurrentPage();
        playButton.onClick.AddListener(LoadSelectedLevel);
    }

    private void LoadPlayerProgress()
    {
        highestUnlockedLevel = PlayerPrefs.GetInt("HighestUnlockedLevel", 0);
    }

    private void InitializeLevelButtons()
    {
        // Clear existing buttons if any
        foreach (Transform child in levelButtonsContainer)
        {
            Destroy(child.gameObject);
        }
        levelButtons.Clear();

        // Create level buttons for all levels
        for (int i = 0; i < levels.Count; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonsContainer);
            levelButtons.Add(buttonObj);
            LevelButton levelButton = buttonObj.GetComponent<LevelButton>();

            // Store bonus level info in PlayerPrefs for easier access
            if (levels[i].isBonusLevel)
            {
                PlayerPrefs.SetInt($"Level_{i}_IsBonus", 1);
            }
            else
            {
                PlayerPrefs.SetInt($"Level_{i}_IsBonus", 0);
            }

            // For bonus levels, check if they're unlocked based on previous levels
            bool isUnlocked = IsLevelUnlocked(i);
            levelButton.SetupButton(i, levels[i], isUnlocked);

            int index = i;
            levelButton.GetButton().onClick.AddListener(() => OnLevelButtonClicked(index));
            buttonObj.SetActive(false); // Hide all initially
        }

        // Save the PlayerPrefs after setting up all levels
        PlayerPrefs.Save();

        totalPages = Mathf.CeilToInt((float)levels.Count / levelsPerPage);
    }

    private bool IsLevelUnlocked(int levelIndex)
    {
        if (levels[levelIndex].isBonusLevel)
        {
            // Bonus levels are unlocked if the player has reached them
            // This means they completed the level before the bonus level
            return levelIndex <= highestUnlockedLevel;
        }
        else
        {
            // Regular levels follow normal unlock logic
            return levelIndex <= highestUnlockedLevel;
        }
    }

    private void OnLevelButtonClicked(int index)
    {
        LevelButton clickedButton = levelButtons[index].GetComponent<LevelButton>();

        if (clickedButton.IsBonusLevel())
        {
            HandleBonusLevelClick(index, clickedButton);
        }
        else
        {
            HandleRegularLevelClick(index);
        }
    }

    private void HandleBonusLevelClick(int index, LevelButton bonusButton)
    {
        // Check if level is unlocked first
        if (index > highestUnlockedLevel)
        {
            NotificationManager.Instance.ShowNotification("Complete previous levels first!", Color.red);
            return;
        }

        if (!bonusButton.IsBonusLevelPurchased())
        {
            // Show purchase dialog
            bonusPurchaseDialog.ShowDialog(index, bonusButton.GetBonusLevelPrice(), (purchased) =>
            {
                if (purchased)
                {
                    // Purchase successful
                    bonusButton.SetBonusLevelPurchased(true);
                    SelectLevel(index);
                    NotificationManager.Instance.ShowNotification("Bonus level purchased!", Color.green);
                }
            });
        }
        else
        {
            // Already purchased, select it
            SelectLevel(index);
        }
    }

    private void HandleRegularLevelClick(int index)
    {
        SelectLevel(index);
    }

    private void InitializePageIndicators()
    {
        // Clear existing indicators
        foreach (Transform child in pageIndicatorsContainer)
        {
            Destroy(child.gameObject);
        }
        pageIndicators.Clear();

        // Create page indicators
        for (int i = 0; i < totalPages; i++)
        {
            GameObject indicator = Instantiate(pageIndicatorPrefab, pageIndicatorsContainer);
            pageIndicators.Add(indicator);
            Image indicatorImage = indicator.GetComponent<Image>();
            Button indicatorButton = indicator.GetComponent<Button>();

            // Set color based on if it's the current page
            indicatorImage.color = (i == currentPageIndex) ? activePageColor : inactivePageColor;

            // Add click event
            int pageIndex = i;
            indicatorButton.onClick.AddListener(() => ShowPage(pageIndex));
        }
    }

    private void SetupNavigationButtons()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);

        if (prevButton != null)
            prevButton.onClick.AddListener(PrevPage);

        UpdateNavigationButtonsState();
    }

    private void UpdateNavigationButtonsState()
    {
        if (prevButton != null)
            prevButton.interactable = (currentPageIndex > 0);

        if (nextButton != null)
            nextButton.interactable = (currentPageIndex < totalPages - 1);
    }

    private void ShowPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= totalPages)
            return;

        currentPageIndex = pageIndex;

        // Hide all level buttons
        foreach (GameObject button in levelButtons)
        {
            button.SetActive(false);
        }

        // Show only the buttons for the current page
        int startIndex = currentPageIndex * levelsPerPage;
        int endIndex = Mathf.Min(startIndex + levelsPerPage, levels.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            levelButtons[i].SetActive(true);
        }

        // Update page indicators
        for (int i = 0; i < pageIndicators.Count; i++)
        {
            Image indicatorImage = pageIndicators[i].GetComponent<Image>();
            indicatorImage.color = (i == currentPageIndex) ? activePageColor : inactivePageColor;
        }

        UpdateNavigationButtonsState();
    }

    private void ShowCurrentPage()
    {
        ShowPage(currentPageIndex);
    }

    private void NextPage()
    {
        if (currentPageIndex < totalPages - 1)
        {
            ShowPage(currentPageIndex + 1);
        }
    }

    private void PrevPage()
    {
        if (currentPageIndex > 0)
        {
            ShowPage(currentPageIndex - 1);
        }
    }

    private void SelectLatestUnlockedLevel()
    {
        // Find the latest unlocked regular level (not bonus)
        int latestRegularLevel = -1;
        for (int i = highestUnlockedLevel; i >= 0; i--)
        {
            if (i < levels.Count && !levels[i].isBonusLevel)
            {
                latestRegularLevel = i;
                break;
            }
        }

        // If no regular level found, find the highest unlocked level (even if bonus)
        if (latestRegularLevel == -1)
        {
            for (int i = highestUnlockedLevel; i >= 0; i--)
            {
                if (i < levels.Count)
                {
                    latestRegularLevel = i;
                    break;
                }
            }
        }

        if (latestRegularLevel >= 0)
        {
            SelectLevel(latestRegularLevel);
            // Show the page containing this level
            int pageIndex = latestRegularLevel / levelsPerPage;
            ShowPage(pageIndex);
        }
    }

    private void SelectLevel(int index)
    {
        // For bonus levels, check if they're purchased
        if (levels[index].isBonusLevel)
        {
            LevelButton bonusButton = levelButtons[index].GetComponent<LevelButton>();
            if (!bonusButton.IsBonusLevelPurchased())
            {
                return; // Don't select unpurchased bonus levels
            }
        }
        else
        {
            // Don't allow selecting locked regular levels
            if (index > highestUnlockedLevel)
                return;
        }

        if (selectedLevelIndex >= 0 && selectedLevelIndex < levelButtons.Count)
        {
            LevelButton prevButton = levelButtons[selectedLevelIndex].GetComponent<LevelButton>();
            prevButton.SetSelected(false);
        }

        selectedLevelIndex = index;

        if (selectedLevelIndex >= 0 && selectedLevelIndex < levelButtons.Count)
        {
            LevelButton newButton = levelButtons[selectedLevelIndex].GetComponent<LevelButton>();
            newButton.SetSelected(true);
            playButton.interactable = true;
        }
    }

    private void LoadSelectedLevel()
    {
        if (selectedLevelIndex >= 0)
        {
            PlayerPrefs.SetString("SelectedLevelPrefab", levels[selectedLevelIndex].levelName);
            PlayerPrefs.SetInt("SelectedLevelIndex", selectedLevelIndex);
            PlayerPrefs.Save();

            // Load appropriate scene based on level type
            if (levels[selectedLevelIndex].isBonusLevel)
            {
                SceneManager.LoadScene(bonusLevelSceneName);
            }
            else
            {
                SceneManager.LoadScene(gameplaySceneName);
            }
        }
    }

    // This method is called when a level is completed
    // It should only be called from the game scene, not from here
    public void UnlockNextLevel(int completedLevelIndex)
    {
        // This method is now deprecated - level unlocking is handled in WinLoseUI
        // Keeping it for backwards compatibility but it won't be used
        Debug.LogWarning("UnlockNextLevel is deprecated. Level unlocking is now handled in WinLoseUI.");
    }

    // Helper method to check if a bonus level is available for purchase
    public bool CanPurchaseBonusLevel(int levelIndex)
    {
        if (levelIndex >= levels.Count || !levels[levelIndex].isBonusLevel)
            return false;

        // Check if player has reached this level
        if (levelIndex > highestUnlockedLevel)
            return false;

        // Check if already purchased
        return PlayerPrefs.GetInt($"BonusLevelPurchased_{levelIndex}", 0) == 0;
    }

    // Method to refresh the UI when returning from a completed level
    private void OnEnable()
    {
        // Refresh the progress when returning to this scene
        LoadPlayerProgress();

        // Update button states based on new progress
        for (int i = 0; i < levelButtons.Count; i++)
        {
            if (levelButtons[i] != null)
            {
                LevelButton levelButton = levelButtons[i].GetComponent<LevelButton>();
                bool isUnlocked = IsLevelUnlocked(i);
                levelButton.SetUnlocked(isUnlocked);
            }
        }
    }
}