using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public Toggle vibrationToggle;

    void OnEnable()
    {
        musicToggle.onValueChanged.AddListener(OnMusicChanged);
        sfxToggle.onValueChanged.AddListener(OnSfxChanged);
        vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
    }

    void OnDisable()
    {
        musicToggle.onValueChanged.RemoveListener(OnMusicChanged);
        sfxToggle.onValueChanged.RemoveListener(OnSfxChanged);
        vibrationToggle.onValueChanged.RemoveListener(OnVibrationChanged);
    }

    void OnMusicChanged(bool value)
    {
        GameCore.Instance.gameData.musicEnabled = value;
        GameCore.Instance.gameData.Save();
    }

    void OnSfxChanged(bool value)
    {
        GameCore.Instance.gameData.sfxEnabled = value;
        GameCore.Instance.gameData.Save();
    }

    void OnVibrationChanged(bool value)
    {
        GameCore.Instance.gameData.vibrationEnabled = value;
        GameCore.Instance.gameData.Save();
    }
}
