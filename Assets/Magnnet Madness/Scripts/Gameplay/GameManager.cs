using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerTurn { Player1, Player2 }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Gameplay Settings")]
    public int initialMagnetCount = 5;

    [Header("Player Magnet Holders")]
    public Transform player1MagnetHolder;
    public Transform player2MagnetHolder;

    [Header("Circle Area")]
    public CircleCollider2D circleAreaCollider;

    [Header("Spawner")]
    public MagnetSpawner magnetSpawner;

    public int player1Magnets;
    public int player2Magnets;

    public int player1Placed = 0;
    public int player2Placed = 0;

    public PlayerTurn currentTurn = PlayerTurn.Player1;

    private List<Magnet> magnetsInCircle = new List<Magnet>();

    private void Awake()
    {
        Instance = this;
    }

    // -------------------------------------------------------------------
    // INITIALIZATION
    // -------------------------------------------------------------------

    public void InitializeGame()
    {
        player1Magnets = initialMagnetCount;
        player2Magnets = initialMagnetCount;

        player1Placed = 0;
        player2Placed = 0;

        magnetsInCircle.Clear();

        currentTurn = PlayerTurn.Player1;

        ClearHolder(player1MagnetHolder);
        ClearHolder(player2MagnetHolder);

        magnetSpawner.SpawnPlayerMagnets();

        UIManager.Instance.InitializeGameplayUI();

        UIManager.Instance.LoadGameplayPanel();
    }

    // -------------------------------------------------------------------
    // TURN SYSTEM
    // -------------------------------------------------------------------

    public void SwitchTurn()
    {
        if (IsGameOver()) return;

        currentTurn = (currentTurn == PlayerTurn.Player1) ? PlayerTurn.Player2 : PlayerTurn.Player1;
        UIManager.Instance.UpdateGameplayUI();
    }

    // -------------------------------------------------------------------
    // MAGNET COUNTS
    // -------------------------------------------------------------------

    public void AddMagnetToPlayer(PlayerTurn p)
    {
        if (p == PlayerTurn.Player1) player1Magnets++;
        else player2Magnets++;

        UIManager.Instance.UpdateGameplayUI();
    }

    public void RemoveMagnetFromPlayer(PlayerTurn p)
    {
        if (p == PlayerTurn.Player1) player1Magnets--;
        else player2Magnets--;

        UIManager.Instance.UpdateGameplayUI();
    }

    public void AddPlaced(PlayerTurn p)
    {
        if (p == PlayerTurn.Player1) player1Placed++;
        else player2Placed++;
    }

    public void RemovePlaced(PlayerTurn p)
    {
        if (p == PlayerTurn.Player1) player1Placed--;
        else player2Placed--;
    }

    // -------------------------------------------------------------------
    // WIN CHECK
    // -------------------------------------------------------------------

    public void CheckWinCondition()
    {
        if (player1Placed == initialMagnetCount)
            UIManager.Instance.ShowWin(GameCore.Instance.gameData.player1Name);

        if (player2Placed == initialMagnetCount)
            UIManager.Instance.ShowWin(GameCore.Instance.gameData.player1Name);
    }

    // -------------------------------------------------------------------
    // CIRCLE REGISTER
    // -------------------------------------------------------------------

    public void RegisterMagnetInCircle(Magnet m)
    {
        if (!magnetsInCircle.Contains(m))
            magnetsInCircle.Add(m);
    }

    public void UnregisterMagnetFromCircle(Magnet m)
    {
        if (magnetsInCircle.Contains(m))
            magnetsInCircle.Remove(m);
    }

    public List<Magnet> GetMagnetsInCircle() => magnetsInCircle;

    public bool IsGameOver()
    {
        return player1Placed == initialMagnetCount || player2Placed == initialMagnetCount;
    }

    // -------------------------------------------------------------------
    // RESTART GAME
    // -------------------------------------------------------------------

    public void OnRestartButtonClicked()
    {
        foreach (Magnet m in new List<Magnet>(magnetsInCircle))
            if (m != null) Destroy(m.gameObject);

        magnetsInCircle.Clear();

        InitializeGame();
    }

    // -------------------------------------------------------------------
    // RETURN TO MAIN MENU
    // -------------------------------------------------------------------

    public void ReturnToMainMenu()
    {
        foreach (Magnet m in new List<Magnet>(magnetsInCircle))
            if (m != null) Destroy(m.gameObject);

        magnetsInCircle.Clear();

        ClearHolder(player1MagnetHolder);
        ClearHolder(player2MagnetHolder);
    }

    private void ClearHolder(Transform holder)
    {
        foreach (Transform slot in holder)
        {
            if (slot.childCount > 0)
                Destroy(slot.GetChild(0).gameObject);
        }
    }
}
