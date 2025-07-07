using UnityEngine;
using DG.Tweening;

public class Key : MonoBehaviour
{
    public AudioClip collectSound;
    public float scaleDuration = 0.5f;

    private bool collected = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !collected)
        {
            collected = true;
            PlayerInventory.instance.hasKey = true;

            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            if (PlayerInventory.instance.keyUIIcon != null)
            {
                PlayerInventory.instance.keyUIIcon.SetActive(true);
            }

            NotificationManager.Instance.ShowNotification("You got the key go open the way", Color.green);

            transform.DOScale(Vector3.zero, scaleDuration)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}