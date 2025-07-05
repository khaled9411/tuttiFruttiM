using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class BonusPurchaseDialog : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Image coinIcon;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InBack;

    private Action<bool> onDialogClosed;
    private int levelIndex;
    private int price;
    private bool isAnimating;

    private void Start()
    {
        // Hide dialog initially
        dialogPanel.SetActive(false);

        // Setup button listeners
        purchaseButton.onClick.AddListener(OnPurchaseClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
    }

    public void ShowDialog(int levelIndex, int price, Action<bool> onClosed)
    {
        if (isAnimating) return;

        this.levelIndex = levelIndex;
        this.price = price;
        this.onDialogClosed = onClosed;

        // Update UI
        titleText.text = $"Bonus Level {levelIndex + 1}";
        descriptionText.text = "Would you like to purchase this bonus level?";
        priceText.text = price.ToString();

        // Show dialog with animation
        dialogPanel.SetActive(true);

        // Start with scaled down and fade
        dialogPanel.transform.localScale = Vector3.zero;
        CanvasGroup canvasGroup = dialogPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = dialogPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        isAnimating = true;

        // Animate in
        Sequence showSequence = DOTween.Sequence();
        showSequence.Append(dialogPanel.transform.DOScale(Vector3.one, animationDuration).SetEase(showEase));
        showSequence.Join(canvasGroup.DOFade(1f, animationDuration));

        // Add bounce effect to coin icon
        if (coinIcon != null)
        {
            showSequence.AppendCallback(() =>
            {
                coinIcon.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2, 0.5f);
            });
        }

        showSequence.OnComplete(() => isAnimating = false);
    }

    private void OnPurchaseClicked()
    {
        if (isAnimating) return;

        // Check if player has enough coins
        if (CurrencyManager.Instance.GetCurrencyData().coins >= price)
        {
            // Purchase successful
            CurrencyManager.Instance.SpendCoins(price);
            HideDialog(true);
        }
        else
        {
            // Not enough coins
            NotificationManager.Instance.ShowNotification("Not enough coins!", Color.red);
            HideDialog(false);
        }
    }

    private void OnCancelClicked()
    {
        if (isAnimating) return;
        HideDialog(false);
    }

    private void HideDialog(bool purchased)
    {
        if (isAnimating) return;

        isAnimating = true;

        CanvasGroup canvasGroup = dialogPanel.GetComponent<CanvasGroup>();

        // Animate out
        Sequence hideSequence = DOTween.Sequence();
        hideSequence.Append(dialogPanel.transform.DOScale(Vector3.zero, animationDuration).SetEase(hideEase));
        hideSequence.Join(canvasGroup.DOFade(0f, animationDuration));

        hideSequence.OnComplete(() =>
        {
            dialogPanel.SetActive(false);
            isAnimating = false;
            onDialogClosed?.Invoke(purchased);
        });
    }
}