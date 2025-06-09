using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixers")]
    public AudioMixer masterMixer;

    [Header("Mixer Group Names")]
    public string musicVolumeParam = "MusicVolume";
    public string sfxVolumeParam = "SFXVolume";

    [Header("UI Elements")]
    public Button musicMuteButton;
    public Button sfxMuteButton;

    [Header("Button Sprites")]
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    private const string MUSIC_MUTED_KEY = "MusicMuted";
    private const string SFX_MUTED_KEY = "SfxMuted";

    private bool isMusicMuted;
    private bool isSfxMuted;

    void Start()
    {
        LoadAudioSettings();

        UpdateMixerVolumes();

        UpdateButtonSprites();

        musicMuteButton.onClick.AddListener(ToggleMusicMute);
        sfxMuteButton.onClick.AddListener(ToggleSfxMute);
    }

    private void LoadAudioSettings()
    {
        isMusicMuted = PlayerPrefs.GetInt(MUSIC_MUTED_KEY, 0) == 1;
        isSfxMuted = PlayerPrefs.GetInt(SFX_MUTED_KEY, 0) == 1;
    }

    private void SaveAudioSettings()
    {
        PlayerPrefs.SetInt(MUSIC_MUTED_KEY, isMusicMuted ? 1 : 0);
        PlayerPrefs.SetInt(SFX_MUTED_KEY, isSfxMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void UpdateButtonSprites()
    {
        musicMuteButton.GetComponent<Image>().sprite = isMusicMuted ? musicOffSprite : musicOnSprite;
        sfxMuteButton.GetComponent<Image>().sprite = isSfxMuted ? sfxOffSprite : sfxOnSprite;
    }

    private void UpdateMixerVolumes()
    {
        float musicVolume = isMusicMuted ? -80f : 0f;
        float sfxVolume = isSfxMuted ? -80f : 0f;

        masterMixer.SetFloat(musicVolumeParam, musicVolume);
        masterMixer.SetFloat(sfxVolumeParam, sfxVolume);
    }

    public void ToggleMusicMute()
    {
        isMusicMuted = !isMusicMuted;
        UpdateMixerVolumes();
        UpdateButtonSprites();
        SaveAudioSettings();
    }

    public void ToggleSfxMute()
    {
        isSfxMuted = !isSfxMuted;
        UpdateMixerVolumes();
        UpdateButtonSprites();
        SaveAudioSettings();
    }
}