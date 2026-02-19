using UnityEngine;

[CreateAssetMenu(fileName = "MagnetSkins", menuName = "MagnetMadness/MagnetSkinLibrary")]
public class MagnetSkinLibrary_SO : ScriptableObject
{
    [Header("Skins")]
    public Sprite[] magnetSkins;

    [Header("Skin Prices (auto-generated)")]
    public int[] skinPrices;

    public Sprite defaultSkin;

    public Sprite GetSkin(int index)
    {
        if (magnetSkins == null || magnetSkins.Length == 0)
            return null;

        index = Mathf.Clamp(index, 0, magnetSkins.Length - 1);
        return magnetSkins[index];
    }

    public Sprite GetDefaultSkin()
    {
        return defaultSkin;
    }

    public void AutoSetupPrices()
    {
        if (magnetSkins == null || magnetSkins.Length == 0)
        {
            Debug.LogWarning("Magnet skins not assigned!");
            return;
        }

        int[] basePrices = { 0, 150, 350 ,550,700};

        if (skinPrices == null || skinPrices.Length != magnetSkins.Length)
        {
            skinPrices = new int[magnetSkins.Length];
        }

        for (int i = 0; i < magnetSkins.Length; i++)
        {
            if (i < basePrices.Length)
            {
                skinPrices[i] = basePrices[i];
            }
            else
            {
                int lastPrice = skinPrices[i - 1];
                int increment = Mathf.RoundToInt(lastPrice * 0.2f);

                int newPrice = lastPrice + increment;

                newPrice = Mathf.RoundToInt(newPrice / 100f) * 100;

                skinPrices[i] = newPrice;
            }
        }
    }

    public int GetPrice(int index)
    {
        if (skinPrices == null || skinPrices.Length == 0)
            return 0;

        index = Mathf.Clamp(index, 0, skinPrices.Length - 1);
        return skinPrices[index];
    }
}
