using Gley.EasyIAP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FruitIAPManager : MonoBehaviour
{
    [SerializeField] private Button removeADsButton;
    [SerializeField] private TextMeshProUGUI removeADsPriseText;
    [SerializeField] private TextMeshProUGUI coin500PriseText;
    [SerializeField] private TextMeshProUGUI coin1000PriseText;
    [SerializeField] private TextMeshProUGUI coin10000PriseText;
    [SerializeField] private TextMeshProUGUI coin50000PriseText;


    bool removeAds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Gley.EasyIAP.API.Initialize(InitializationComplete);
        if (Gley.EasyIAP.API.IsInitialized())
        {
            RefreshUI();
        }
    }

    void RefreshUI()
    {
        //this should be the example
        //but since your products will not have the same names, we will use the string version 
        //buyCoinsText.text = $"Buy {Gley.EasyIAP.API.GetValue(ShopProductNames.Coins)} Coins {Gley.EasyIAP.API.GetLocalizedPriceString(ShopProductNames.Coins)}";
        //buyRemoveAdsText.text = $"Remove ads - {Gley.EasyIAP.API.GetLocalizedPriceString(ShopProductNames.RemoveAds)}";

        //this is just a workaround to avoid errors
        //ShopProductNames coinsProduct = Gley.EasyIAP.API.ConvertNameToShopProduct("Coins");
        //ShopProductNames adsProduct = Gley.EasyIAP.API.ConvertNameToShopProduct("RemoveAds");
        //buyCoinsText.text = $"Buy {Gley.EasyIAP.API.GetValue(coinsProduct)} Coins {Gley.EasyIAP.API.GetLocalizedPriceString(coinsProduct)}";
        //buyRemoveAdsText.text = $"Remove ads - {Gley.EasyIAP.API.GetLocalizedPriceString(adsProduct)}";

        removeADsPriseText.text = Gley.EasyIAP.API.GetLocalizedPriceString(ShopProductNames.RemoveAds);
        coin500PriseText.text = Gley.EasyIAP.API.GetLocalizedPriceString(ShopProductNames.Coins500);
        coin1000PriseText.text = Gley.EasyIAP.API.GetLocalizedPriceString(ShopProductNames.Coins1000);
        coin10000PriseText.text = Gley.EasyIAP.API.GetLocalizedPriceString(ShopProductNames.Coins10000);
        coin50000PriseText.text = Gley.EasyIAP.API.GetLocalizedPriceString(ShopProductNames.Coins50000);
    }

    private void InitializationComplete(IAPOperationStatus status, string message)
    {
        if (status == IAPOperationStatus.Success)
        {
            // IAP was successfully initialized.
            // If remove ads was bought before, mark it as owned.

            //this should be your call
            if (API.IsActive(ShopProductNames.RemoveAds))
            {
                removeADsButton.interactable = false;
                removeADsPriseText.text = "Owned";
            }

            // This is just a workaround to avoid errors
            //ShopProductNames adsProduct = Gley.EasyIAP.API.ConvertNameToShopProduct("RemoveAds");
            //if (API.IsActive(adsProduct))
            //{
            //    removeAds = true;
            //}
        }
        else
        {
            Debug.Log("Error occurred: " + message);
        }
        RefreshUI();
    }

    public void BuyCoins(string productName)
    {
        //this is the normal implementation
        ////but since your products will not have the same names, we will use the string version to avoid compile errors
        //Gley.EasyIAP.API.BuyProduct(ShopProductNames.Coins, ProductBought);


        ShopProductNames coinsProduct = Gley.EasyIAP.API.ConvertNameToShopProduct(productName);
        Gley.EasyIAP.API.BuyProduct(coinsProduct, ProductBought);
    }

    public void BuyRemoveAds()
    {
        // this is the normal implementation
        //but since your products will not have the same names, we will use the string version to avoid compile errors
        //Gley.EasyIAP.API.BuyProduct(ShopProductNames.RemoveAds, ProductBought);

        ShopProductNames adsProduct = Gley.EasyIAP.API.ConvertNameToShopProduct("RemoveAds");
        Gley.EasyIAP.API.BuyProduct(adsProduct, ProductBought);
    }

    private void ProductBought(IAPOperationStatus status, string message, StoreProduct product)
    {
        if (status == IAPOperationStatus.Success)
        {
            //since all consumable products reward the same coin, a simple type check is enough 
            if (product.productType == ProductType.Consumable)
            {
                CurrencyManager.Instance.AddCoins(product.value);
            }

            if (product.productName == "RemoveAds")
            {
                Gley.MobileAds.API.RemoveAds(true);
            }
        }
        else
        {
            Debug.Log("Error occurred: " + message);
        }
        RefreshUI();
    }
}
