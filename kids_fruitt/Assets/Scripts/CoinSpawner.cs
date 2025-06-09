using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int numberOfCoins = 10;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minHeight = 0.5f;
    [SerializeField] private float maxHeight = 1.5f;

    [Header("Collection Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private KeyCode collectAllKey = KeyCode.C;
    [SerializeField] private float collectAllDuration = 0.8f;

    private List<GameObject> spawnedCoins = new List<GameObject>();

    private void Start()
    {
        if (coinPrefab == null)
        {
            Debug.LogError("Prefab currency not set!");
            return;
        }

        SpawnCoins();
    }

    private void Update()
    {
        if (Input.GetKeyDown(collectAllKey))
        {
            CollectAllCoins();
        }
    }

    public void SpawnCoins()
    {
        foreach (GameObject coin in spawnedCoins)
        {
            if (coin != null)
            {
                Destroy(coin);
            }
        }
        spawnedCoins.Clear();

        for (int i = 0; i < numberOfCoins; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            float randomHeight = Random.Range(minHeight, maxHeight);
            Vector3 spawnPosition = new Vector3(randomCircle.x, randomHeight, randomCircle.y) + transform.position;

            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360), 0));

            spawnedCoins.Add(coin);
        }
    }

    public void CollectAllCoins()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned!");
            return;
        }

        foreach (GameObject coin in spawnedCoins)
        {
            if (coin == null) continue;

            float delay = Random.Range(0, 0.2f);

            coin.transform.DOMove(player.position, collectAllDuration)
                .SetDelay(delay)
                .SetEase(Ease.InQuad)
                .OnComplete(() => {
                    CoinAnimation coinAnim = coin.GetComponent<CoinAnimation>();
                    if (coinAnim != null)
                    {
                        coinAnim.Collect();
                    }
                    else
                    {
                        CurrencyManager currencyManager = CurrencyManager.Instance;
                        if (currencyManager != null)
                        {
                            currencyManager.AddCoins(1);
                        }
                        Destroy(coin);
                    }
                });
        }

        spawnedCoins.Clear();
    }

    public void SpawnCoinsInCircle(Vector3 center, float radius, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count);
            float x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = center.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector3 spawnPosition = new Vector3(x, center.y, z);
            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.Euler(0, angle, 0));
            spawnedCoins.Add(coin);
        }
    }
}