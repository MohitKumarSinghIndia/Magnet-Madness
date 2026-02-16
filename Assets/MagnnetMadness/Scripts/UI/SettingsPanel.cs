using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider vibrationSlider;

    [Header("Handle Images")]
    public Image musicHandle;
    public Image sfxHandle;
    public Image vibrationHandle;

    private Color onColor = Color.green;
    private Color offColor = Color.red;

    void OnEnable()
    {
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        vibrationSlider.onValueChanged.AddListener(OnVibrationChanged);

        SetupSliders();
        LoadSettings();
    }

    void OnDisable()
    {
        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        vibrationSlider.onValueChanged.RemoveListener(OnVibrationChanged);
    }

    private void SetupSliders()
    {
        SetupSlider(musicSlider);
        SetupSlider(sfxSlider);
        SetupSlider(vibrationSlider);
    }

    private void SetupSlider(Slider slider)
    {
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.wholeNumbers = true;
    }

    private void LoadSettings()
    {
        musicSlider.value = GameCore.Instance.gameData.musicEnabled ? 1 : 0;
        sfxSlider.value = GameCore.Instance.gameData.sfxEnabled ? 1 : 0;
        vibrationSlider.value = GameCore.Instance.gameData.vibrationEnabled ? 1 : 0;

        UpdateHandleColor(musicSlider.value, musicHandle);
        UpdateHandleColor(sfxSlider.value, sfxHandle);
        UpdateHandleColor(vibrationSlider.value, vibrationHandle);
    }

    // SFX PLAY METHOD
    private void PlayToggleSFX()
    {
        if (AudioManager.Instance != null &&
            AudioManager.Instance.buttonClick != null &&
            GameCore.Instance.gameData.sfxEnabled)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.toggleClick);
        }
    }

    void OnMusicChanged(float value)
    {
        bool enabled = value == 1;

        GameCore.Instance.gameData.musicEnabled = enabled;
        GameCore.Instance.gameData.Save();

        AudioManager.Instance.ApplySettings();

        if (enabled)
            AudioManager.Instance.PlayMusic();
        else
            AudioManager.Instance.StopMusic();

        UpdateHandleColor(value, musicHandle);

        PlayToggleSFX();
    }

    void OnSfxChanged(float value)
    {
        bool enabled = value == 1;

        GameCore.Instance.gameData.sfxEnabled = enabled;
        GameCore.Instance.gameData.Save();

        AudioManager.Instance.ApplySettings();

        UpdateHandleColor(value, sfxHandle);

        PlayToggleSFX();
    }

    void OnVibrationChanged(float value)
    {
        bool enabled = value == 1;

        GameCore.Instance.gameData.vibrationEnabled = enabled;
        GameCore.Instance.gameData.Save();

        UpdateHandleColor(value, vibrationHandle);

        PlayToggleSFX();
    }

    private void UpdateHandleColor(float value, Image handle)
    {
        handle.color = value == 1 ? onColor : offColor;
    }
}
