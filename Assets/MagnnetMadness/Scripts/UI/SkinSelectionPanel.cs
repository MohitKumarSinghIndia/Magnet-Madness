using UnityEngine;

public class SkinSelectionPanel : MonoBehaviour
{
    public Transform gridParent;
    public GameObject skinPrefab;

    private int selectedSkinIndex;
    private SkinItemButton selectedButton;

    private void OnEnable()
    {
        selectedSkinIndex = GameCore.Instance.gameData.selectedMagnetSkin;
        LoadGrid();
    }

    private void LoadGrid()
    {
        foreach (Transform t in gridParent)
            Destroy(t.gameObject);

        var lib = GameCore.Instance.skinLibrary;
        var data = GameCore.Instance.gameData;

        for (int i = 0; i < lib.magnetSkins.Length; i++)
        {
            GameObject item = Instantiate(skinPrefab, gridParent);
            SkinItemButton btn = item.GetComponent<SkinItemButton>();

            bool unlocked = data.unlockedSkins[i];
            bool selected = i == selectedSkinIndex;

            btn.Init(
                i,
                lib.magnetSkins[i],
                this,
                unlocked,
                selected
            );

            if (selected)
                selectedButton = btn;
        }
    }

    // TAP = SELECT (and buy if needed)
    public void OnSkinItemClicked(SkinItemButton btn)
    {
        var data = GameCore.Instance.gameData;
        var lib = GameCore.Instance.skinLibrary;

        int index = btn.skinIndex;
        int price = lib.GetPrice(index);

        // BUY IF LOCKED
        if (!data.unlockedSkins[index])
        {
            if (!data.HasEnoughCoins(price))
            {
                Debug.Log("Not enough coins");
                return;
            }

            data.SpendCoins(price);
            data.unlockedSkins[index] = true;
            btn.Unlock();
        }

        // UPDATE SELECTION UI
        if (selectedButton != null)
            selectedButton.SetSelected(false);

        selectedButton = btn;
        selectedButton.SetSelected(true);

        selectedSkinIndex = index;

        GameCore.Instance.gameData.selectedMagnetSkin = selectedSkinIndex;
        GameCore.Instance.gameData.Save();

        UIManager.Instance.RefreshCoinsUI();

        Debug.Log($"Applied skin {selectedSkinIndex}");
    }
}
