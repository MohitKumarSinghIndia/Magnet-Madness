using System;
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

    #region Gameplay Settings

    [Header("Gameplay Settings")]
    public int initialMagnetCount = 5;

    [Header("Ad Settings")]
    private int gameOverCount = 0;

    [Header("Player Magnet Holders")]
    public Transform player1MagnetHolder;
    public Transform player2MagnetHolder;

    [Header("Circle Area")]
    public CircleCollider2D circleAreaCollider;

    [Header("Spawner")]
    public MagnetSpawner magnetSpawner;

    [Header("Players Magnets")]
    [Space(10)]
    public int player1Magnets;
    public int player2Magnets;

    [Space(10)]
    public int player1PlacedMagnets;
    public int player2PlacedMagnets;

    [Header("Current Player Turn")]
    [Space(5)]
    public PlayerTurn currentTurn = PlayerTurn.Player1;

    private List<Magnet> magnetsInCircle = new List<Magnet>();

    private bool isGameOver = false;

    #endregion

    #region Unity

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        gameOverCount = 0;
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

        if (GoogleAdsManager.Instance != null)
            GoogleAdsManager.Instance.ShowBanner();
    }

    #endregion

    #region Turn System

    public void SwitchTurn()
    {
        if (isGameOver)
            return;

        currentTurn = currentTurn == PlayerTurn.Player1
            ? PlayerTurn.Player2
            : PlayerTurn.Player1;

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
        if (isGameOver)
            return;

        if (player1Magnets <= 0)
        {
            HandleGameOver(GameCore.Instance.gameData.player1Name);
        }
        else if (player2Magnets <= 0)
        {
            HandleGameOver(GameCore.Instance.gameData.player2Name);
        }
    }

    private void HandleGameOver(string winnerName)
    {
        if (isGameOver)
            return;

        isGameOver = true;

        UIManager.Instance.ShowWin(winnerName);
        TryShowInterstitialOnGameOver();
    }

    public void TryShowInterstitialOnGameOver()
    {
        if (GoogleAdsManager.Instance == null)
            return;

        gameOverCount++;

        if (gameOverCount >= GoogleAdsManager.Instance.adsDisplayInterval)
        {
            GoogleAdsManager.Instance.HideBanner();
            GoogleAdsManager.Instance.ShowInterstitial();
            gameOverCount = 0;
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
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

    public List<Magnet> GetMagnetsInCircle()
    {
        return magnetsInCircle;
    }

    #endregion

    #region Restart Game

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

    #endregion

    #region Main Menu

    public void ReturnToMainMenu()
    {
        gameOverCount = 0;

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
