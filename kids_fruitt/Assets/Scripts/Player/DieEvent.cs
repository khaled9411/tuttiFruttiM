using System;
using UnityEngine;

public class DieEvent : MonoBehaviour
{
    public static DieEvent Instance { get; private set; }

    public Action onPlayerDie;

    [SerializeField] private GameObject dieEffect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerController>(out _))
        {
            Debug.Log("Game Over!");
            Instantiate(dieEffect, other.transform.position, dieEffect.transform.rotation);
            onPlayerDie?.Invoke();
        }
    }
}
