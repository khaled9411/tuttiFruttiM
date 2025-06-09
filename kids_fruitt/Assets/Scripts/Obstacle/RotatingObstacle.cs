using UnityEngine;
using DG.Tweening;

public class RotatingObstacle : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float pushForce = 2f;

    private void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), rotationSpeed, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    public void HandleCollision(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out _))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
                pushDirection.y = 0;
                playerRb.DOMove(collision.transform.position + pushDirection * pushForce, 0.5f)
                    .SetEase(Ease.OutQuad);
            }
        }
    }
}
