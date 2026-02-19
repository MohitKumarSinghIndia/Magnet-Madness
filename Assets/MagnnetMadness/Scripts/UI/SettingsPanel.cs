using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("Toggles")]
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public Toggle vibrationToggle;

    private bool isLoadingSettings = false;

    void OnEnable()
    {
        musicToggle.onValueChanged.AddListener(OnMusicChanged);
        sfxToggle.onValueChanged.AddListener(OnSfxChanged);
        vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);

        LoadSettings();
    }

    void OnDisable()
    {
        musicToggle.onValueChanged.RemoveListener(OnMusicChanged);
        sfxToggle.onValueChanged.RemoveListener(OnSfxChanged);
        vibrationToggle.onValueChanged.RemoveListener(OnVibrationChanged);
    }

    private void LoadSettings()
    {
        isLoadingSettings = true;

        musicToggle.isOn = GameCore.Instance.gameData.musicEnabled;
        sfxToggle.isOn = GameCore.Instance.gameData.sfxEnabled;
        vibrationToggle.isOn = GameCore.Instance.gameData.vibrationEnabled;

        isLoadingSettings = false;
    }

    private void PlayToggleSFX()
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

        PlayToggleSFX();
    }

    void OnSfxChanged(bool enabled)
    {
        GameCore.Instance.gameData.sfxEnabled = enabled;
        GameCore.Instance.gameData.Save();

        AudioManager.Instance.ApplySettings();

        PlayToggleSFX();
    }

    void OnVibrationChanged(bool enabled)
    {
        GameCore.Instance.gameData.vibrationEnabled = enabled;
        GameCore.Instance.gameData.Save();

        PlayToggleSFX();
    }
}
