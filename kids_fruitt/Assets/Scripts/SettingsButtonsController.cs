using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsButtonsController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button mainSettingsButton;
    [SerializeField] private List<Button> settingsButtons;
    [SerializeField] private Image mainButtonImage;
    [SerializeField] private Sprite settingsSprite;
    [SerializeField] private Sprite closeSprite;

    [Header("Animation Settings")]
    [SerializeField] private float rotationDuration = 0.4f;
    [SerializeField] private float slideDistance = 200f;
    [SerializeField] private float buttonAnimDuration = 0.3f;
    [SerializeField] private float buttonDelay = 0.08f;

    private bool isOpen = false;
    private Sequence currentSequence;
    private List<Vector2> originalPositions;

    private void Start()
    {
        originalPositions = new List<Vector2>();
        foreach (var button in settingsButtons)
        {
            RectTransform rect = button.GetComponent<RectTransform>();
            originalPositions.Add(rect.anchoredPosition);
            button.gameObject.SetActive(true);
        }

        mainSettingsButton.onClick.AddListener(ToggleSettings);
    }

    private void ToggleSettings()
    {
        if (currentSequence != null)
        {
            currentSequence.Kill();
        }

        currentSequence = DOTween.Sequence();

        if (!isOpen)
        {
            OpenSettingsButtons();
        }
        else
        {
            CloseSettingsButtons();
        }

        isOpen = !isOpen;
    }

    private void OpenSettingsButtons()
    {
        currentSequence.Append(mainSettingsButton.transform
            .DORotate(new Vector3(0, 0, 180f), rotationDuration)
            .OnComplete(() => mainButtonImage.sprite = closeSprite));

        for (int i = 0; i < settingsButtons.Count; i++)
        {
            var button = settingsButtons[i];
            RectTransform rect = button.GetComponent<RectTransform>();

            Vector2 targetPosition = originalPositions[i] + new Vector2(slideDistance, 0);

            currentSequence.Insert(rotationDuration + (i * buttonDelay),
                rect.DOAnchorPos(targetPosition, buttonAnimDuration)
                    .SetEase(Ease.OutBack, 1.2f)
                    .OnStart(() => button.interactable = true));
        }

        for (int i = 0; i < mainSettingsButton.transform.childCount; i++)
        {
            mainSettingsButton.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void CloseSettingsButtons()
    {


        for (int i = settingsButtons.Count - 1; i >= 0; i--)
        {
            var button = settingsButtons[i];
            RectTransform rect = button.GetComponent<RectTransform>();

            int index = settingsButtons.Count - 1 - i;
            currentSequence.Insert(index * buttonDelay,
                rect.DOAnchorPos(originalPositions[i], buttonAnimDuration)
                    .SetEase(Ease.InBack, 1.2f)
                    .OnComplete(() => button.interactable = false));
        }

        float totalDuration = settingsButtons.Count * buttonDelay;
        currentSequence.Insert(totalDuration,
            mainSettingsButton.transform.DORotate(Vector3.zero, rotationDuration)
                .OnComplete(() => mainButtonImage.sprite = settingsSprite));

        for (int i = 0; i < mainSettingsButton.transform.childCount; i++)
        {
            mainSettingsButton.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (currentSequence != null)
        {
            currentSequence.Kill();
        }

        mainSettingsButton.onClick.RemoveListener(ToggleSettings);
    }

    public void SetSlideDistance(float newDistance)
    {
        slideDistance = newDistance;
    }
}