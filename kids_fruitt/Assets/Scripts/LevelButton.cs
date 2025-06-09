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
    //[SerializeField] private GameObject progressIndicator;
    //[SerializeField] private TextMeshProUGUI progressText;

    [Header("Visual States")]
    //[SerializeField] private Color unlockedColor = Color.white;
    //[SerializeField] private Color lockedColor = Color.gray;
    //[SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Sprite unlockedBackground;
    [SerializeField] private Sprite lockedBackground;
    [SerializeField] private Sprite selectedBackground;

    private int levelIndex;
    private LevelData levelData;
    private bool isUnlocked;
    private bool isSelected;
    private bool isCompleted;

    public void SetupButton(int index, LevelData data, bool unlocked)
    {
        levelIndex = index;
        levelData = data;
        isUnlocked = unlocked;

        // Set the level number text
        levelNumberText.text = $"level\n{levelIndex + 1}";

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

    //public void SetProgress(float progressPercentage)
    //{
    //    if (progressIndicator != null && progressText != null)
    //    {
    //        // Only show progress for in-progress levels (not completed, not locked)
    //        bool showProgress = isUnlocked && !isCompleted && progressPercentage > 0;
    //        progressIndicator.SetActive(showProgress);

    //        if (showProgress)
    //        {
    //            progressText.text = $"{Mathf.RoundToInt(progressPercentage)}%";
    //        }
    //    }
    //}

    private void UpdateVisualState()
    {
        // Handle locked state
        if (!isUnlocked)
        {
            // Locked state
            backgroundImage.sprite = lockedBackground;
            //backgroundImage.color = lockedColor;
            lockIcon.gameObject.SetActive(true);
            completedIcon.gameObject.SetActive(false);
            //progressIndicator?.SetActive(false);
            button.interactable = false;
            return;
        }

        // Handle unlocked states
        lockIcon.gameObject.SetActive(false);
        button.interactable = true;

        if (isSelected)
        {
            // Selected state
            backgroundImage.sprite = selectedBackground;
            //backgroundImage.color = selectedColor;
        }
        else
        {
            // Normal unlocked state
            backgroundImage.sprite = unlockedBackground;
            //backgroundImage.color = unlockedColor;
        }

        // Show completed icon if level is completed
        if(PlayerPrefs.GetInt("HighestUnlockedLevel", 0) > levelIndex)
            completedIcon.gameObject.SetActive(true);

        // Hide progress indicator if completed or selected
        //if (progressIndicator != null)
        //{
        //    progressIndicator.SetActive(!isCompleted && !isSelected && levelData.progress > 0);
        //}
    }

    public Button GetButton()
    {
        return button;
    }

    public int GetLevelIndex()
    {
        return levelIndex;
    }
}