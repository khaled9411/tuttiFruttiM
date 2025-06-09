using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{

    [SerializeField] private GameObject MainManuPanel;
    [SerializeField] private GameObject selectCharacterPanel;
    [SerializeField] private Button opecSelectCharacterButton;
    [SerializeField] private Button closeSelectCharacterButton;


    private void Awake()
    {
        opecSelectCharacterButton.onClick.AddListener(ShowSelectCharacterPanel);
        opecSelectCharacterButton.onClick.AddListener(HideMainManu);
        closeSelectCharacterButton.onClick.AddListener(HideSelectCharacterPanel);
        closeSelectCharacterButton.onClick.AddListener(ShowMainManu);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.01f);

        CloseAllPanels();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSelectCharacterPanel() =>  selectCharacterPanel.SetActive(true);
    public void HideSelectCharacterPanel() => selectCharacterPanel.SetActive(false);
    public void ShowMainManu() => MainManuPanel.SetActive(true);
    public void HideMainManu() => MainManuPanel?.SetActive(false);

    public void CloseAllPanels()
    {
        selectCharacterPanel.SetActive(false);
        MainManuPanel.SetActive(true);
    }
}
