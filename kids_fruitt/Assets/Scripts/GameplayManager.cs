using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    public Transform playerSpawnPoint;

    private void Awake()
    {
        Instance = this;
        SpawnSelectedCharacter();
    }


    private void SpawnSelectedCharacter()
    {
        Character selectedCharacter = CharacterManager.Instance.GetSelectedCharacter();
        PlayerVisuals playerVisuals = FindFirstObjectByType<PlayerVisuals>();
        playerVisuals.SetVisualModel(Instantiate(selectedCharacter.prefab, playerSpawnPoint.position, playerSpawnPoint.rotation, FindFirstObjectByType<PlayerController>().transform).transform);
    }

 
}