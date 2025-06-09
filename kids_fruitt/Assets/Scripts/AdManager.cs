using UnityEngine;
using System.Collections;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    [Header("Ad Settings")]
    [SerializeField] private float interstitialAdCooldown = 180f;
    [SerializeField] private float rewardedAdCooldown = 240f;
    [SerializeField] private int rewardAmount = 100;
    /*[HideInInspector]*/ public GameObject rewardedAdButton;

    private float lastInterstitialTime;
    private float lastRewardedTime;
    private bool canShowInterstitial = true;
    private bool canShowRewarded = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Gley.MobileAds.API.Initialize();
        //Gley.MobileAds.API.Initialize();
        //Advertisements.Instance.Initialize();

        StartCoroutine(CheckRewardedAdsAvailability());

        UpdateRewardedAdButton();
    }

    private void Update()
    {
        UpdateAdTimers();
    }

    private void UpdateAdTimers()
    {
        if (!canShowInterstitial && Time.time - lastInterstitialTime >= interstitialAdCooldown)
        {
            canShowInterstitial = true;
        }

        if (!canShowRewarded && Time.time - lastRewardedTime >= rewardedAdCooldown)
        {
            canShowRewarded = true;
            UpdateRewardedAdButton();
        }
    }

    private void UpdateRewardedAdButton()
    {
        if (rewardedAdButton != null)
        {
            bool isRewardedAdAvailable = Gley.MobileAds.API.IsRewardedVideoAvailable() && canShowRewarded;
            rewardedAdButton.SetActive(isRewardedAdAvailable);
        }
    }

    private IEnumerator CheckRewardedAdsAvailability()
    {
        while (true)
        {
            UpdateRewardedAdButton();
            yield return new WaitForSeconds(1f);
        }
    }

    #region Interstitial Ad

    public void ShowInterstitialAd()
    {
        if (canShowInterstitial && Gley.MobileAds.API.IsInterstitialAvailable())
        {
            Gley.MobileAds.API.ShowInterstitial();
            //Advertisements.Instance.ShowInterstitial(InterstitialClosed);
            lastInterstitialTime = Time.time;
            canShowInterstitial = false;
        }
    }

    private void InterstitialClosed(bool success)
    {
        Debug.Log("Skippable ad closed. Success: " + success);
    }

    public bool CanShowInterstitialAd()
    {
        return canShowInterstitial && Gley.MobileAds.API.IsInterstitialAvailable();
    }

    #endregion

    #region Rewarded Ad

    public void ShowRewardedAd()
    {
        if (canShowRewarded && Gley.MobileAds.API.IsRewardedVideoAvailable())
        {
            Gley.MobileAds.API.ShowRewardedVideo(RewardedAdClosed);
        }
    }

    private void RewardedAdClosed(bool success)
    {
        Debug.Log("The reward ad has been closed. Success: " + success);

        if (success)
        {
            CurrencyManager.Instance.AddCoins(rewardAmount);

            lastRewardedTime = Time.time;
            canShowRewarded = false;
        }

        UpdateRewardedAdButton();
    }

    public bool CanShowRewardedAd()
    {
        return canShowRewarded && Gley.MobileAds.API.IsRewardedVideoAvailable();
    }

    #endregion
}