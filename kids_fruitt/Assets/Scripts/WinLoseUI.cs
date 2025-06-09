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

    private IEnumerator ShowWinScreenWithAnimation()
    {
        yield return new WaitForSeconds(winDelay);

        string randomWinMessage = winMessages[Random.Range(0, winMessages.Length)];
        winMessage.text = randomWinMessage;

        winScreen.SetActive(true);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(winScreenRect.DOScale(1f, animationDuration).SetEase(Ease.OutBack));

        sequence.Append(winMessage.DOFade(1f, animationDuration).SetEase(Ease.InQuad));

        foreach (var button in winButtons)
        {
            sequence.Append(button.DOScale(1f, animationDuration / 2).SetEase(Ease.OutBack));
        }
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
        int highestUnlockedLevel = PlayerPrefs.GetInt("HighestUnlockedLevel");

        if (highestUnlockedLevel < 15)
        {
            PlayerPrefs.SetString("SelectedLevelPrefab", $"Level {highestUnlockedLevel}");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            nextLevelButton.gameObject.SetActive(false);
        }
    }

    public void LoadMainManu()
    {
        SceneManager.LoadScene("MainManu");
    }
}