using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using TMPro;


public class WinLoseUI : MonoBehaviour
{
    // UI elements
    public GameObject winScreen;
    public GameObject loseScreen;
    public TMP_Text winMessage;
    public TMP_Text loseMessage;
    public Button retryButton;
    public Button nextLevelButton;
    public Button winMainMenuButton;
    public Button loseMainMenuButton;

    public RectTransform loseScreenRect;
    public RectTransform winScreenRect;
    public RectTransform[] loseButtons;
    public RectTransform[] winButtons;

    [SerializeField] private float winDelay = 0.5f;
    [SerializeField] private float loseDelay = 1.5f;

    [SerializeField] private float animationDuration = 0.5f;

    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;
    private AudioSource audioSource;

    // Current level tracking
    private int currentLevelIndex = -1;
    private int highestUnlockedLevel = 0;

    // Messages
    private string[] winMessages = {
        "Awesome! You are a real hero!",
        "Well done! You are amazing!",
        "What a great win!",
        "You are a shining star!"
    };

    private string[] loseMessages = {
        "It's okay! You can try again!",
        "Be brave and try again!",
        "You are learning and improving! Try again!",
        "Don't give up! You can succeed!"
    };

    private void Awake()
    {

        if (loseScreenRect == null)
            loseScreenRect = loseScreen.GetComponent<RectTransform>();

        if (winScreenRect == null)
            winScreenRect = winScreen.GetComponent<RectTransform>();

        if (loseScreenRect != null)
            loseScreenRect.localScale = Vector3.zero;

        if (winScreenRect != null)
            winScreenRect.localScale = Vector3.zero;

        audioSource = GetComponent<AudioSource>();

        // Load current level index from PlayerPrefs
        currentLevelIndex = PlayerPrefs.GetInt("SelectedLevelIndex", -1);
        highestUnlockedLevel = PlayerPrefs.GetInt("HighestUnlockedLevel", 0);
    }

    private void Start()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        retryButton.onClick.AddListener(RetryLevel);
        nextLevelButton.onClick.AddListener(NextLevel);
        loseMainMenuButton.onClick.AddListener(LoadMainManu);
        winMainMenuButton.onClick.AddListener(LoadMainManu);

        DieEvent.Instance.onPlayerDie += Lose;
        WinEvent.Instance.onPlayerWin += Win;

        if (winMessage != null)
            winMessage.color = new Color(winMessage.color.r, winMessage.color.g, winMessage.color.b, 0);

        if (loseMessage != null)
            loseMessage.color = new Color(loseMessage.color.r, loseMessage.color.g, loseMessage.color.b, 0);

        foreach (var button in loseButtons)
        {
            if (button != null)
                button.localScale = Vector3.zero;
        }

