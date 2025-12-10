using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI player1CountText;
    [SerializeField] private TextMeshProUGUI player2CountText;
    [SerializeField] private TextMeshProUGUI winMessageText;

    [SerializeField] private GameObject player1TurnImage;
    [SerializeField] private GameObject player2TurnImage;

    [SerializeField] private RectTransform player1Holder;
    [SerializeField] private RectTransform player2Holder;

    [SerializeField] private RectTransform winPanel;
    [SerializeField] private Button restartButton;

    private bool isGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
    }

    private void OnDisable()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(OnRestartClicked);
    }

    public void InitializeUI()
    {
        winPanel.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        isGameOver = false;
        winMessageText.text = "";
        UpdateUI();
    }

    public void UpdateUI()
    {
        player1CountText.text = $"P1 Magnets: {GameManager.Instance.player1Magnets}";
        player2CountText.text = $"P2 Magnets: {GameManager.Instance.player2Magnets}";

        UpdateTurnIndicator(GameManager.Instance.currentTurn);
    }

    private void UpdateTurnIndicator(PlayerTurn turn)
    {
        if (isGameOver) return;

        bool isP1 = turn == PlayerTurn.Player1;

        player1TurnImage.SetActive(isP1);
        player2TurnImage.SetActive(!isP1);

        if (isP1)
        {
            player1Holder.SetAsLastSibling();
        }
        else
        {
            player2Holder.SetAsLastSibling();
        }
    }

    public void ShowWin(PlayerTurn winner)
    {
        isGameOver = true;

        winPanel.SetAsLastSibling();
        winPanel.gameObject.SetActive(true);

        restartButton.gameObject.SetActive(true);

        winMessageText.text = $"{winner} Wins!";

        player1TurnImage.SetActive(false);
        player2TurnImage.SetActive(false);
    }

    private void OnRestartClicked()
    {
        GameManager.Instance.OnRestartButtonClicked();
    }
}
