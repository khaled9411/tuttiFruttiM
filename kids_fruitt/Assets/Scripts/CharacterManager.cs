 using UnityEngine;

[System.Serializable]
public class Character
{
    public string name;
    public GameObject prefab;
    public Sprite thumbnail;
    public int price;
    public bool isDefaultUnlocked;
}

public class CharacterManager : MonoBehaviour
{
    public Character[] availableCharacters;
    private const string SELECTED_CHARACTER_KEY = "SelectedCharacter";
    private const string PURCHASED_PREFIX = "Purchased_Character_";

    public static CharacterManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var character in availableCharacters)
            {
                if (character.isDefaultUnlocked && !IsCharacterPurchased(GetCharacterIndex(character)))
                {
                    PurchaseCharacter(GetCharacterIndex(character), false);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private int GetCharacterIndex(Character character)
    {
        for (int i = 0; i < availableCharacters.Length; i++)
        {
            if (availableCharacters[i] == character)
            {
                return i;
            }
        }
        return -1;
    }

    public void SelectCharacter(int index)
    {
        if (index >= 0 && index < availableCharacters.Length && IsCharacterPurchased(index))
        {
            PlayerPrefs.SetInt(SELECTED_CHARACTER_KEY, index);
            PlayerPrefs.Save();
        }
    }

    public Character GetSelectedCharacter()
    {
        int index = PlayerPrefs.GetInt(SELECTED_CHARACTER_KEY, 0);
        return availableCharacters[index];
    }

    public bool IsCharacterSelected(int index)
    {
        return PlayerPrefs.GetInt(SELECTED_CHARACTER_KEY, 0) == index;
    }

    public bool PurchaseCharacter(int index, bool useCoins = true)
    {
        if (index < 0 || index >= availableCharacters.Length)
            return false;

        if (IsCharacterPurchased(index))
            return false;

        if (useCoins)
        {
            Character character = availableCharacters[index];
            if (!CurrencyManager.Instance.SpendCoins(character.price))
                return false;
        }

        PlayerPrefs.SetInt(PURCHASED_PREFIX + index, 1);
        PlayerPrefs.Save();
        return true;
    }

    public bool IsCharacterPurchased(int index)
    {
        if (index < 0 || index >= availableCharacters.Length)
            return false;

        return PlayerPrefs.GetInt(PURCHASED_PREFIX + index, 0) == 1;
    }

    public int GetCharacterPrice(int index)
    {
        if (index < 0 || index >= availableCharacters.Length)
            return 0;

        return availableCharacters[index].price;
    }
}

