using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public Toggle vibrationToggle;

    void Awake()
    {
        // Load saved settings
        GameData.MusicEnabled = PlayerPrefs.GetInt("music", 1) == 1;
        GameData.SfxEnabled = PlayerPrefs.GetInt("sfx", 1) == 1;
        GameData.VibrationEnabled = PlayerPrefs.GetInt("vibration", 1) == 1;

        // Apply values to toggles (no callbacks fire in Awake)
        musicToggle.isOn = GameData.MusicEnabled;
        sfxToggle.isOn = GameData.SfxEnabled;
        vibrationToggle.isOn = GameData.VibrationEnabled;
    }

    void OnEnable()
    {
        // Add listeners when panel opens
        musicToggle.onValueChanged.AddListener(OnMusicChanged);
        sfxToggle.onValueChanged.AddListener(OnSfxChanged);
        vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
    }

    void OnDisable()
    {
        // Remove listeners when panel closes to prevent duplicates
        musicToggle.onValueChanged.RemoveListener(OnMusicChanged);
        sfxToggle.onValueChanged.RemoveListener(OnSfxChanged);
        vibrationToggle.onValueChanged.RemoveListener(OnVibrationChanged);
    }

    // ==============================
    // Save Settings
    // ==============================

    void OnMusicChanged(bool value)
    {
        GameData.MusicEnabled = value;
        PlayerPrefs.SetInt("music", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    void OnSfxChanged(bool value)
    {
        GameData.SfxEnabled = value;
        PlayerPrefs.SetInt("sfx", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    void OnVibrationChanged(bool value)
    {
        GameData.VibrationEnabled = value;
        PlayerPrefs.SetInt("vibration", value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
