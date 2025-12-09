using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI player1CountText;
    public TextMeshProUGUI player2CountText;
    public TextMeshProUGUI currentTurnText;
    public TextMeshProUGUI winMessageText;
    public GameObject winPanel;
    public Button restartButton;
    public GameManager gameManager;

    void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        restartButton.onClick.AddListener(gameManager.OnRestartButtonClicked);
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(gameManager.OnRestartButtonClicked);
    }

    // -------------------------------------------------------
    // INITIALIZE UI (call this once on game start or restart)
    // -------------------------------------------------------
    public void InitializeUI()
    {
        winPanel.SetActive(false);
        restartButton.gameObject.SetActive(false);
        winMessageText.text = "";

        UpdateUI(); // Refresh magnet counts + turn UI
    }

    // -------------------------------------------------------
    // UPDATE IN-GAME UI (counts, turn text)
    // -------------------------------------------------------
    public void UpdateUI()
    {
        player1CountText.text = "P1 Magnets: " + GameManager.Instance.player1Magnets;
        player2CountText.text = "P2 Magnets: " + GameManager.Instance.player2Magnets;
        currentTurnText.text = "Turn: " + GameManager.Instance.currentTurn;
    }

    // -------------------------------------------------------
    // SHOW WIN PANEL
    // -------------------------------------------------------
    public void ShowWin(PlayerTurn winner)
    {
        winMessageText.text = winner + " Wins!";
        winPanel.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }
}
