using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    #region VARIABLES
    [Header("-----------UI-------------")]

    [Header("Containers")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;

    [Header("-----------MAIN MENU UI-------------")]

    [Header("Home Screen Panels")]
    public GameObject homePanel;
    public GameObject settingsPanel;
    public GameObject shopPanel;
    public GameObject playerNamePanel;
    public GameObject aboutPanel;

    [Header("Player Input Fields")]
    public TMP_InputField player1Input;
    public TMP_InputField player2Input;

    [Header("Coin Panel UI")]
    private int coinAmount;
    public TextMeshProUGUI coinsText;

    [Header("Bounce Targets")]
    public RectTransform logoTransform;
    public RectTransform playButtonTransform;

    [Header("Transition Settings")]
    public float panelAnimDuration = 0.35f;

    [Header("-----------GAME UI-------------")]

    [Header("Player UI")]
    public RectTransform player1Holder;
    public RectTransform player2Holder;
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player1CountText;
    public TextMeshProUGUI player2CountText;

    [Header("Timer UI")]
    public float warningTimeThreshold = 10f;
    public TextMeshProUGUI turnTimerText;
    private Color normalTimerColor = Color.white;
    private Color warningTimerColor = Color.red;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button gameOverPanelHomeButton;
    public TextMeshProUGUI winMessageText;

    [Header("Pause Panel")]
    public GameObject pausePanel;
    public Button pauseButton;
    public Button resumeButton;
    public Button pausePanelHomeButton;

    [Header("Private Fields")]
    private bool isGameOver = false;
    private bool isPaused = false;
    private bool isPanelAnimating = false;

    private CanvasGroup player1CG;
    private CanvasGroup player2CG;
    private Sequence logoBounceSeq;
    private Tween playButtonTween;

    #endregion

    #region UNITY METHODS

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        ShowMainMenuOnly();

        player1CG = player1Holder.GetComponent<CanvasGroup>();
        player2CG = player2Holder.GetComponent<CanvasGroup>();

        if (player1CG == null || player2CG == null)
            Debug.LogError("CanvasGroup missing on player holders!");
    }

    private void OnEnable()
    {
        if (gameOverPanel != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
            gameOverPanelHomeButton.onClick.AddListener(OnHomeClicked);
        }

        if (pausePanel != null)
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
            resumeButton.onClick.AddListener(OnResumeClicked);
            pausePanelHomeButton.onClick.AddListener(OnPausePanelHomeClicked);
        }
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(OnRestartClicked);
        gameOverPanelHomeButton.onClick.RemoveListener(OnHomeClicked);

        pauseButton.onClick.RemoveListener(OnPauseClicked);
        resumeButton.onClick.RemoveListener(OnResumeClicked);
        pausePanelHomeButton.onClick.RemoveListener(OnPausePanelHomeClicked);
    }

    #endregion

    #region START GAME

    public void OnStartGameClicked()
    {
        PlayButtonClickSound();

        if (string.IsNullOrWhiteSpace(player1Input.text) ||
            string.IsNullOrWhiteSpace(player2Input.text))
        {
            Debug.LogWarning("Player names cannot be empty!");
            return;
        }

        GameCore.Instance.gameData.player1Name = player1Input.text;
        GameCore.Instance.gameData.player2Name = player2Input.text;
        GameCore.Instance.gameData.Save();

        GameManager.Instance.InitializeGame();
    }

    #endregion

    #region PANEL NAVIGATION

    private void AnimatePanelOpen(GameObject panel, UnityAction onCompleteEvent = null)
    {
        isPanelAnimating = false;

        panel.SetActive(true);

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        panel.transform.DOKill(true);
        cg.DOKill(true);

        panel.transform.localScale = Vector3.zero;
        cg.alpha = 0f;

        isPanelAnimating = true;

        Sequence seq = DOTween.Sequence();
        seq.Append(panel.transform
            .DOScale(Vector3.one, panelAnimDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true));

        seq.Join(cg
            .DOFade(1f, panelAnimDuration)
            .SetUpdate(true));

        seq.OnComplete(() =>
        {
            isPanelAnimating = false;
            onCompleteEvent?.Invoke();
        });

        if (panel == homePanel)
            StartHomeAnimation();
    }

    private void HideAllMenuSubPanels()
    {
        StopHomeAnimation();

        homePanel.SetActive(false);
        playerNamePanel.SetActive(false);
        settingsPanel.SetActive(false);
        shopPanel.SetActive(false);
        aboutPanel.SetActive(false); 
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void ShowPanel(GameObject target)
    {
        HideAllMenuSubPanels();
        AnimatePanelOpen(target);
        RefreshCoinsUI();
    }

    public void ShowMainMenuOnly()
    {
        gameplayPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        player1Input.text = string.Empty;
        player2Input.text = string.Empty;

        HideAllMenuSubPanels();
        AnimatePanelOpen(homePanel);

        RefreshCoinsUI();
    }

    public void LoadGameplayPanel()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(true);
    }

    public void OnPlayClicked()
    {
        PlayButtonClickSound();
        ShowPanel(playerNamePanel);

        player1Input.text = "Player 1";
        player2Input.text = "Player 2";
    }

    public void OnSettingsButtonClicked()
    {
        PlayButtonClickSound();
        ShowPanel(settingsPanel);
    }

    public void OnHomeButtonClicked()
    {
        PlayButtonClickSound();
        ShowMainMenuOnly();
    }

    public void OnShopButtonClicked()
    {
        PlayButtonClickSound();
        ShowPanel(shopPanel);
    }

    public void OnAboutClicked()
    {
        PlayButtonClickSound();
        ShowPanel(aboutPanel);
    }

    public void OnBackToMainMenu()
    {
        PlayButtonClickSound();
        HideAllMenuSubPanels();
        AnimatePanelOpen(homePanel);
        RefreshCoinsUI();
    }

    #endregion

    #region GAMEPLAY UI

    public void InitializeGameplayUI()
    {
        isGameOver = false;
        isPaused = false;

        gameOverPanel.SetActive(false);
        winMessageText.text = "";

        player1NameText.text = GameCore.Instance.gameData.player1Name;
        player2NameText.text = GameCore.Instance.gameData.player2Name;

        turnTimerText.color = normalTimerColor;
        UpdateTurnTimer(GameManager.Instance.turnDuration);

        UpdateGameplayUI();
    }

    public void UpdateGameplayUI()
    {
        player1CountText.text = GameManager.Instance.player1Magnets.ToString();
        player2CountText.text = GameManager.Instance.player2Magnets.ToString();

        UpdateTurnIndicator(GameManager.Instance.currentTurn);
    }

    public void ShowWin(string winnerName)
    {
        isGameOver = true;

        GameCore.Instance.gameData.AddCoins(50);
        RefreshCoinsUI();

        winMessageText.text = $"<size=200>{winnerName}</size>\nWins!";

        AnimatePanelOpen(gameOverPanel);

        SetCanvasGroupAlpha(player1CG, 0.5f);
        SetCanvasGroupAlpha(player2CG, 0.5f);
    }

    private void OnRestartClicked()
    {
        GameManager.Instance.OnRestartButtonClicked();
        PlayButtonClickSound();
    }

    private void OnHomeClicked()
    {
        GameManager.Instance.ReturnToMainMenu();
        PlayButtonClickSound();
        ShowMainMenuOnly();
    }

    private void OnPauseClicked()
    {
        if (isPaused || GameManager.Instance.IsGameOver()) return;

        isPaused = true;

        HideAllMenuSubPanels();
        AnimatePanelOpen(pausePanel, () => { Time.timeScale = 0f; });

        PlayButtonClickSound();
    }

    private void OnResumeClicked()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        pausePanel.SetActive(false);

        PlayButtonClickSound();
    }

    private void OnPausePanelHomeClicked()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ReturnToMainMenu();
        PlayButtonClickSound();
        ShowMainMenuOnly();
    }

    #endregion

    #region TURN SYSTEM

    private void UpdateTurnIndicator(PlayerTurn turn)
    {
        if (isGameOver) return;

        bool isP1 = turn == PlayerTurn.Player1;

        SetCanvasGroupAlpha(player1CG, isP1 ? 1f : 0.5f);
        SetCanvasGroupAlpha(player2CG, isP1 ? 0.5f : 1f);

        if (isP1) player1Holder.SetAsLastSibling();
        else player2Holder.SetAsLastSibling();
    }

    private void SetCanvasGroupAlpha(CanvasGroup cg, float alpha)
    {
        if (cg != null)
            cg.alpha = alpha;
    }

    public void UpdateTurnTimer(float time)
    {
        if (turnTimerText == null) return;

        time = Mathf.Clamp(time, 0f, 999f);
        turnTimerText.text = Mathf.CeilToInt(time) + "s";

        if (time <= warningTimeThreshold)
            turnTimerText.color = warningTimerColor;
        else
            turnTimerText.color = normalTimerColor;
    }

    #endregion

    #region HOME BOUNCE SYSTEM

    private void StartHomeAnimation()
    {
        LoopLogoBounce(logoTransform);

        PlayButtonPulse(playButtonTransform);
    }

    private void StopHomeAnimation()
    {
        if (logoBounceSeq != null)
            logoBounceSeq.Kill();

        if (playButtonTween != null)
            playButtonTween.Kill();

        if (logoTransform != null)
            logoTransform.localScale = Vector3.one;

        if (playButtonTransform != null)
            playButtonTransform.localScale = Vector3.one;
    }

    private void LoopLogoBounce(RectTransform target)
    {
        if (target == null) return;

        if (logoBounceSeq != null && logoBounceSeq.IsActive())
            logoBounceSeq.Kill();

        target.localScale = Vector3.one;

        logoBounceSeq = DOTween.Sequence();

        logoBounceSeq.Append(target.DOScale(1.15f, 0.5f).SetEase(Ease.OutQuad));
        logoBounceSeq.Append(target.DOScale(1f, 0.5f).SetEase(Ease.OutElastic));
    }

    private void PlayButtonPulse(RectTransform target)
    {
        if (target == null) return;

        if (playButtonTween != null && playButtonTween.IsActive())
            playButtonTween.Kill();

        target.localScale = Vector3.one;

        playButtonTween = target.DOScale(1.05f, 2.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    #endregion

    #region COINS UI

    public void RefreshCoinsUI()
    {
        if (GameCore.Instance == null || GameCore.Instance.gameData == null)
        {
            Debug.LogWarning("GameCore or GameData not ready yet");
            return;
        }

        coinAmount = GameCore.Instance.gameData.GetCoins();

        if (coinsText != null)
            coinsText.text = coinAmount.ToString();
    }

    #endregion

    #region BUTTON SOUNDS
    private void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.buttonClick != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
        }
    }

    #endregion

    #region ADS

    public void ShowAdForCoins()
    {
        int coins = (int)Random.Range(5, 50);

        GoogleAdsManager.Instance.ShowRewarded(() =>
        {
            GameCore.Instance.gameData.AddCoins(coins);
            RefreshCoinsUI();
        });
    }

    #endregion
}