using UnityEngine;
using DG.Tweening;

public class Barrier : MonoBehaviour
{
    public float openHeight = 5f;
    public float openDuration = 1f;
    public AudioClip openSound;

    private Vector3 initialPosition;
    private bool isOpen = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && PlayerInventory.instance.hasKey && !isOpen)
        {
            OpenBarrier();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PlayerInventory.instance.hasKey)
        {
            NotificationManager.Instance.ShowNotification("You Need Key To Open This", Color.red);
        }
    }

    void OpenBarrier()
    {
        isOpen = true;

        if (openSound != null)
        {
            AudioSource.PlayClipAtPoint(openSound, transform.position);
        }

        transform.DOMoveY(initialPosition.y + openHeight, openDuration)
            .SetEase(Ease.OutQuad);
    }
}