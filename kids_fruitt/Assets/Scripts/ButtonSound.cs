using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip clickSound;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    void PlayClickSound()
    {
        if (clickSound == null) return;

        GameObject tempGO = new GameObject("TempAudioForClickButton");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.PlayOneShot(clickSound);
        Destroy(tempGO, clickSound.length);
    }
}
