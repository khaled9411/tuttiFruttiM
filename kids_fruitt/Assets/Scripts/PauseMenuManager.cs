using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Image backgroundImage;
    public Button resumeButton;
    public Button mainMenuButton;

    [Header("Animation Settings")]
    public float showAnimationDuration = 0.5f;
    public float hideAnimationDuration = 0.3f;
    public float buttonDelayBetween = 0.1f;
    public Ease showEase = Ease.OutBack;
    public Ease hideEase = Ease.InBack;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pauseSound;
    public AudioClip buttonClickSound;

    private bool isPaused = false;
    private bool isAnimating = false;

    private Vector3 originalBackgroundScale;
    private Vector3 originalResumeButtonScale;
    private Vector3 originalMainMenuButtonScale;

    void Start()
    {
        InitializePauseMenu();
        SetupButtonListeners();
    }

    void Update()
    {
        // Check for pause input (ESC key or mobile back button)
        if (Input.GetKeyDown(KeyCode.Escape) && !isAnimating)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void InitializePauseMenu()
    {
        // Store original scales
        originalBackgroundScale = backgroundImage.transform.localScale;
        originalResumeButtonScale = resumeButton.transform.localScale;
        originalMainMenuButtonScale = mainMenuButton.transform.localScale;

        // Initially hide the pause menu
        pauseMenuPanel.SetActive(false);

        // Set initial scales to zero
        backgroundImage.transform.localScale = Vector3.zero;
        resumeButton.transform.localScale = Vector3.zero;
        mainMenuButton.transform.localScale = Vector3.zero;
    }

    void SetupButtonListeners()
    {
        resumeButton.onClick.AddListener(() => {
            PlayButtonSound();
            ResumeGame();
        });

        mainMenuButton.onClick.AddListener(() => {
            PlayButtonSound();
            GoToMainMenu();
        });
    }

    public void PauseGame()
    {
        if (isPaused || isAnimating) return;

        isPaused = true;
        isAnimating = true;

        // Pause the game
        Time.timeScale = 0f;

        // Play pause sound
        PlayPauseSound();

        // Show pause menu with animation
        pauseMenuPanel.SetActive(true);
        ShowPauseMenuAnimation();
    }

    public void ResumeGame()
    {
        if (!isPaused || isAnimating) return;

        isAnimating = true;

        // Hide pause menu with animation
        HidePauseMenuAnimation(() => {
            isPaused = false;
            isAnimating = false;
            pauseMenuPanel.SetActive(false);

            // Resume the game
            Time.timeScale = 1f;
        });
    }

    public void GoToMainMenu()
    {
        if (isAnimating) return;

        isAnimating = true;

        // Resume time scale before changing scene
        Time.timeScale = 1f;

        // Hide menu with animation then load main menu
        HidePauseMenuAnimation(() => {
            // Load main menu scene
            SceneManager.LoadScene("MainManu"); // Replace with your main menu scene name
        });
    }

    void ShowPauseMenuAnimation()
    {
        Sequence showSequence = DOTween.Sequence();
        showSequence.SetUpdate(true); // Important for paused game

        // Background scaling animation
        showSequence.Append(
            backgroundImage.transform.DOScale(originalBackgroundScale, showAnimationDuration)
                .SetEase(showEase)
        );

        // Resume button animation (appears after small delay)
        showSequence.Append(
            resumeButton.transform.DOScale(originalResumeButtonScale, showAnimationDuration * 0.8f)
                .SetEase(showEase)
                .SetDelay(buttonDelayBetween)
        );

        // Main menu button animation (appears after resume button)
        showSequence.Append(
            mainMenuButton.transform.DOScale(originalMainMenuButtonScale, showAnimationDuration * 0.8f)
                .SetEase(showEase)
                .SetDelay(buttonDelayBetween)
        );

        // Add some bounce effect to buttons
        showSequence.AppendCallback(() => {
            resumeButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 1, 0.5f).SetUpdate(true);
            mainMenuButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 1, 0.5f).SetUpdate(true).SetDelay(0.1f);
        });

        showSequence.OnComplete(() => {
            isAnimating = false;
        });
    }

    void HidePauseMenuAnimation(System.Action onComplete = null)
    {
        Sequence hideSequence = DOTween.Sequence();
        hideSequence.SetUpdate(true); // Important for paused game

        // Buttons disappear first (faster)
        hideSequence.Append(
            mainMenuButton.transform.DOScale(Vector3.zero, hideAnimationDuration)
                .SetEase(hideEase)
        );

        hideSequence.Join(
            resumeButton.transform.DOScale(Vector3.zero, hideAnimationDuration)
                .SetEase(hideEase)
                .SetDelay(buttonDelayBetween * 0.5f)
        );

        // Background disappears last
        hideSequence.Append(
            backgroundImage.transform.DOScale(Vector3.zero, hideAnimationDuration)
                .SetEase(hideEase)
        );

        hideSequence.OnComplete(() => {
            isAnimating = false;
            onComplete?.Invoke();
        });
    }

    void PlayPauseSound()
    {
        if (audioSource != null && pauseSound != null)
        {
            audioSource.PlayOneShot(pauseSound);
        }
    }

    void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    // Public method to pause from other scripts
    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    // Property to check if game is paused
    public bool IsPaused => isPaused;

    void OnDestroy()
    {
        // Clean up DOTween sequences
        DOTween.Kill(this);
    }
}