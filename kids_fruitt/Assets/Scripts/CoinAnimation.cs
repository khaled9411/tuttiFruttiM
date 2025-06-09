using UnityEngine;
using DG.Tweening;

public class CoinAnimation : MonoBehaviour
{
    [Header("Coin Movement")]
    [SerializeField] private float hoverHeight = 0.5f;     
    [SerializeField] private float hoverDuration = 1.0f;   
    [SerializeField] private float rotationDuration = 1.5f;

    [Header("Collecting Coin Animation")]
    [SerializeField] private float collectMoveHeight = 2.0f;  
    [SerializeField] private float collectMoveDuration = 0.5f;
    [SerializeField] private float fadeDuration = 0.3f;       

    [Header("Audio")]
    [SerializeField] private AudioClip coinCollectSound;

    private Vector3 startPosition;
    private AudioSource audioSource;
    private Sequence idleSequence;
    private bool isCollected = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && coinCollectSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        startPosition = transform.position;

        StartIdleAnimation();
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);

        if (idleSequence != null)
        {
            idleSequence.Kill();
        }
    }

    private void StartIdleAnimation()
    {
        if (isCollected) return;

        idleSequence = DOTween.Sequence();

        Tween hoverTween = transform.DOMoveY(startPosition.y + hoverHeight, hoverDuration / 2)
            .SetEase(Ease.InOutSine);

        Tween rotateTween = transform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);

        idleSequence.Append(hoverTween);
        idleSequence.Join(rotateTween);

        idleSequence.SetLoops(-1, LoopType.Yoyo); 
        idleSequence.SetUpdate(true);
    }


    public void Collect()
    {
        if (isCollected) return;
        isCollected = true;

        if (idleSequence != null)
        {
            idleSequence.Kill();
        }

        if (audioSource != null && coinCollectSound != null)
        {
            audioSource.PlayOneShot(coinCollectSound);
        }

        Sequence collectSequence = DOTween.Sequence();

        Tween moveTween = transform.DOMoveY(transform.position.y + collectMoveHeight, collectMoveDuration)
            .SetEase(Ease.OutQuad);

        Tween spinTween = transform.DORotate(new Vector3(0, 720, 0), collectMoveDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutQuad);

        Tween fadeTween = transform.DOScale(Vector3.zero, fadeDuration)
            .SetEase(Ease.InBack)
            .SetDelay(collectMoveDuration - fadeDuration / 2);

        collectSequence.Append(moveTween);
        collectSequence.Join(spinTween);
        collectSequence.Join(fadeTween);

        collectSequence.OnComplete(() => Destroy(gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<PlayerController>(out _) && !isCollected)
        {
            CurrencyManager currencyManager = CurrencyManager.Instance;
            if (currencyManager != null)
            {
                currencyManager.AddCoins(1);
            }

            Collect();
        }
    }
}