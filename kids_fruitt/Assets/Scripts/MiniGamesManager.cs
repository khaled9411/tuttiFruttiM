using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class MiniGamesManager : MonoBehaviour
{
    public static MiniGamesManager instance;

    [Header("General UI")]
    [SerializeField] private RectTransform gamePanel;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Guessing game")]
    [SerializeField] private GameObject guessingGamePanel;
    [SerializeField] private GameObject[] gridButtons = new GameObject[9];
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite coinSprite;
    [SerializeField] private Sprite keySprite;
    [SerializeField] private Sprite xSprite;

    [Header("One try game")]
    [SerializeField] private GameObject oneChanceGamePanel;
    [SerializeField] private GameObject[] itemButtons;

    private int playerCoins = 0;
    private int attemptsLeft = 3;
    private bool isGameActive = false;

    private float coinProbability = 0.7f; //  70%
    //private float xProbability = 0.25f;   // 25%
    private float keyProbability = 0.05f; // 5%

    private float winProbability = 0.4f; // 40%

    // Add this to kill all tweens when needed
    private void KillAllTweens()
    {
        //DOTween.KillAll();
    }

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        gamePanel.localScale = Vector3.zero;
        guessingGamePanel.SetActive(false);
        oneChanceGamePanel.SetActive(false);
        messageText.gameObject.SetActive(false);
    }

    public void OpenGameBox(bool isRandom, int gameIndex = 0)
    {
        // Kill any existing tweens before starting new ones
        KillAllTweens();

        int selectedGame = isRandom ? Random.Range(0, 2) : gameIndex;

        if (selectedGame == 0)
            StartGuessingGame();
        else
            StartOneChanceGame();
    }

    private void StartGuessingGame()
    {
        // Reset everything first
        isGameActive = true;
        attemptsLeft = 3;
        playerCoins = 0;

        // Setup UI
        messageText.gameObject.SetActive(true);
        messageText.text = "You have 3 attempts. Select a square to reveal what is inside it!";

        // Reset button states
        for (int i = 0; i < gridButtons.Length; i++)
        {
            if (gridButtons[i] != null)
            {
                // Reset button scale
                gridButtons[i].transform.localScale = Vector3.zero;

                // Enable button interaction
                Button btn = gridButtons[i].GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = true;
                }

                // Reset image
                if (gridButtons[i].transform.childCount > 0)
                {
                    Image img = gridButtons[i].transform.GetChild(0).GetComponent<Image>();
                    if (img != null)
                    {
                        img.sprite = defaultSprite;
                        img.color = new Color(1, 1, 1, 1);
                    }
                }
            }
        }

        // Activate panel
        guessingGamePanel.SetActive(true);

        // Show the game panel with animation
        ShowGamePanel(guessingGamePanel);
    }

    private void StartOneChanceGame()
    {
        // Reset state
        isGameActive = true;
        playerCoins = 0;

        // Setup UI
        messageText.gameObject.SetActive(true);
        messageText.text = "You only have one try! Pick an item to win 1000 coins!";

        // Reset buttons
        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (itemButtons[i] != null)
            {
                // Reset scale
                itemButtons[i].transform.localScale = Vector3.zero;

                // Enable interaction
                Button btn = itemButtons[i].GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = true;
                }
            }
        }

        // Activate panel
        oneChanceGamePanel.SetActive(true);

        // Show panel with animation
        ShowGamePanel(oneChanceGamePanel);
    }

    private void ShowGamePanel(GameObject panel)
    {
        // Ensure correct panel state
        guessingGamePanel.SetActive(panel == guessingGamePanel);
        oneChanceGamePanel.SetActive(panel == oneChanceGamePanel);
        panel.SetActive(true);

        // Animate panel scale
        gamePanel.localScale = Vector3.zero;

        // Create a sequence for the panel animation
        Sequence panelSequence = DOTween.Sequence();
        panelSequence.Append(gamePanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));

        // After panel animation, animate buttons
        panelSequence.OnComplete(() => {
            if (panel == guessingGamePanel)
            {
                StartCoroutine(AnimateGridButtons());
            }
            else if (panel == oneChanceGamePanel)
            {
                StartCoroutine(AnimateItemButtons());
            }
        });
    }

    private IEnumerator AnimateGridButtons()
    {
        // Animate each button one by one
        for (int i = 0; i < gridButtons.Length; i++)
        {
            if (gridButtons[i] != null)
            {
                float duration = 0.3f;
                gridButtons[i].transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Wait a bit to ensure all animations are complete
        yield return new WaitForSeconds(0.3f);

        // Enable all buttons after animations
        foreach (GameObject button in gridButtons)
        {
            if (button != null)
            {
                Button btn = button.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = true;
                }
            }
        }
    }

    private IEnumerator AnimateItemButtons()
    {
        // Animate each button one by one
        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (itemButtons[i] != null)
            {
                float duration = 0.3f;
                itemButtons[i].transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Wait a bit to ensure all animations are complete
        yield return new WaitForSeconds(0.3f);

        // Enable all buttons after animations
        foreach (GameObject button in itemButtons)
        {
            if (button != null)
            {
                Button btn = button.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = true;
                }
            }
        }
    }

    public void OnGridButtonClick(int buttonIndex)
    {
        if (!isGameActive || attemptsLeft <= 0 || buttonIndex >= gridButtons.Length)
            return;

        attemptsLeft--;

        // Get button and disable it
        Button btn = gridButtons[buttonIndex].GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = false;
        }

        // Determine result based on probability
        float randomValue = Random.value;

        // Get image reference safely
        Image buttonImage = null;
        if (gridButtons[buttonIndex].transform.childCount > 0)
        {
            buttonImage = gridButtons[buttonIndex].transform.GetChild(0).GetComponent<Image>();
        }

        if (buttonImage == null)
        {
            Debug.LogError("Button image not found on child of grid button " + buttonIndex);
            return;
        }

        // Set result based on probability
        if (randomValue < keyProbability)
        {
            // Key result (1000 coins)
            buttonImage.sprite = keySprite;
            playerCoins += 1000;
            CurrencyManager.Instance.AddCoins(1000);
            ShowRewardAnimation(gridButtons[buttonIndex].transform, "1000!");
            messageText.text = "Awesome! You got a key! +1000 coins";
        }
        else if (randomValue < keyProbability + coinProbability)
        {
            // Coin result (100 coins)
            buttonImage.sprite = coinSprite;
            playerCoins += 100;
            CurrencyManager.Instance.AddCoins(100);
            ShowRewardAnimation(gridButtons[buttonIndex].transform, "100!");
            messageText.text = "Good! You got a coin! +100 coins";
        }
        else
        {
            // X result (no coins)
            buttonImage.sprite = xSprite;
            ShowRewardAnimation(gridButtons[buttonIndex].transform, "X");
            messageText.text = "Unfortunately! You didn't win anything this time.";
        }

        // Animate the revealed image
        buttonImage.color = Color.white;
        buttonImage.transform.localScale = Vector3.zero;

        // Create a separate tween for the button animation
        Tween revealTween = buttonImage.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        // Check if game is over
        if (attemptsLeft <= 0)
        {
            StartCoroutine(EndGameAfterDelay(1.5f));
            messageText.text = "Trials are over! You've won " + playerCoins + " coins!";
        }
        else
        {
            messageText.text += " | " + attemptsLeft + " Remaining attempts";
        }
    }

    public void OnItemButtonClick(int buttonIndex)
    {
        if (!isGameActive || buttonIndex >= itemButtons.Length)
            return;

        isGameActive = false;

        // Disable all buttons
        foreach (GameObject button in itemButtons)
        {
            if (button != null)
            {
                Button btn = button.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = false;
                }
            }
        }

        // Determine result
        bool isWin = Random.value < winProbability;

        if (isWin)
        {
            // Win result
            playerCoins += 1000;
            CurrencyManager.Instance.AddCoins(1000);
            ShowRewardAnimation(itemButtons[buttonIndex].transform, "1000!");
            messageText.text = "Congratulations! You have won 1000 coins!";

            // Create a sequence for the button animation
            Sequence winSequence = DOTween.Sequence();
            winSequence.Append(itemButtons[buttonIndex].transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack));
            winSequence.Append(itemButtons[buttonIndex].transform.DOScale(1f, 0.2f));
        }
        else
        {
            // Lose result
            ShowRewardAnimation(itemButtons[buttonIndex].transform, "X");
            messageText.text = "Unfortunately! This item is protected.";

            // Fix the shake animation to avoid infinite loops
            itemButtons[buttonIndex].transform.DOShakePosition(0.5f, 10, 10, 90, false, false);
        }

        StartCoroutine(EndGameAfterDelay(2f));
    }

    private void ShowRewardAnimation(Transform buttonTransform, string rewardText)
    {
        if (buttonTransform == null)
            return;

        // Create reward text object
        GameObject rewardObj = new GameObject("RewardText");
        rewardObj.transform.SetParent(gamePanel);
        rewardObj.transform.position = buttonTransform.position;

        // Setup text component
        TextMeshProUGUI reward = rewardObj.AddComponent<TextMeshProUGUI>();
        reward.font = messageText.font;
        reward.fontSize = 36;
        reward.alignment = TextAlignmentOptions.Center;
        reward.text = rewardText;

        // Set color based on result
        if (rewardText == "X")
        {
            reward.color = Color.red;
        }
        else
        {
            reward.color = Color.yellow;
        }

        // Create animation sequence
        Sequence rewardSequence = DOTween.Sequence();
        rewardSequence.Append(reward.transform.DOMoveY(reward.transform.position.y + 100, 1f));
        rewardSequence.Join(reward.DOFade(0, 1f));
        rewardSequence.OnComplete(() => {
            if (rewardObj != null)
            {
                Destroy(rewardObj);
            }
        });
    }

    public bool GetIsMiniGameActive()
    {
        return isGameActive;
    }

    private IEnumerator EndGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Animate buttons to scale down before closing the panel
        Sequence closeSequence = DOTween.Sequence();

        foreach (GameObject button in gridButtons)
        {
            if (button != null)
            {
                closeSequence.Join(button.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
            }
        }

        foreach (GameObject button in itemButtons)
        {
            if (button != null)
            {
                closeSequence.Join(button.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
            }
        }

        // Wait for buttons animation to complete
        yield return closeSequence.WaitForCompletion();

        // Animate the panel closing
        Tween closeTween = gamePanel.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        yield return closeTween.WaitForCompletion();

        // Deactivate elements after animation completes
        guessingGamePanel.SetActive(false);
        oneChanceGamePanel.SetActive(false);
        messageText.gameObject.SetActive(false);
        isGameActive = false;

        // Kill any remaining tweens
        KillAllTweens();
    }

}