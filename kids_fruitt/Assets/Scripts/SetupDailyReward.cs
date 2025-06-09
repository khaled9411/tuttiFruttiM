using UnityEngine;

public class SetupDailyReward : MonoBehaviour
{

    public static SetupDailyReward Instance { get; private set; }

    [SerializeField] private GameObject characterDisplayPoint;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Gley.DailyRewards.API.Calendar.AddClickListener(CollectButtonClicked);
    }

    private void CollectButtonClicked(int dayNumber, int reward, Sprite rewardSprite)
    {
        CurrencyManager.Instance.AddCoins(reward);
    }
    public void ShowCalender()
    {
        Gley.DailyRewards.API.Calendar.Show();
    }

    public void ShowCharacter()
    {
        characterDisplayPoint.SetActive(true);
    }

    public void HideCharacter()
    {
        characterDisplayPoint.SetActive(false);
    }

    public void HideCalender()
    {
        Destroy(GameObject.Find("DailyRewardCanvas(Clone)"));
    }
}
