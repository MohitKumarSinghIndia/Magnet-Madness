using System.Collections.Generic;
using UnityEngine;

public enum PlayerTurn
{
    Player1,
    Player2
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    #region VARIABLES

    [Header("Players Magnet Holders")]
    public Transform player1MagnetHolder;
    public Transform player2MagnetHolder;

    [Header("Players Magnets")]
    public int player1Magnets;
    public int player2Magnets;
    [Space(10)]
    public int player1PlacedMagnets;
    public int player2PlacedMagnets;

    [Header("Current Player Turn")]
    public PlayerTurn currentTurn = PlayerTurn.Player1;
    public Magnet activeDraggingMagnet;

    [Header("Circle Area")]
    public CircleCollider2D circleAreaCollider;

    [Header("Spawner")]
    public MagnetSpawner magnetSpawner;

    [Header("Gameplay Settings")]
    public int initialMagnetCount = 5;
    public float turnDuration = 30f;

    [Header("Ad Settings")]
    private int gameOverCount = 0;

    [Header("Private Fields")]
    private List<Magnet> magnetsInCircle = new();
    private bool isGameOver = false;
    private float currentTurnTime;
    private bool isTimerRunning;

    #endregion

    #region Unity Lifecycle

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
        gameOverCount = 0;
    }

    private void Update()
    {
        HandleTurnTimer();
    }

    #endregion

    #region Initialization

    public void InitializeGame()
    {
        isGameOver = false;

        player1Magnets = initialMagnetCount;
        player2Magnets = initialMagnetCount;

        player1PlacedMagnets = 0;
        player2PlacedMagnets = 0;

        magnetsInCircle.Clear();
        currentTurn = PlayerTurn.Player1;

        ClearHolder(player1MagnetHolder);
        ClearHolder(player2MagnetHolder);

        magnetSpawner.SpawnPlayerMagnets();

        UIManager.Instance.InitializeGameplayUI();
        UIManager.Instance.LoadGameplayPanel();
        UIManager.Instance.UpdateGameplayUI();

        StartTurnTimer();

        if (GoogleAdsManager.Instance != null)
            GoogleAdsManager.Instance.ShowBanner();
    }

    #endregion

    #region TURN TIMER

    private void HandleTurnTimer()
    {
        if (!isTimerRunning || isGameOver) return;

        currentTurnTime -= Time.deltaTime;
        currentTurnTime = Mathf.Max(0f, currentTurnTime);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateTurnTimer(currentTurnTime);

        if (currentTurnTime <= 0f)
        {
            isTimerRunning = false;
            OnTurnTimeOver();
        }
    }

    private void StartTurnTimer()
    {
        currentTurnTime = turnDuration;
        isTimerRunning = true;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateTurnTimer(currentTurnTime);
    }
    public void OnTurnTimeOver()
    {
        if (activeDraggingMagnet != null)
        {
            activeDraggingMagnet.ForceReturnToSlot();
            activeDraggingMagnet = null;
        }

        SwitchTurn();
    }

    private void StopTurnTimer()
    {
        isTimerRunning = false;
    }

    #endregion

    #region Turn System

    public void SwitchTurn()
    {
        if (isGameOver) return;

        currentTurn = currentTurn == PlayerTurn.Player1
            ? PlayerTurn.Player2
            : PlayerTurn.Player1;

        StartTurnTimer();
        UIManager.Instance.UpdateGameplayUI();
    }

    #endregion

    #region Magnet Counts

    public void AddMagnetToPlayer(PlayerTurn player)
    {
        if (player == PlayerTurn.Player1)
            player1Magnets++;
        else
            player2Magnets++;

        UIManager.Instance.UpdateGameplayUI();
    }

    public void RemoveMagnetFromPlayer(PlayerTurn player)
    {
        if (player == PlayerTurn.Player1)
            player1Magnets = Mathf.Max(0, player1Magnets - 1);
        else
            player2Magnets = Mathf.Max(0, player2Magnets - 1);

        UIManager.Instance.UpdateGameplayUI();
    }

    public void AddPlaced(PlayerTurn player)
    {
        if (player == PlayerTurn.Player1)
            player1PlacedMagnets++;
        else
            player2PlacedMagnets++;
    }

    public void RemovePlaced(PlayerTurn player)
    {
        if (player == PlayerTurn.Player1)
            player1PlacedMagnets = Mathf.Max(0, player1PlacedMagnets - 1);
        else
            player2PlacedMagnets = Mathf.Max(0, player2PlacedMagnets - 1);
    }

    #endregion

    #region Win Check

    public void CheckWinCondition()
    {
        if (isGameOver) return;

        if (player1Magnets <= 0)
            HandleGameOver(GameCore.Instance.gameData.player1Name);
        else if (player2Magnets <= 0)
            HandleGameOver(GameCore.Instance.gameData.player2Name);
    }

    private void HandleGameOver(string winnerName)
    {
        if (isGameOver) return;

        isGameOver = true;
        StopTurnTimer();

        UIManager.Instance.ShowWin(winnerName);
        TryShowInterstitialOnGameOver();
    }

    public bool IsGameOver() => isGameOver;

    #endregion

    #region Ads

    public void TryShowInterstitialOnGameOver()
    {
        if (GoogleAdsManager.Instance == null) return;

        gameOverCount++;

        if (gameOverCount >= GoogleAdsManager.Instance.adsDisplayInterval)
        {
            GoogleAdsManager.Instance.HideBanner();
            GoogleAdsManager.Instance.ShowInterstitial();
            gameOverCount = 0;
        }
    }

    #endregion

    #region Circle Register

    public void RegisterMagnetInCircle(Magnet magnet)
    {
        if (!magnetsInCircle.Contains(magnet))
            magnetsInCircle.Add(magnet);
    }

    public void UnregisterMagnetFromCircle(Magnet magnet)
    {
        if (magnetsInCircle.Contains(magnet))
            magnetsInCircle.Remove(magnet);
    }

    public List<Magnet> GetMagnetsInCircle() => magnetsInCircle;

    #endregion

    #region Restart / Main Menu

    public void OnRestartButtonClicked()
    {
        foreach (Magnet magnet in new List<Magnet>(magnetsInCircle))
        {
            if (magnet != null)
                Destroy(magnet.gameObject);
        }

        magnetsInCircle.Clear();
        InitializeGame();
    }

    public void ReturnToMainMenu()
    {
        gameOverCount = 0;
        StopTurnTimer();

        foreach (Magnet magnet in new List<Magnet>(magnetsInCircle))
        {
            if (magnet != null)
                Destroy(magnet.gameObject);
        }

        magnetsInCircle.Clear();
        ClearHolder(player1MagnetHolder);
        ClearHolder(player2MagnetHolder);
    }

    #endregion

    #region Utility

    private void ClearHolder(Transform holder)
    {
        foreach (Transform slot in holder)
        {
            if (slot.childCount > 0)
                Destroy(slot.GetChild(0).gameObject);
        }
    }

    #endregion
}
