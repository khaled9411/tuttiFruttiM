using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerVisuals : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Transform visualModel;

    [Header("Rotation Settings")]
    [SerializeField] private float rollSpeed = 360f;
    [SerializeField] private bool useCustomRollAxis = false;
    [SerializeField] private Vector3 customRollAxis = Vector3.right;

    [Header("Tilt Settings")]
    [SerializeField] private float maxTiltAngle = 15f;
    [SerializeField] private float tiltSpeed = 0.3f;

    [Header("Jump Effect Settings")]
    [SerializeField] private bool useJumpEffect = true;
    [SerializeField] private float jumpSquashAmount = 0.25f;
    [SerializeField] private float squashDuration = 0.2f;
    [SerializeField] private float stretchDuration = 0.2f;
    [SerializeField] private Ease squashEase = Ease.OutBack;
    [SerializeField] private Ease stretchEase = Ease.OutBack;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem jumpParticleEffect;
    [SerializeField] private ParticleSystem movementParticleEffect;
    [SerializeField] private float particleEmissionRate = 20f;

    private Vector3 originalScale;
    private Quaternion defaultRotation;
    private Sequence jumpSequence;
    private ParticleSystem.EmissionModule movementEmission;
    private int currentDirection = 1;
    private bool hasVisualModel;

    private void Awake()
    {
        //InitializeVisuals();
        SetupParticleSystems();
    }

    private IEnumerator Start()
    {
        yield return null;
        WinEvent.Instance.onPlayerWin += ShowWinEffect;
    }

    private void InitializeVisuals()
    {
        if (visualModel == null && transform.childCount > 0)
        {
            visualModel = GameObject.FindWithTag("PlayerVisual").transform;
        }

        hasVisualModel = visualModel != null;

        if (hasVisualModel)
        {
            originalScale = visualModel.localScale;
            defaultRotation = visualModel.rotation;
        }
        else
        {
            Debug.LogWarning("No visual model set for player. Visual effects will be disabled.");
        }
    }

    private void SetupParticleSystems()
    {
        if (jumpParticleEffect != null)
        {
            jumpParticleEffect.Stop();
        }

        if (movementParticleEffect != null)
        {
            movementEmission = movementParticleEffect.emission;
            movementEmission.rateOverTime = 0;
            movementParticleEffect.Stop();
        }
    }

    public void HandleMovement(Vector3 moveDirection, MovementType movementType, MovementAxis movementAxis , bool isGrounded)
    {
        if (!hasVisualModel) return;

        

        UpdateMovementParticles(moveDirection.magnitude, moveDirection.x,isGrounded);

        if (moveDirection != Vector3.zero)
        {
            if (movementType == MovementType.Rolling)
            {
                ApplyRollingMovement(moveDirection, movementAxis);
            }
            else
            {
                ApplyTiltingMovement(moveDirection, movementAxis);
            }
        }
        else if (movementType == MovementType.Tilting)
        {
            RotateToDefault(movementAxis);
        }
    }

    private void UpdateMovementParticles(float movementMagnitude, float direction, bool isGrounded)
    {
        if (movementParticleEffect == null || !isGrounded)
        {
            movementEmission.rateOverTime = 0;
            return;
        }

        if (movementMagnitude > 0.1f)
        {
            movementEmission.rateOverTime = particleEmissionRate;

            if (!movementParticleEffect.isPlaying)
            {
                movementParticleEffect.Play();
            }

            int newDirection = direction > 0 ? 1 : -1;
            if (newDirection != currentDirection)
            {
                currentDirection = newDirection;
                FlipParticleSystem();
            }
        }
        else
        {
            movementEmission.rateOverTime = 0;
        }
    }


    private void FlipParticleSystem()
    {
        if (movementParticleEffect != null)
        {
            Vector3 currentScale = movementParticleEffect.transform.parent.localScale;
            movementParticleEffect.transform.parent.localScale = new Vector3(
                -Mathf.Abs(currentScale.x) * -currentDirection,
                currentScale.y,
                currentScale.z
            );
        }
    }



    private void ApplyRollingMovement(Vector3 moveDirection, MovementAxis movementAxis)
    {
        Vector3 rotationAxis = useCustomRollAxis ? customRollAxis :
            (movementAxis == MovementAxis.XAxis ? Vector3.forward : Vector3.left);

        visualModel.Rotate(rotationAxis, rollSpeed * Time.deltaTime * moveDirection.magnitude * Mathf.Sign(moveDirection.x + moveDirection.z));
    }

    private void ApplyTiltingMovement(Vector3 moveDirection, MovementAxis movementAxis)
    {
        float currentYRotation = visualModel.rotation.eulerAngles.y;
        float targetAngle = (movementAxis == MovementAxis.XAxis ? moveDirection.x : moveDirection.z) * -maxTiltAngle;

        Vector3 eulerAngles = movementAxis == MovementAxis.XAxis ?
            new Vector3(0, currentYRotation, -targetAngle) :
            new Vector3(targetAngle, currentYRotation, 0);

        visualModel.rotation = Quaternion.Slerp(visualModel.rotation, Quaternion.Euler(eulerAngles), tiltSpeed);
    }

    public void HandleJump()
    {
        if (jumpParticleEffect != null)
        {
            jumpParticleEffect.Play();
        }

        if (hasVisualModel && useJumpEffect)
        {
            ApplyJumpSquashAndStretch();
        }
    }

    private void ApplyJumpSquashAndStretch()
    {
        if (jumpSequence != null && jumpSequence.IsActive())
        {
            jumpSequence.Kill();
        }

        jumpSequence = DOTween.Sequence();

        Vector3 squashedScale = new Vector3(
            originalScale.x * (1 + jumpSquashAmount),
            originalScale.y * (1 - jumpSquashAmount),
            originalScale.z * (1 + jumpSquashAmount)
        );

        Vector3 stretchedScale = new Vector3(
            originalScale.x * (1 - jumpSquashAmount * 0.5f),
            originalScale.y * (1 + jumpSquashAmount),
            originalScale.z * (1 - jumpSquashAmount * 0.5f)
        );

        jumpSequence.Append(visualModel.DOScale(squashedScale, squashDuration).SetEase(squashEase))
                    .Append(visualModel.DOScale(stretchedScale, stretchDuration).SetEase(stretchEase))
                    .Append(visualModel.DOScale(originalScale, stretchDuration).SetEase(Ease.OutBounce));

        jumpSequence.Play();
    }

    public void ResetRotation()
    {
        if (!hasVisualModel) return;
        RotateToDefault(MovementAxis.ZAxis);
    }

    private void RotateToDefault(MovementAxis movementAxis)
    {
        float currentYRotation = visualModel.rotation.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, currentYRotation, 0);
        visualModel.rotation = Quaternion.Slerp(visualModel.rotation, targetRotation, tiltSpeed);
    }

    // Public setters for runtime configuration
    public void SetVisualModel(Transform newVisualModel)
    {
        visualModel = newVisualModel;
        InitializeVisuals();
    }

    public void SetJumpEffectEnabled(bool enabled)
    {
        useJumpEffect = enabled;
    }

    public void SetParticleEmissionRate(float rate)
    {
        particleEmissionRate = rate;
    }

    private void ShowWinEffect()
    {
        transform.DOScale(Vector3.one * 1.5f, 0.5f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() => {
                transform.DOScale(Vector3.one, 0.5f);
            });
    }
}