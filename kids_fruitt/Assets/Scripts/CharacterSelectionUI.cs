using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform characterDisplayPoint;
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private GameObject buttonPrefab;

    [System.Serializable]
    public class ButtonConfig
    {
        public Sprite normalButtonSprite;
        public Sprite selectedButtonSprite;
        public Sprite lockedButtonSprite;
    }

    [SerializeField] private ButtonConfig buttonConfig = new ButtonConfig();

    private GameObject currentDisplayedCharacter;
    private CharacterManager characterManager;

    private class CharacterButtonInfo
    {
        public GameObject buttonObject;
        public Button button;
        public Image backgroundImage;
        public Image characterImage;
        public TextMeshProUGUI priceText;
    }

    private CharacterButtonInfo[] characterButtons;

    private void Start()
    {
        characterManager = CharacterManager.Instance;
        CreateCharacterButtons();
        UpdateUI();
        UpdateCharacterDisplay();
    }

    private void CreateCharacterButtons()
    {
        characterButtons = new CharacterButtonInfo[characterManager.availableCharacters.Length];

        for (int i = 0; i < characterManager.availableCharacters.Length; i++)
        {
            Character character = characterManager.availableCharacters[i];

            GameObject buttonObj = Instantiate(buttonPrefab, buttonsContainer);

            CharacterButtonInfo buttonInfo = new CharacterButtonInfo();
            characterButtons[i] = buttonInfo;

            buttonInfo.buttonObject = buttonObj;
            buttonInfo.button = buttonObj.GetComponent<Button>();

            Transform backgroundTrans = buttonObj.transform.GetChild(0);
            Transform characterTrans = buttonObj.transform.GetChild(1);
            Transform priceTrans = buttonObj.transform.GetChild(2);

            if (backgroundTrans != null)
            {
                buttonInfo.backgroundImage = backgroundTrans.GetComponent<Image>();
            }
            else
            {
                Debug.LogError("The first child (background image) is not in Prefab Zer!");
            }

            if (characterTrans != null)
            {
                buttonInfo.characterImage = characterTrans.GetComponent<Image>();
                if (buttonInfo.characterImage != null && character.thumbnail != null)
                {
                    buttonInfo.characterImage.sprite = character.thumbnail;
                }
            }
            else
            {
                Debug.LogError("The second child (character image) is not in the button prefab!");
            }

            if (priceTrans != null)
            {
                buttonInfo.priceText = priceTrans.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogError("3rd child (half price) not in prefab button!");
            }

            int characterIndex = i;
            buttonInfo.button.onClick.AddListener(() => OnCharacterButtonClicked(characterIndex));
        }
    }

    public void UpdateUI()
    {
        if (characterButtons == null) return;

        for (int i = 0; i < characterButtons.Length; i++)
        {
            UpdateButtonUI(i);
        }
    }

    private void UpdateButtonUI(int index)
    {
        if (index < 0 || index >= characterButtons.Length) return;

        Character character = characterManager.availableCharacters[index];
        CharacterButtonInfo buttonInfo = characterButtons[index];

        bool isPurchased = characterManager.IsCharacterPurchased(index);
        bool isSelected = characterManager.IsCharacterSelected(index);

        if (!isPurchased)
        {
            if (buttonInfo.backgroundImage != null && buttonConfig.lockedButtonSprite != null)
            {
                buttonInfo.backgroundImage.sprite = buttonConfig.lockedButtonSprite;
            }

            if (buttonInfo.priceText != null)
            {
                buttonInfo.priceText.gameObject.SetActive(true);
                buttonInfo.priceText.text = character.price.ToString();
            }

        }
        else if (!isSelected)
        {
            if (buttonInfo.backgroundImage != null && buttonConfig.normalButtonSprite != null)
            {
                buttonInfo.backgroundImage.sprite = buttonConfig.normalButtonSprite;
            }

            if (buttonInfo.priceText != null)
            {
                buttonInfo.priceText.gameObject.SetActive(false);
            }

        }
        else
        {
            if (buttonInfo.backgroundImage != null && buttonConfig.selectedButtonSprite != null)
            {
                buttonInfo.backgroundImage.sprite = buttonConfig.selectedButtonSprite;
            }

            if (buttonInfo.priceText != null)
            {
                buttonInfo.priceText.gameObject.SetActive(false);
            }
        }
    }

    public void OnCharacterButtonClicked(int index)
    {
        if (characterManager.IsCharacterPurchased(index))
        {
            characterManager.SelectCharacter(index);
        }
        else
        {
            if (characterManager.PurchaseCharacter(index))
            {
                characterManager.SelectCharacter(index);
            }
            else
            {
                Debug.Log("There are not enough coins to purchase this character!");
            }
        }

        UpdateUI();
        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        if (currentDisplayedCharacter != null)
        {
            Destroy(currentDisplayedCharacter);
        }

        Character selectedCharacter = characterManager.GetSelectedCharacter();
        if (selectedCharacter != null && selectedCharacter.prefab != null)
        {
            currentDisplayedCharacter = Instantiate(selectedCharacter.prefab, characterDisplayPoint);
        }
    }
}