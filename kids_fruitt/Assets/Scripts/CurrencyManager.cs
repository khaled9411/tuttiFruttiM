using UnityEngine;
using System;
using System.IO;

[Serializable]
public class CurrencyData
{
    public int coins = 0;
    public int gems = 0;
    public int stars = 0;
}

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }


    [SerializeField] private AudioClip takeMoneySound;
    [SerializeField] private AudioClip takeAlotMoneySound;
    [SerializeField] private AudioClip spendMoneySound;


    private CurrencyData currencyData = new CurrencyData();

    private string savePath;

    public event Action<CurrencyData> OnCurrencyChanged;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, "currency_data.json");

            LoadCurrency();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoins(int amount)
    {
        if (amount < 0) return;

        if(amount == 1)
            PlaySound(takeMoneySound);
        else 
            PlaySound(takeAlotMoneySound);

        currencyData.coins += amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currencyData);
    }

    public void AddGems(int amount)
    {
        if (amount < 0) return;
        currencyData.gems += amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currencyData);
    }

    public void AddStars(int amount)
    {
        if (amount < 0) return;
        currencyData.stars += amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currencyData);
    }

    public bool SpendCoins(int amount)
    {
        if (amount < 0 || currencyData.coins < amount) return false;

        PlaySound(spendMoneySound);
        currencyData.coins -= amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currencyData);
        return true;
    }

    public bool SpendGems(int amount)
    {
        if (amount < 0 || currencyData.gems < amount) return false;

        currencyData.gems -= amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currencyData);
        return true;
    }

    public bool SpendStars(int amount)
    {
        if (amount < 0 || currencyData.stars < amount) return false;

        currencyData.stars -= amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currencyData);
        return true;
    }

    public CurrencyData GetCurrencyData()
    {
        return currencyData;
    }

    public void SaveCurrency()
    {
        string jsonData = JsonUtility.ToJson(currencyData);
        File.WriteAllText(savePath, jsonData);
    }

    public void LoadCurrency()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            currencyData = JsonUtility.FromJson<CurrencyData>(jsonData) ?? new CurrencyData();
        }
    }

    public void ResetCurrency()
    {
        currencyData = new CurrencyData();
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currencyData);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudioForMoney");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.PlayOneShot(clip);
        Destroy(tempGO, clip.length);
    }
}