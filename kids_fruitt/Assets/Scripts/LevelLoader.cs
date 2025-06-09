using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Transform levelSpawnPoint;

    private GameObject currentLevelInstance;

    private void Start()
    {
        LoadAndInstantiateLevelPrefab();
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

        // Level setup (special actions can be added here)
        Debug.Log("Level loaded: " + prefabName);
    }

    public void ReloadLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        LoadAndInstantiateLevelPrefab();
    }
}