using UnityEngine;
using System;

public class WinEvent : MonoBehaviour
{
    public static WinEvent Instance { get; private set; }

    public Action onPlayerWin;

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
            int highestUnlockedLevel = PlayerPrefs.GetInt("HighestUnlockedLevel") + 1;
            PlayerPrefs.SetInt("HighestUnlockedLevel", highestUnlockedLevel);
            PlayerPrefs.Save();
            onPlayerWin?.Invoke();
        }
    }
}
