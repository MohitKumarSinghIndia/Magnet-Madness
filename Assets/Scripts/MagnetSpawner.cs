using UnityEngine;

public class MagnetSpawner : MonoBehaviour
{
    [Header("Magnet Prefab")]
    public GameObject magnetPrefab;

    [Header("Spawn Settings")]
    public int magnetsPerPlayer = 5;
    public float spacing = 1f;

    void Start()
    {
        SpawnPlayerMagnets();
    }

    void SpawnPlayerMagnets()
    {
        if (magnetPrefab == null)
        {
            Debug.LogError("Magnet prefab is not assigned!");
            return;
        }

        if (GameManager.Instance.player1MagnetHolder == null ||
            GameManager.Instance.player2MagnetHolder == null)
        {
            Debug.LogError("Player magnet holders are not assigned in GameManager!");
            return;
        }

        // Spawn Player 1 magnets
        for (int i = 0; i < magnetsPerPlayer; i++)
        {
            Vector3 spawnPosition = GameManager.Instance.player1MagnetHolder.position +
                                   new Vector3(i * spacing, 0, 0);
            SpawnMagnet(spawnPosition, PlayerTurn.Player1, GameManager.Instance.player1MagnetHolder);
        }

        // Spawn Player 2 magnets
        for (int i = 0; i < magnetsPerPlayer; i++)
        {
            Vector3 spawnPosition = GameManager.Instance.player2MagnetHolder.position +
                                   new Vector3(i * spacing, 0, 0);
            SpawnMagnet(spawnPosition, PlayerTurn.Player2, GameManager.Instance.player2MagnetHolder);
        }
    }

    void SpawnMagnet(Vector3 position, PlayerTurn owner, Transform parent)
    {
        GameObject magnetObj = Instantiate(magnetPrefab, position, Quaternion.identity, parent);
        Magnet magnet = magnetObj.GetComponent<Magnet>();

        if (magnet != null)
        {
            magnet.SetOwner(owner);
        }

        magnetObj.name = owner.ToString() + "_Magnet";
    }

    public void RespawnMagnet(PlayerTurn owner)
    {
        Transform holder = (owner == PlayerTurn.Player1) ?
            GameManager.Instance.player1MagnetHolder :
            GameManager.Instance.player2MagnetHolder;

        if (holder != null)
        {
            // Count existing magnets
            int existingMagnets = holder.childCount;
            Vector3 spawnPosition = holder.position + new Vector3(existingMagnets * spacing, 0, 0);

            SpawnMagnet(spawnPosition, owner, holder);
        }
    }
}