using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public Toggle vibrationToggle;

    void Start()
    {
        LoadSettings();
    }

    public void OnMusicChanged()
    {
        GameData.MusicEnabled = musicToggle.isOn;
        PlayerPrefs.SetInt("music", musicToggle.isOn ? 1 : 0);
    }

    public void OnSfxChanged()
    {
        GameData.SfxEnabled = sfxToggle.isOn;
        PlayerPrefs.SetInt("sfx", sfxToggle.isOn ? 1 : 0);
    }

    public void OnVibrationChanged()
    {
        GameData.VibrationEnabled = vibrationToggle.isOn;
        PlayerPrefs.SetInt("vibration", vibrationToggle.isOn ? 1 : 0);
    }

    void LoadSettings()
    {
        GameData.MusicEnabled = PlayerPrefs.GetInt("music", 1) == 1;
        GameData.SfxEnabled = PlayerPrefs.GetInt("sfx", 1) == 1;
        GameData.VibrationEnabled = PlayerPrefs.GetInt("vibration", 1) == 1;

        musicToggle.isOn = GameData.MusicEnabled;
        sfxToggle.isOn = GameData.SfxEnabled;
        vibrationToggle.isOn = GameData.VibrationEnabled;
    }
}