        foreach (var button in winButtons)
        {
            if (button != null)
                button.localScale = Vector3.zero;
        }
    }

    public void Win()
    {
        audioSource.clip = winClip;
        audioSource.Play();

        // Only unlock next level if player won the highest unlocked level
        // and it's not a bonus level
        if (currentLevelIndex == highestUnlockedLevel && !IsCurrentLevelBonus())
        {
            UnlockNextLevel();
        }

        StartCoroutine(ShowWinScreenWithAnimation());
        // AudioManager.Instance.PlayWinSound();
    }

    public void Lose()
    {
        audioSource.clip = loseClip;
        audioSource.Play();

        StartCoroutine(ShowLoseScreenWithAnimation());

        // AudioManager.Instance.PlayLoseSound();
    }

    private bool IsCurrentLevelBonus()
    {
        // Check if current level is a bonus level
        // This assumes bonus levels are marked with a specific naming convention
        // or you can implement a more sophisticated check based on your level data
        string selectedLevelName = PlayerPrefs.GetString("SelectedLevelPrefab", "");
        return selectedLevelName.Contains("Bonus") || selectedLevelName.Contains("bonus");
    }

    private void UnlockNextLevel()
    {
        int nextLevelIndex = currentLevelIndex + 1;

        // Check if there's a next level and it's currently locked
        if (nextLevelIndex > highestUnlockedLevel)
        {
            // Find the next level to unlock, skipping bonus levels
            int levelToUnlock = FindNextLevelToUnlock(nextLevelIndex);

            if (levelToUnlock != -1 && levelToUnlock <= 29) // Make sure we don't exceed total levels
            {
                PlayerPrefs.SetInt("HighestUnlockedLevel", levelToUnlock);
                PlayerPrefs.Save();
                highestUnlockedLevel = levelToUnlock;

                Debug.Log($"Level {currentLevelIndex} completed. Unlocked level {levelToUnlock}");
            }
        }
    }

    private int FindNextLevelToUnlock(int startIndex)
    {
        int levelToUnlock = startIndex;

        // If the next level is a bonus level, skip it and unlock the level after it
        if (IsLevelBonus(startIndex))
        {
            levelToUnlock = startIndex + 1;
            Debug.Log($"Skipping bonus level {startIndex}, unlocking level {levelToUnlock} instead");
        }

        // Make sure we don't go beyond the maximum level count
        if (levelToUnlock > 29)
        {
            return -1; // No more levels to unlock
        }

        return levelToUnlock;
    }

    private bool IsLevelBonus(int levelIndex)
    {
        // This is a simple check - you might want to make this more sophisticated
        // based on your actual level data structure
        return PlayerPrefs.GetInt($"Level_{levelIndex}_IsBonus", 0) == 1;
    }

    private IEnumerator ShowWinScreenWithAnimation()
    {
        yield return new WaitForSeconds(winDelay);

        string randomWinMessage = winMessages[Random.Range(0, winMessages.Length)];
        winMessage.text = randomWinMessage;

        winScreen.SetActive(true);

        // Check if Next Level button should be visible
        UpdateNextLevelButtonVisibility();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(winScreenRect.DOScale(1f, animationDuration).SetEase(Ease.OutBack));

        sequence.Append(winMessage.DOFade(1f, animationDuration).SetEase(Ease.InQuad));

        foreach (var button in winButtons)
        {
            sequence.Append(button.DOScale(1f, animationDuration / 2).SetEase(Ease.OutBack));
        }
    }

    private void UpdateNextLevelButtonVisibility()
    {
        // Hide Next Level button if:
        // 1. Current level is a bonus level
        // 2. There's no next regular level available
        // 3. Player hasn't reached the current level as their highest unlocked level

        if (IsCurrentLevelBonus())
        {
            nextLevelButton.gameObject.SetActive(false);
            return;
        }

        int nextRegularLevelIndex = GetNextRegularLevelIndex();

        if (nextRegularLevelIndex == -1 || nextRegularLevelIndex > 29) // No next regular level
        {
            nextLevelButton.gameObject.SetActive(false);
        }
        else
        {
            nextLevelButton.gameObject.SetActive(true);
        }
    }

    private int GetNextRegularLevelIndex()
    {
        int nextIndex = currentLevelIndex + 1;

        // Skip bonus levels
        while (nextIndex <= 29 && IsLevelBonus(nextIndex))
        {
            nextIndex++;
        }

        return nextIndex <= 29 ? nextIndex : -1;
    }

    private IEnumerator ShowLoseScreenWithAnimation()
    {
        yield return new WaitForSeconds(loseDelay);

        string randomLoseMessage = loseMessages[Random.Range(0, loseMessages.Length)];
        loseMessage.text = randomLoseMessage;

        loseScreen.SetActive(true);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(loseScreenRect.DOScale(1f, animationDuration).SetEase(Ease.OutBack));

        sequence.Append(loseMessage.DOFade(1f, animationDuration).SetEase(Ease.InQuad));

        foreach (var button in loseButtons)
        {
            sequence.Append(button.DOScale(1f, animationDuration / 2).SetEase(Ease.OutBack));
        }
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        // Don't proceed if current level is a bonus level
        if (IsCurrentLevelBonus())
        {
            return;
        }

        int nextRegularLevelIndex = GetNextRegularLevelIndex();

        if (nextRegularLevelIndex != -1 && nextRegularLevelIndex <= highestUnlockedLevel)
        {
            // Set the next level as selected
            PlayerPrefs.SetString("SelectedLevelPrefab", $"Level {nextRegularLevelIndex}");
            PlayerPrefs.SetInt("SelectedLevelIndex", nextRegularLevelIndex);
            PlayerPrefs.Save();

            // Reload the scene to play the next level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void LoadMainManu()
    {
        SceneManager.LoadScene("MainManu");
    }
}