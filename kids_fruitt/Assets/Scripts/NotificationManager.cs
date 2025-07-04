using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private Image notificationBackground;

    [Header("Animation Settings")]
    [SerializeField] private float showDuration = 0.5f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float hideDuration = 0.5f;
    [SerializeField] private Vector3 showOffset = new Vector3(0, 100f, 0);

    private bool isShowing = false;
    private Coroutine hideCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Hide notification initially
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
    }

    public void ShowNotification(string message, Color color)
    {
        // Handle spam prevention
        if (isShowing)
        {
            // Cancel current notification and show new one
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }

            // Kill any ongoing animations
            notificationPanel.transform.DOKill();

            // Update text and color immediately
            UpdateNotificationContent(message, color);

            // Reset display timer
            hideCoroutine = StartCoroutine(HideAfterDelay());
            return;
        }

        StartCoroutine(ShowNotificationCoroutine(message, color));
    }

    private IEnumerator ShowNotificationCoroutine(string message, Color color)
    {
        isShowing = true;

        // Setup notification content
        UpdateNotificationContent(message, color);

        // Show panel
        notificationPanel.SetActive(true);

        // Set initial position (above screen)
        Vector3 originalPosition = notificationPanel.transform.localPosition;
        Vector3 startPosition = originalPosition + showOffset;
        notificationPanel.transform.localPosition = startPosition;

        // Set initial alpha
        CanvasGroup canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = notificationPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        // Animate in
        Sequence showSequence = DOTween.Sequence();
        showSequence.Append(notificationPanel.transform.DOLocalMove(originalPosition, showDuration).SetEase(Ease.OutBounce));
        showSequence.Join(canvasGroup.DOFade(1f, showDuration));

        // Add text animation
        notificationText.transform.localScale = Vector3.zero;
        showSequence.Join(notificationText.transform.DOScale(Vector3.one, showDuration).SetEase(Ease.OutBack));

        yield return showSequence.WaitForCompletion();

        // Start hide timer
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        // Animate out
        Vector3 originalPosition = notificationPanel.transform.localPosition;
        Vector3 endPosition = originalPosition + showOffset;

        CanvasGroup canvasGroup = notificationPanel.GetComponent<CanvasGroup>();

        Sequence hideSequence = DOTween.Sequence();
        hideSequence.Append(notificationPanel.transform.DOLocalMove(endPosition, hideDuration).SetEase(Ease.InBack));
        hideSequence.Join(canvasGroup.DOFade(0f, hideDuration));
        hideSequence.Join(notificationText.transform.DOScale(Vector3.zero, hideDuration).SetEase(Ease.InBack));

        yield return hideSequence.WaitForCompletion();

        // Hide panel
        notificationPanel.SetActive(false);
        isShowing = false;
        hideCoroutine = null;
        notificationPanel.transform.localPosition = Vector3.zero;
    }

    private void UpdateNotificationContent(string message, Color color)
    {
        notificationText.text = message;
        notificationText.color = color;

        if (notificationBackground != null)
        {
            // Make background slightly transparent version of the text color
            Color bgColor = color;
            bgColor.a = 0.8f;
            //notificationBackground.color = bgColor;
        }
    }
}