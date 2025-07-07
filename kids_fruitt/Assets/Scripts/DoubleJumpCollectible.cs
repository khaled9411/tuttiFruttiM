using UnityEngine;
using DG.Tweening;

public class DoubleJumpCollectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectEffect;

    [Header("Animation Settings")]
    [SerializeField] private float floatHeight = 0.5f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseSpeed = 1.5f;

    [Header("Collection Animation")]
    [SerializeField] private float collectAnimationDuration = 0.5f;
    [SerializeField] private float collectScaleMultiplier = 1.5f;
    [SerializeField] private Ease collectEase = Ease.OutBack;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnAnimationDuration = 0.8f;
    [SerializeField] private Ease respawnEase = Ease.OutBounce;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Tween floatTween;
    private Tween rotateTween;
    private Tween pulseTween;
    private Sequence collectSequence;
    private Sequence respawnSequence;

    private bool isCollected = false;
    private bool isAnimating = false;
    private AudioSource audioSource;
    private MeshRenderer meshRenderer;
    private Collider itemCollider;

    private void Start()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;

        audioSource = GetComponent<AudioSource>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        itemCollider = GetComponent<Collider>();

        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }

        StartIdleAnimation();
    }

    private void StartIdleAnimation()
    {

        floatTween = transform.DOMoveY(originalPosition.y + floatHeight, floatSpeed)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        rotateTween = transform.DORotate(new Vector3(0, 360, 0), rotationSpeed / 90f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);

        pulseTween = transform.DOScale(originalScale * pulseScale, pulseSpeed)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopIdleAnimation()
    {
        floatTween?.Kill();
        rotateTween?.Kill();
        pulseTween?.Kill();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected || isAnimating) return;

        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            CollectItem(controller);
        }
    }

    private void CollectItem(PlayerController controller)
    {
        if (isCollected || isAnimating) return;

        isCollected = true;
        isAnimating = true;

        StopIdleAnimation();

        controller.GrantDoubleJump(this);

        if (audioSource != null && collectSound != null)
        {
            audioSource.clip = collectSound;
            audioSource.Play();
        }

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        PlayCollectAnimation();
    }

    private void PlayCollectAnimation()
    {
        collectSequence = DOTween.Sequence();

        collectSequence.Append(transform.DOScale(originalScale * collectScaleMultiplier, collectAnimationDuration * 0.3f)
            .SetEase(Ease.OutBack));

        collectSequence.Join(transform.DORotate(new Vector3(0, 720, 0), collectAnimationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart));

        collectSequence.Append(transform.DOScale(Vector3.zero, collectAnimationDuration * 0.7f)
            .SetEase(collectEase));

        if (meshRenderer != null)
        {
            Material material = meshRenderer.material;
            collectSequence.Join(material.DOFade(0f, collectAnimationDuration * 0.7f));
        }

        collectSequence.OnComplete(() =>
        {
            meshRenderer.enabled = false;
            itemCollider.enabled = false;
            isAnimating = false;
        });
    }

    public void OnDoubleJumpUsed()
    {
        Debug.Log("Double Jump used - Respawning collectible");
        RespawnItem();
    }

    private void RespawnItem()
    {
        isCollected = false;

        meshRenderer.enabled = true;
        itemCollider.enabled = true;

        if (meshRenderer != null && meshRenderer.material != null)
        {
            Material material = meshRenderer.material;
            Color color = material.color;
            color.a = 1f;
            material.color = color;
        }

        transform.position = originalPosition;
        transform.localScale = Vector3.zero;
        transform.rotation = Quaternion.identity;

        PlayRespawnAnimation();
    }

    private void PlayRespawnAnimation()
    {
        isAnimating = true;

        respawnSequence = DOTween.Sequence();

        respawnSequence.Append(transform.DOScale(originalScale, respawnAnimationDuration)
            .SetEase(respawnEase));

        respawnSequence.Join(transform.DORotate(new Vector3(0, 360, 0), respawnAnimationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart));

        if (meshRenderer != null)
        {
            Material material = meshRenderer.material;
            Color originalColor = material.color;
            material.color = Color.white;

            respawnSequence.Join(material.DOColor(originalColor, respawnAnimationDuration * 0.8f)
                .SetDelay(respawnAnimationDuration * 0.2f));
        }

        respawnSequence.OnComplete(() =>
        {
            isAnimating = false;
            StartIdleAnimation();
        });
    }

    private void OnDestroy()
    {
        collectSequence?.Kill();
        respawnSequence?.Kill();
        StopIdleAnimation();
    }

    public void ResetCollectible()
    {
        collectSequence?.Kill();
        respawnSequence?.Kill();
        StopIdleAnimation();

        isCollected = false;
        isAnimating = false;

        transform.position = originalPosition;
        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;

        meshRenderer.enabled = true;
        itemCollider.enabled = true;

        if (meshRenderer != null && meshRenderer.material != null)
        {
            Material material = meshRenderer.material;
            Color color = material.color;
            color.a = 1f;
            material.color = color;
        }

        StartIdleAnimation();
    }

    public bool IsCollected => isCollected;
    public bool IsAnimating => isAnimating;
}