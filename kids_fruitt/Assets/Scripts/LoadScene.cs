using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Scenes
{
    MainManu,
    Loading,
    Level
}

public class LoadScene : MonoBehaviour
{
    [SerializeField] private GameObject rewardedAdButton;
    private void Start()
    {
        AdManager.Instance.rewardedAdButton = rewardedAdButton;
        rewardedAdButton.GetComponent<Button>().onClick.AddListener(AdManager.Instance.ShowRewardedAd);

        AdManager.Instance.ShowInterstitialAd();
    }
    public void LoadLoadingScene()
    {
        SceneManager.LoadScene("Loading");
    }
}
