using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("Toggles")]
    public Toggle soundToggle;
    public Toggle musicToggle;
    public Toggle vibrationToggle;

    private bool isLoadingSettings = false;

    void OnEnable()
    {
        musicToggle.onValueChanged.AddListener(OnMusicChanged);
        soundToggle.onValueChanged.AddListener(OnSoundChanged);
        vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);

        LoadSettings();
    }

    void OnDisable()
    {
        musicToggle.onValueChanged.RemoveListener(OnMusicChanged);
        soundToggle.onValueChanged.RemoveListener(OnSoundChanged);
        vibrationToggle.onValueChanged.RemoveListener(OnVibrationChanged);
    }

    private void LoadSettings()
    {
        isLoadingSettings = true;

        musicToggle.isOn = GameCore.Instance.gameData.musicEnabled;
        soundToggle.isOn = GameCore.Instance.gameData.soundEnabled;
        vibrationToggle.isOn = GameCore.Instance.gameData.vibrationEnabled;

        isLoadingSettings = false;
    }

    private void PlayToggleSound()
    {
        if (isLoadingSettings) return;

        AudioManager.Instance?.PlayButtonClick();
    }

    void OnMusicChanged(bool enabled)
    {
        GameCore.Instance.gameData.musicEnabled = enabled;
        GameCore.Instance.gameData.Save();

        AudioManager.Instance.ApplySettings();

        if (enabled)
            AudioManager.Instance.PlayMusic();
        else
            AudioManager.Instance.StopMusic();

        PlayToggleSound();
    }

    void OnSoundChanged(bool enabled)
    {
        GameCore.Instance.gameData.soundEnabled = enabled;
        GameCore.Instance.gameData.Save();

        AudioManager.Instance.ApplySettings();

        PlayToggleSound();
    }

    void OnVibrationChanged(bool enabled)
    {
        GameCore.Instance.gameData.vibrationEnabled = enabled;
        GameCore.Instance.gameData.Save();

        PlayToggleSound();
    }
}
