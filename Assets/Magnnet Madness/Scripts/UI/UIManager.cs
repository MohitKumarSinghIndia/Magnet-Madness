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

    [Header("Gameplay UI")]
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player1CountText;
    public TextMeshProUGUI player2CountText;

    public GameObject player1TurnImage;
    public GameObject player2TurnImage;

    public RectTransform player1Holder;
    public RectTransform player2Holder;

    [Header("Game Over UI")]
    public RectTransform GameOverPanel;
    public TextMeshProUGUI winMessageText;
    public Button restartButton;
    public Button homeButton;

    private bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
        ShowMainMenuOnly();
    }

    private void OnEnable()
    {
        restartButton.onClick.AddListener(() => GameManager.Instance.OnRestartButtonClicked());
        homeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ReturnToMainMenu();
            ShowMainMenuOnly();
        });
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
        homeButton.onClick.RemoveAllListeners();
    }

    // -------------------------------------------------------------------
    // PANEL NAVIGATION
    // -------------------------------------------------------------------

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

        HideAllMenuSubPanels();
        menuButtonsPanel.SetActive(true);
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
    }

    public void OnBackToMainMenu()
    {
        HideAllMenuSubPanels();
        menuButtonsPanel.SetActive(true);
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    // -------------------------------------------------------------------
    // START GAME
    // -------------------------------------------------------------------

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

    // -------------------------------------------------------------------
    // GAMEPLAY UI
    // -------------------------------------------------------------------

    public void InitializeGameplayUI()
    {
        isGameOver = false;
        GameOverPanel.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        winMessageText.text = "";

        player1NameText.text = GameCore.Instance.gameData.player1Name;
        player2NameText.text = GameCore.Instance.gameData.player2Name;

        UpdateGameplayUI();
    }

    public void UpdateGameplayUI()
    {
        player1CountText.text = $"{GameManager.Instance.player1Magnets}";
        player2CountText.text = $"{GameManager.Instance.player2Magnets}";

        UpdateTurnIndicator(GameManager.Instance.currentTurn);
    }

    private void UpdateTurnIndicator(PlayerTurn turn)
    {
        if (isGameOver) return;

        bool isP1 = turn == PlayerTurn.Player1;

        player1TurnImage.SetActive(isP1);
        player2TurnImage.SetActive(!isP1);

        if (isP1) player1Holder.SetAsLastSibling();
        else player2Holder.SetAsLastSibling();
    }

    public void ShowWin(string winnerName)
    {
        isGameOver = true;

        GameOverPanel.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);

        winMessageText.text = $"{winnerName} Wins!";

        player1TurnImage.SetActive(false);
        player2TurnImage.SetActive(false);
    }

}
