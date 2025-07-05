using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Transform levelSpawnPoint;
    private GameObject currentLevelInstance;
    private int currentLevelIndex = -1;
    private bool isCurrentLevelBonus = false;

    private void Start()
    {
        LoadPlayerProgress();
        LoadAndInstantiateLevelPrefab();
    }

    private void LoadPlayerProgress()
    {
        // Load the current level index from PlayerPrefs
        currentLevelIndex = PlayerPrefs.GetInt("SelectedLevelIndex", -1);

        if (currentLevelIndex >= 0)
        {
            // Check if current level is a bonus level
            isCurrentLevelBonus = PlayerPrefs.GetInt($"Level_{currentLevelIndex}_IsBonus", 0) == 1;
        }

        Debug.Log($"Loading Level Index: {currentLevelIndex}, Is Bonus: {isCurrentLevelBonus}");
    }

    private void LoadAndInstantiateLevelPrefab()
    {
        string prefabName = PlayerPrefs.GetString("SelectedLevelPrefab", "");
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("The prefab name for the level has not been determined!");
            return;
        }

        GameObject levelPrefab = Resources.Load<GameObject>("LevelPrefabs/" + prefabName);
        if (levelPrefab == null)
        {
            Debug.LogError("No prefab level found: " + prefabName);
            return;
        }

        currentLevelInstance = Instantiate(levelPrefab, levelSpawnPoint.position, levelSpawnPoint.rotation);

        // Pass level information to other systems that might need it
        NotifyLevelLoaded();

        Debug.Log($"Level loaded: {prefabName} (Index: {currentLevelIndex}, Bonus: {isCurrentLevelBonus})");
    }

    private void NotifyLevelLoaded()
    {
        // Find and notify the WinLoseUI component about the current level
        WinLoseUI winLoseUI = FindFirstObjectByType<WinLoseUI>();
        if (winLoseUI != null)
        {
            // The WinLoseUI will read the level info from PlayerPrefs in its Awake method
            // so we don't need to pass it manually
            Debug.Log("WinLoseUI found and will be notified about current level");
        }

        // You can add more notifications here if needed
        // For example, notifying a GameManager, ScoreManager, etc.
    }

    public void ReloadLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        // Reload the progress in case it changed
        LoadPlayerProgress();
        LoadAndInstantiateLevelPrefab();
    }

    // Helper methods for other systems to get level information
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    public bool IsCurrentLevelBonus()
    {
        return isCurrentLevelBonus;
    }

    public string GetCurrentLevelName()
    {
        return PlayerPrefs.GetString("SelectedLevelPrefab", "Unknown");
    }

    // Method to load a specific level (useful for testing or special cases)
    public void LoadSpecificLevel(int levelIndex, string levelName, bool isBonusLevel)
    {
        // Save the new level info
        PlayerPrefs.SetInt("SelectedLevelIndex", levelIndex);
        PlayerPrefs.SetString("SelectedLevelPrefab", levelName);
        PlayerPrefs.SetInt($"Level_{levelIndex}_IsBonus", isBonusLevel ? 1 : 0);
        PlayerPrefs.Save();

        // Update local variables
        currentLevelIndex = levelIndex;
        isCurrentLevelBonus = isBonusLevel;

        // Reload the level
        ReloadLevel();
    }

    // Method to get the next regular level index (skipping bonus levels)
    public int GetNextRegularLevelIndex()
    {
        int nextIndex = currentLevelIndex + 1;

        // Skip bonus levels
        while (nextIndex <= 29 && PlayerPrefs.GetInt($"Level_{nextIndex}_IsBonus", 0) == 1)
        {
            nextIndex++;
        }

        return nextIndex <= 29 ? nextIndex : -1;
    }

    // Method to check if there's a next level available
    public bool HasNextLevel()
    {
        if (isCurrentLevelBonus)
        {
            return false; // No next level from bonus levels
        }

        int nextLevelIndex = GetNextRegularLevelIndex();
        int highestUnlockedLevel = PlayerPrefs.GetInt("HighestUnlockedLevel", 0);

        return nextLevelIndex != -1 && nextLevelIndex <= highestUnlockedLevel;
    }

    // Debug method to print current level information
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            Debug.Log($"Current Level: {currentLevelIndex}, Bonus: {isCurrentLevelBonus}, Name: {GetCurrentLevelName()}");
        }
    }
}