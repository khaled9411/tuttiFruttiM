using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinsText;
    private TextMeshProUGUI gemsText;
    private TextMeshProUGUI starsText;

    private void Start()
    {
        CurrencyManager.Instance.OnCurrencyChanged += UpdateUI;

        UpdateUI(CurrencyManager.Instance.GetCurrencyData());
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateUI;
        }
    }

    private void UpdateUI(CurrencyData data)
    {
        if (coinsText != null) coinsText.text = data.coins.ToString();
        if (gemsText != null) gemsText.text = data.gems.ToString();
        if (starsText != null) starsText.text = data.stars.ToString();
    }
}