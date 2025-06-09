using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using Gley.DailyRewards.Internal;

public class CalendarButtonEffects : MonoBehaviour
{
    [SerializeField] private Button calendarButton;
    [SerializeField] private Image glowImage;

    [SerializeField] private float rotationAngle = 5f;
    [SerializeField] private float rotationDuration = 0.5f;

    [SerializeField] private float glowPulseDuration = 1f;
    [SerializeField] private float glowMinAlpha = 0.3f;
    [SerializeField] private float glowMaxAlpha = 0.8f;

    [SerializeField] private float checkInterval = 1f;

    private Quaternion originalButtonRotation;
    private Sequence rotationSequence;
    private Sequence glowSequence;
    private bool effectsActive = false;
    private bool lastRewardState = false;

    private void Start()
    {
        if (glowImage == null)
        {
            GameObject glowObj = new GameObject("ButtonGlow");
            glowObj.transform.SetParent(calendarButton.transform);
            glowObj.transform.SetAsFirstSibling();

            glowImage = glowObj.AddComponent<Image>();
            glowImage.raycastTarget = false;

            RectTransform glowRect = glowImage.rectTransform;
            glowRect.anchorMin = Vector2.zero;
            glowRect.anchorMax = Vector2.one;
            glowRect.offsetMin = new Vector2(-20, -20);
            glowRect.offsetMax = new Vector2(20, 20);

            glowImage.color = new Color(1f, 0.8f, 0.2f, 0f);
        }
        else
        {
            glowImage.color = new Color(glowImage.color.r, glowImage.color.g, glowImage.color.b, 0f);
        }

        originalButtonRotation = calendarButton.transform.localRotation;

        StartCoroutine(MonitorRewardStatus());
    }

    private void OnEnable()
    {
        CheckRewardStatus();
    }

    private IEnumerator MonitorRewardStatus()
    {
        while (true)
        {
            CheckRewardStatus();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void CheckRewardStatus()
    {
        bool isRewardReady = CalendarManager.Instance.TimeExpired();

        if (isRewardReady != lastRewardState)
        {
            lastRewardState = isRewardReady;

            if (isRewardReady && !effectsActive)
            {
                StartEffects();
            }
            else if (!isRewardReady && effectsActive)
            {
                StopEffects();
            }
        }
    }

    public void ForceCheckStatus()
    {
        CheckRewardStatus();
    }

    private void StartEffects()
    {
        effectsActive = true;

        if (rotationSequence != null)
        {
            rotationSequence.Kill();
        }

        rotationSequence = DOTween.Sequence();

        rotationSequence.Append(calendarButton.transform.DOLocalRotate(new Vector3(0, 0, rotationAngle), rotationDuration).SetEase(Ease.InOutSine))
                        .Append(calendarButton.transform.DOLocalRotate(new Vector3(0, 0, -rotationAngle), rotationDuration * 2).SetEase(Ease.InOutSine))
                        .Append(calendarButton.transform.DOLocalRotate(new Vector3(0, 0, 0), rotationDuration).SetEase(Ease.InOutSine));

        rotationSequence.SetLoops(-1);

        if (glowImage != null)
        {
            if (glowSequence != null)
            {
                glowSequence.Kill();
            }

            glowSequence = DOTween.Sequence();
            glowSequence.Append(glowImage.DOFade(glowMaxAlpha, glowPulseDuration / 2).SetEase(Ease.InOutSine))
                        .Append(glowImage.DOFade(glowMinAlpha, glowPulseDuration / 2).SetEase(Ease.InOutSine));

            glowSequence.SetLoops(-1);
        }
    }

    private void StopEffects()
    {
        effectsActive = false;

        if (rotationSequence != null)
        {
            rotationSequence.Kill();
            rotationSequence = null;
        }

        calendarButton.transform.localRotation = originalButtonRotation;

        if (glowSequence != null)
        {
            glowSequence.Kill();
            glowSequence = null;
        }

        if (glowImage != null)
        {
            glowImage.DOFade(0f, 0.3f);
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(calendarButton.transform);
        DOTween.Kill(glowImage);

        if (rotationSequence != null)
        {
            rotationSequence.Kill();
        }

        if (glowSequence != null)
        {
            glowSequence.Kill();
        }
    }
}