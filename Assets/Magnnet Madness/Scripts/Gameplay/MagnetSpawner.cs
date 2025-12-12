using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MagnetSpawner : MonoBehaviour
{
    public GameObject magnetPrefab;

    public List<Transform> player1Slots;
    public List<Transform> player2Slots;

    public Dictionary<PlayerTurn, List<Transform>> playerSlots;

    void Awake()
    {
        playerSlots = new Dictionary<PlayerTurn, List<Transform>>
        {
            { PlayerTurn.Player1, player1Slots },
            { PlayerTurn.Player2, player2Slots }
        };
    }

    public void SpawnPlayerMagnets()
    {
        SpawnFor(PlayerTurn.Player1, GameManager.Instance.initialMagnetCount);
        SpawnFor(PlayerTurn.Player2, GameManager.Instance.initialMagnetCount);
    }

    void SpawnFor(PlayerTurn owner, int count)
    {
        var slots = playerSlots[owner];
        int skin = GameCore.Instance.gameData.selectedMagnetSkin;

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(magnetPrefab, slots[i]);
            obj.transform.localPosition = Vector3.zero;

            obj.GetComponent<Image>().sprite =
                GameCore.Instance.skinLibrary.GetSkin(skin);

            Magnet m = obj.GetComponent<Magnet>();
            m.SetOwner(owner);
            m.SetSlotIndex(i);
        }
    }

    int GetFreeSlot(PlayerTurn owner)
    {
        var slots = playerSlots[owner];
        bool[] used = new bool[slots.Count];

        foreach (Transform slot in slots)
        {
            if (slot.childCount > 0)
            {
                Magnet m = slot.GetChild(0).GetComponent<Magnet>();
                if (m != null) used[m.slotIndex] = true;
            }
        }

        for (int i = 0; i < used.Length; i++)
            if (!used[i]) return i;

        return slots.Count - 1;
    }

    public void RespawnMagnet(PlayerTurn owner)
    {
        int freeSlot = GetFreeSlot(owner);
        Transform slot = playerSlots[owner][freeSlot];

        GameObject obj = Instantiate(magnetPrefab, slot);
        obj.transform.localPosition = Vector3.zero;

        Magnet m = obj.GetComponent<Magnet>();
        m.SetOwner(owner);
        m.SetSlotIndex(freeSlot);
    }
}
