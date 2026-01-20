using UnityEngine;

[CreateAssetMenu(fileName = "MagnetSkins", menuName = "MagnetMadness/MagnetSkinLibrary")]
public class MagnetSkinLibrary_SO : ScriptableObject
{
    [Header("Skins")]
    public Sprite[] magnetSkins;

    [Header("Skin Prices (auto-generated)")]
    public int[] skinPrices;

    // Get skin sprite
    public Sprite GetSkin(int index)
    {
        if (magnetSkins == null || magnetSkins.Length == 0)
            return null;

        index = Mathf.Clamp(index, 0, magnetSkins.Length - 1);
        return magnetSkins[index];
    }
    // Generate skin prices
    public void AutoSetupPrices()
    {
        if (magnetSkins == null)
            return;

        if (skinPrices == null || skinPrices.Length != magnetSkins.Length)
        {
            skinPrices = new int[magnetSkins.Length];

            for (int i = 0; i < skinPrices.Length; i++)
            {
                skinPrices[i] = (i == 0) ? 0 : i * 100;
            }
        }
    }

    // Get skin price
    public int GetPrice(int index)
    {
        if (skinPrices == null || skinPrices.Length == 0)
            return 0;

        index = Mathf.Clamp(index, 0, skinPrices.Length - 1);
        return skinPrices[index];
    }
}
