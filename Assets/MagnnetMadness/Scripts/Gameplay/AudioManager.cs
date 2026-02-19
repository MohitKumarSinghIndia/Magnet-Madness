using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    public AudioClip backgroundMusic;

    [Header("UI Sounds")]
    public AudioClip buttonClick;

    [Header("Game Sounds")]
    public AudioClip magnetCollapse;
    public AudioClip coinCollect;
    public AudioClip winning;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        ApplySettings();

        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;

            if (GameCore.Instance.gameData.musicEnabled)
                musicSource.Play();
        }
    }

    #region SETTINGS SYNC

    public void ApplySettings()
    {
        bool musicEnabled = GameCore.Instance.gameData.musicEnabled;
        bool sfxEnabled = GameCore.Instance.gameData.sfxEnabled;

        musicSource.mute = !musicEnabled;
        sfxSource.mute = !sfxEnabled;
    }

    #endregion

    #region MUSIC

    public void PlayMusic()
    {
        if (!GameCore.Instance.gameData.musicEnabled) return;

        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    #endregion

    #region UI SFX

    public void PlayButtonClick()
    {
        PlaySFX(buttonClick);
    }

    #endregion

    #region GAME SFX

    public void PlayMagnetCollapse()
    {
        PlaySFX(magnetCollapse);
    }

    public void PlayWinning()
    {
        PlaySFX(winning);
    }

    public void PlayCoin()
    {
        if (!GameCore.Instance.gameData.sfxEnabled) return;
        if (coinCollect == null) return;

        sfxSource.pitch = Random.Range(0.9f, 1.1f);
        sfxSource.PlayOneShot(coinCollect);
        sfxSource.pitch = 1f;
    }

    #endregion

    #region CORE SFX METHOD

    public void PlaySFX(AudioClip clip)
    {
        if (!GameCore.Instance.gameData.sfxEnabled) return;
        if (clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    #endregion
}
