using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string levelName;
    public bool isCompleted;

    [Header("Bonus Level Settings")]
    public bool isBonusLevel;
    [SerializeField] private int bonusLevelPrice;

    public int BonusLevelPrice => bonusLevelPrice;
}