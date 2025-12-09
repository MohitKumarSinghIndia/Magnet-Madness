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
    public Button restartButton;
    public GameManager gameManager;

    void Awake()
    {
        Instance = this;
        restartButton.gameObject.SetActive(false);
        winMessageText.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        restartButton.onClick.AddListener(gameManager.OnRestartButtonClicked);
    }
    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(gameManager.OnRestartButtonClicked);
    }
    public void UpdateUI()
    {
        player1CountText.text = "P1 Magnets: " + GameManager.Instance.player1Magnets;
        player2CountText.text = "P2 Magnets: " + GameManager.Instance.player2Magnets;

        currentTurnText.text = "Turn: " + GameManager.Instance.currentTurn;
    }

    public void ShowWin(PlayerTurn winner)
    {
        winMessageText.text = winner + " Wins!";
        winMessageText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }
}
