using UnityEngine;

public class SkinSelectionPanel : MonoBehaviour
{
    public Transform gridParent;
    public GameObject skinPrefab;
    private int appliedSkinIndex;

    private void OnEnable()
    {
        appliedSkinIndex = GameCore.Instance.gameData.selectedMagnetSkin;
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

            btn.Init(
                i,
                lib.magnetSkins[i],
                this,
                data.unlockedSkins[i]
            );
        }
    }

    // ONE TAP ENTRY POINT
    public void OnSkinItemClicked(SkinItemButton btn)
    {
        var data = GameCore.Instance.gameData;
        var lib = GameCore.Instance.skinLibrary;

        int index = btn.skinIndex;
        int price = lib.GetPrice(index);

        Debug.Log($"Coins before action: {data.GetCoins()}");

        // BUY IF LOCKED
        if (!data.unlockedSkins[index])
        {
            // Check coins
            if (!data.HasEnoughCoins(price))
            {
                Debug.Log($"Not enough coins. Need {price}");
                return;
            }

            // Spend coins
            bool spent = data.SpendCoins(price);
            if (!spent)
            {
                Debug.Log("SpendCoins failed unexpectedly");
                return;
            }

            // Unlock skin
            data.unlockedSkins[index] = true;
            btn.Unlock();

            Debug.Log($"Coins after purchase: {data.GetCoins()}");
        }

        //  APPLY SKIN
        appliedSkinIndex = index;
        data.selectedMagnetSkin = index;
        data.Save();

        UIManager.Instance.RefreshCoinsUI();

        Debug.Log($"Applied skin {index}");
    }

}
