using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

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

    [HideInInspector] public int player1Magnets;
    [HideInInspector] public int player2Magnets;

    public PlayerTurn currentTurn = PlayerTurn.Player1;

    private List<Magnet> magnetsInCircle = new List<Magnet>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializeGame();
    }

    // ------------------ GAME INIT ------------------
    public void InitializeGame()
    {
        player1Magnets = initialMagnetCount;
        player2Magnets = initialMagnetCount;

        magnetSpawner.SpawnPlayerMagnets();

        UIManager.Instance.UpdateUI();
    }

    // ------------------ TURN LOGIC ------------------
    public void SwitchTurn()
    {
        currentTurn = currentTurn == PlayerTurn.Player1 ? PlayerTurn.Player2 : PlayerTurn.Player1;
        UIManager.Instance.UpdateUI();
    }

    public PlayerTurn GetCurrentTurn() => currentTurn;

    // ------------------ PLAYER INVENTORY ------------------
    public void AddMagnetToPlayer(PlayerTurn p)
    {
        if (p == PlayerTurn.Player1) player1Magnets++;
        else player2Magnets++;

        UIManager.Instance.UpdateUI();
    }

    public void RemoveMagnetFromPlayer(PlayerTurn p)
    {
        if (p == PlayerTurn.Player1) player1Magnets--;
        else player2Magnets--;

        CheckWinCondition();
        UIManager.Instance.UpdateUI();
    }

    public int GetPlayerMagnetCount(PlayerTurn p)
    {
        return (p == PlayerTurn.Player1) ? player1Magnets : player2Magnets;
    }

    // ------------------ WIN CHECK ------------------
    void CheckWinCondition()
    {
        // If player placed all their magnets in the circle they win
        if (player1Magnets <= 0)
            UIManager.Instance.ShowWin(PlayerTurn.Player1);

        if (player2Magnets <= 0)
            UIManager.Instance.ShowWin(PlayerTurn.Player2);
    }

    // ------------------ CIRCLE LOGIC ------------------
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
        return player1Magnets <= 0 || player2Magnets <= 0;
    }

    public void OnRestartButtonClicked()
    {
        // 1. Clear all magnets currently in circle
        foreach (Magnet m in new List<Magnet>(magnetsInCircle))
        {
            if (m != null)
                Destroy(m.gameObject);
        }
        magnetsInCircle.Clear();

        // 2. Clear holders
        ClearHolder(player1MagnetHolder);
        ClearHolder(player2MagnetHolder);

        // 3. Reset counts
        player1Magnets = initialMagnetCount;
        player2Magnets = initialMagnetCount;

        // 4. Reset turn
        currentTurn = PlayerTurn.Player1;

        // 5. Spawn fresh magnets
        magnetSpawner.SpawnPlayerMagnets();

        // 6. Update UI
        UIManager.Instance.UpdateUI();
    }
    void ClearHolder(Transform holder)
    {
        foreach (Transform slot in holder)
        {
            if (slot.childCount > 0)
                Destroy(slot.GetChild(0).gameObject);
        }
    }

}
