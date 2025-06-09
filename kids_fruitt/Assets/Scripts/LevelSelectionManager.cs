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
    [SerializeField] private int levelsPerPage = 15;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Transform pageIndicatorsContainer;
    [SerializeField] private GameObject pageIndicatorPrefab;
    [SerializeField] private Color activePageColor = Color.white;
    [SerializeField] private Color inactivePageColor = new Color(0.7f, 0.7f, 0.9f, 1f);

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
            levelButton.SetupButton(i, levels[i], i <= highestUnlockedLevel);
            int index = i;
            levelButton.GetButton().onClick.AddListener(() => SelectLevel(index));
            buttonObj.SetActive(false); // Hide all initially
        }

        totalPages = Mathf.CeilToInt((float)levels.Count / levelsPerPage);
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
        if (highestUnlockedLevel >= 0 && highestUnlockedLevel < levels.Count)
        {
            SelectLevel(highestUnlockedLevel);
            // Show the page containing this level
            int pageIndex = highestUnlockedLevel / levelsPerPage;
            ShowPage(pageIndex);
        }
    }

    private void SelectLevel(int index)
    {
        // Don't allow selecting locked levels
        if (index > highestUnlockedLevel)
            return;

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
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    public void UnlockNextLevel(int completedLevelIndex)
    {
        int nextLevelIndex = completedLevelIndex + 1;
        if (nextLevelIndex > highestUnlockedLevel && nextLevelIndex < levels.Count)
        {
            highestUnlockedLevel = nextLevelIndex;
            PlayerPrefs.SetInt("HighestUnlockedLevel", highestUnlockedLevel);
            PlayerPrefs.Save();

            // Update UI to show newly unlocked level
            if (nextLevelIndex < levelButtons.Count)
            {
                LevelButton levelButton = levelButtons[nextLevelIndex].GetComponent<LevelButton>();
                levelButton.SetUnlocked(true);
            }
        }
    }
}