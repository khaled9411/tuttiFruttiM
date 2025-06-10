using UnityEngine;

public class MovingPlatformParent : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
            Debug.Log("Player attached to platform");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
            Debug.Log("Player detached from platform");
        }
    }
}

