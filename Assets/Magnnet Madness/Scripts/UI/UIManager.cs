using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main Containers")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;

    [Header("Menu Panels")]
    public GameObject menuButtonsPanel;
    public GameObject playerNamePanel;
    public GameObject settingsPanel;
    public GameObject skinSelectionPanel;
    public GameObject aboutPanel;

    [Header("Player Input Fields")]
    public TMP_InputField player1Input;
    public TMP_InputField player2Input;

    [Header("Top Panel UI")]
    [SerializeField] private int coinAmount;
    public TextMeshProUGUI coinsText;

    [Header("Gameplay UI")]
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player1CountText;
    public TextMeshProUGUI player2CountText;

    [Header("Turn Timer UI")]
    public TextMeshProUGUI turnTimerText;

    [Header("Turn Timer Colors")]
    public Color normalTimerColor = Color.white;
    public Color warningTimerColor = Color.red;
    public float warningTimeThreshold = 10f;

    private CanvasGroup player1CG;
    private CanvasGroup player2CG;

    public RectTransform player1Holder;
    public RectTransform player2Holder;

    [Header("Game Over UI")]
    public RectTransform gameOverPanel;
    public TextMeshProUGUI winMessageText;
    public Button restartButton;
    public Button homeButton;

    private bool isGameOver = false;

    #region Unity Methods

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
        restartButton.onClick.AddListener(OnRestartClicked);
        homeButton.onClick.AddListener(OnHomeClicked);
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(OnRestartClicked);
        homeButton.onClick.RemoveListener(OnHomeClicked);
    }

    #endregion

    #region PANEL NAVIGATION

    private void HideAllMenuSubPanels()
    {
        menuButtonsPanel.SetActive(false);
        playerNamePanel.SetActive(false);
        settingsPanel.SetActive(false);
        skinSelectionPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    public void ShowMainMenuOnly()
    {
        gameplayPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        player1Input.text = string.Empty;
        player2Input.text = string.Empty;

        HideAllMenuSubPanels();
        menuButtonsPanel.SetActive(true);

        RefreshCoinsUI();
    }

    public void LoadGameplayPanel()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(true);
    }

    public void OnPlayClicked() => ShowPanel(playerNamePanel);
    public void OnSettingsClicked() => ShowPanel(settingsPanel);
    public void OnSkinSelectClicked() => ShowPanel(skinSelectionPanel);
    public void OnAboutClicked() => ShowPanel(aboutPanel);

    private void ShowPanel(GameObject target)
    {
        HideAllMenuSubPanels();
        target.SetActive(true);
        RefreshCoinsUI();
    }

    public void OnBackToMainMenu()
    {
        HideAllMenuSubPanels();
        menuButtonsPanel.SetActive(true);
        RefreshCoinsUI();
    }

    #endregion

    #region START GAME

    public void OnStartGameClicked()
    {
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

    #region GAMEPLAY UI

    public void InitializeGameplayUI()
    {
        isGameOver = false;

        gameOverPanel.gameObject.SetActive(false);
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

    #endregion

    #region TURN INDICATOR

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

    #endregion

    #region TURN TIMER UI

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

    #region WIN PANEL

    public void ShowWin(string winnerName)
    {
        isGameOver = true;

        GameCore.Instance.gameData.AddCoins(50);
        RefreshCoinsUI();

        gameOverPanel.gameObject.SetActive(true);
        gameOverPanel.SetAsLastSibling();

        winMessageText.text = $"{winnerName} Wins!";

        SetCanvasGroupAlpha(player1CG, 0.5f);
        SetCanvasGroupAlpha(player2CG, 0.5f);
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

    #region BUTTON CALLBACKS

    private void OnRestartClicked()
    {
        GameManager.Instance.OnRestartButtonClicked();
    }

    private void OnHomeClicked()
    {
        GameManager.Instance.ReturnToMainMenu();
        ShowMainMenuOnly();
    }

    #endregion
}
