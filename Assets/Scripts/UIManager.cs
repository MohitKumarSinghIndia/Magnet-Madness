using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI player1CountText;
    public TextMeshProUGUI player2CountText;
    public TextMeshProUGUI currentTurnText;
    public TextMeshProUGUI winMessageText;
    public GameObject restartButton;
    public GameObject dragAreaPanel;

    [Header("Colors")]
    public Color player1Color = Color.blue;
    public Color player2Color = Color.red;
    public Color activeTurnColor = Color.yellow;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (GameManager.Instance == null) return;

        // Update player counts
        player1CountText.text = "Player 1: " + GameManager.Instance.GetPlayerMagnetCount(PlayerTurn.Player1);
        player2CountText.text = "Player 2: " + GameManager.Instance.GetPlayerMagnetCount(PlayerTurn.Player2);

        // Update turn with color coding
        PlayerTurn currentTurn = GameManager.Instance.GetCurrentTurn();
        currentTurnText.text = "Current Turn: " + currentTurn.ToString();

        if (currentTurn == PlayerTurn.Player1)
        {
            currentTurnText.color = player1Color;
            player1CountText.color = activeTurnColor;
            player2CountText.color = Color.white;
        }
        else
        {
            currentTurnText.color = player2Color;
            player2CountText.color = activeTurnColor;
            player1CountText.color = Color.white;
        }

        // Show/hide win message
        winMessageText.gameObject.SetActive(GameManager.Instance.IsGameOver());
        restartButton.SetActive(GameManager.Instance.IsGameOver());

        // Show/hide drag area based on turn (for visual feedback)
        if (dragAreaPanel != null)
        {
            // Optional: Visual feedback for active player's area
        }
    }

    // Called by Restart Button
    public void OnRestartButtonClick()
    {
        GameManager.Instance.RestartGame();
    }
}