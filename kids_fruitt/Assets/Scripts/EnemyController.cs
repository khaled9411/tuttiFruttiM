using UnityEngine;
using DG.Tweening;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] patrolPoints;
    public float moveSpeed = 4f;
    public float rotationTiltAngle = 15f;

    [Header("Attack & Defense Settings")]
    public string playerTag = "Player";
    public string groundTag = "Ground";
    public float playerBounceForce = 7f;
    public LayerMask playerLayer;

    [Header("Loot Settings")]
    public GameObject[] lootPrefabs;
    [Range(0, 1)]
    public float lootDropChance = 0.5f;
    public int minLootToDrop = 0;
    public int maxLootToDrop = 1;

    private int currentPatrolIndex = 0;
    private bool movingForward = true;
    private Rigidbody rb;
    private Collider enemyCollider;
    private Vector3 initialScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<Collider>();
        initialScale = transform.localScale;
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("Enemy has no patrol points assigned! Please assign points in the Inspector.", this);
            enabled = false;
            return;
        }
    }

    void Start()
    {
        MoveToNextPoint();
    }

    void MoveToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        Vector3 targetPosition = patrolPoints[currentPatrolIndex].position;
        Vector3 direction = (targetPosition - transform.position).normalized;

        float duration = Vector3.Distance(transform.position, targetPosition) / moveSpeed;

        transform.DOMove(targetPosition, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                Vector3 lookDir = (targetPosition - transform.position);
                lookDir.y = 0;
                //if (lookDir != Vector3.zero)
                //{
                //    transform.forward = lookDir.normalized;
                //}

                float tiltZ = movingForward ? rotationTiltAngle : rotationTiltAngle;
                transform.DOLocalRotate(new Vector3(0, transform.localEulerAngles.y, tiltZ * lookDir.normalized.z), 0.2f);
            })
            .OnComplete(() =>
            {
                if (movingForward)
                {
                    currentPatrolIndex++;
                    if (currentPatrolIndex >= patrolPoints.Length)
                    {
                        currentPatrolIndex = patrolPoints.Length - 2;
                        movingForward = false;
                    }
                }
                else
                {
                    currentPatrolIndex--;
                    if (currentPatrolIndex < 0)
                    {
                        currentPatrolIndex = 1;
                        movingForward = true;
                    }
                }
                MoveToNextPoint();
            });
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null && transform.position.y < collision.transform.position.y)
            {
                ContactPoint highestContact = GetHighestContactPoint(collision);

                if (highestContact.point.y > enemyCollider.bounds.center.y + enemyCollider.bounds.extents.y * 0.5f)
                {
                    KillEnemy();
                    playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0, playerRb.linearVelocity.z);
                    playerRb.AddForce(Vector3.up * playerBounceForce, ForceMode.VelocityChange);
                }
                else
                {
                    DieEvent.Instance.onPlayerDie?.Invoke();
                    DOTween.Kill(transform);
                }
            }
            else
            {
                DieEvent.Instance.onPlayerDie?.Invoke();
                DOTween.Kill(transform);
            }
        }
    }

    private ContactPoint GetHighestContactPoint(Collision collision)
    {
        ContactPoint highest = collision.contacts[0];
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.point.y > highest.point.y)
            {
                highest = contact;
            }
        }
        return highest;
    }

    void KillEnemy()
    {
        DOTween.Kill(transform);
        enemyCollider.enabled = false;
        rb.isKinematic = false;

        transform.DOScaleY(0.1f, 0.3f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                DropLoot();
                Destroy(gameObject, 2f);
            });
    }

    void DropLoot()
    {
        if (lootPrefabs.Length == 0) return;

        int numberOfLootToDrop = Random.Range(minLootToDrop, maxLootToDrop + 1);

        for (int i = 0; i < numberOfLootToDrop; i++)
        {
            if (Random.value < lootDropChance)
            {
                GameObject selectedLoot = lootPrefabs[Random.Range(0, lootPrefabs.Length)];
                Instantiate(selectedLoot, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (patrolPoints.Length > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Gizmos.DrawSphere(patrolPoints[i].position, 0.3f);
                if (i < patrolPoints.Length - 1)
                {
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                }
            }
        }
    }
}