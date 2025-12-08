using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public enum PlayerTurn { Player1, Player2 }

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int initialMagnetCount = 5;
    public PlayerTurn startingPlayer = PlayerTurn.Player1;

    [Header("Game State")]
    [SerializeField] private PlayerTurn currentTurn;
    [SerializeField] private int player1MagnetCount;
    [SerializeField] private int player2MagnetCount;

    [Header("UI References")]
    public TextMeshProUGUI player1CountText;
    public TextMeshProUGUI player2CountText;
    public TextMeshProUGUI currentTurnText;
    public TextMeshProUGUI winMessageText;
    public GameObject restartButton;

    [Header("Magnet Holders")]
    public Transform player1MagnetHolder;
    public Transform player2MagnetHolder;

    [Header("Circle Area")]
    public Collider2D circleAreaCollider;

    private List<Magnet> magnetsInCircle = new List<Magnet>();
    private bool isGameOver = false;

    [Header("Magnet Spawner")]
    public MagnetSpawner magnetSpawner;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        // Initialize magnet counts
        player1MagnetCount = initialMagnetCount;
        player2MagnetCount = initialMagnetCount;

        // Set starting turn
        currentTurn = startingPlayer;

        // Clear magnets in circle
        magnetsInCircle.Clear();

        // Reset UI
        UpdateUI();
        winMessageText.gameObject.SetActive(false);
        restartButton.SetActive(false);
        isGameOver = false;

        Debug.Log("Game Initialized. Starting with: " + currentTurn);
    }

    public PlayerTurn GetCurrentTurn()
    {
        return currentTurn;
    }

    public int GetPlayerMagnetCount(PlayerTurn player)
    {
        return player == PlayerTurn.Player1 ? player1MagnetCount : player2MagnetCount;
    }

    public void AddMagnetToPlayer(PlayerTurn player, int amount = 1)
    {
        if (player == PlayerTurn.Player1)
        {
            player1MagnetCount += amount;
        }
        else
        {
            player2MagnetCount += amount;
        }

        UpdateUI();
        CheckWinConditions();
    }

    public void RemoveMagnetFromPlayer(PlayerTurn player, int amount = 1)
    {
        if (player == PlayerTurn.Player1)
        {
            player1MagnetCount -= amount;
            player1MagnetCount = Mathf.Max(0, player1MagnetCount);
        }
        else
        {
            player2MagnetCount -= amount;
            player2MagnetCount = Mathf.Max(0, player2MagnetCount);
        }

        UpdateUI();
        CheckWinConditions();
    }

    public void RegisterMagnetInCircle(Magnet magnet)
    {
        if (!magnetsInCircle.Contains(magnet))
        {
            magnetsInCircle.Add(magnet);
        }
    }

    public void UnregisterMagnetFromCircle(Magnet magnet)
    {
        magnetsInCircle.Remove(magnet);
    }

    public List<Magnet> GetMagnetsInCircle()
    {
        return new List<Magnet>(magnetsInCircle);
    }

    public void SwitchTurn()
    {
        if (isGameOver) return;

        // Switch to the other player
        currentTurn = (currentTurn == PlayerTurn.Player1) ?
                     PlayerTurn.Player2 : PlayerTurn.Player1;

        UpdateUI();

        Debug.Log("Turn switched to: " + currentTurn);
    }

    void UpdateUI()
    {
        if (player1CountText != null)
            player1CountText.text = "Player 1: " + player1MagnetCount;

        if (player2CountText != null)
            player2CountText.text = "Player 2: " + player2MagnetCount;

        if (currentTurnText != null)
            currentTurnText.text = "Current Turn: " + currentTurn.ToString();
    }

    void CheckWinConditions()
    {
        if (player1MagnetCount <= 0)
        {
            EndGame(PlayerTurn.Player2);
        }
        else if (player2MagnetCount <= 0)
        {
            EndGame(PlayerTurn.Player1);
        }
    }

    void EndGame(PlayerTurn winner)
    {
        isGameOver = true;

        string winnerText = (winner == PlayerTurn.Player1) ? "Player 1" : "Player 2";
        winMessageText.text = winnerText + " Wins!";
        winMessageText.gameObject.SetActive(true);
        restartButton.SetActive(true);

        Debug.Log("Game Over! Winner: " + winnerText);
    }

    public void RestartGame()
    {
        // Destroy all magnets in circle
        foreach (Magnet magnet in magnetsInCircle)
        {
            if (magnet != null)
                Destroy(magnet.gameObject);
        }
        magnetsInCircle.Clear();

        // Destroy all magnets in holders
        DestroyMagnetsInHolder(player1MagnetHolder);
        DestroyMagnetsInHolder(player2MagnetHolder);

        // Reinitialize game
        InitializeGame();
    }

    void DestroyMagnetsInHolder(Transform holder)
    {
        if (holder == null) return;

        foreach (Transform child in holder)
        {
            Destroy(child.gameObject);
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}