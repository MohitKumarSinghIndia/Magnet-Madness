using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "MagnetMadness/GameData")]
public class GameData_SO : ScriptableObject
{
    public bool musicEnabled = true;
    public bool sfxEnabled = true;
    public bool vibrationEnabled = true;

    [Header("Currency")]
    public int coins = 0;

    [Header("Skins")]
    public int selectedMagnetSkin = 0;
    public bool[] unlockedSkins;

    // -------------------------------------------------------------------
    // LOAD / SAVE
    // -------------------------------------------------------------------

    public void LoadFromPrefs(int skinCount)
    {
        musicEnabled = PlayerPrefs.GetInt("music", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("sfx", 1) == 1;
        vibrationEnabled = PlayerPrefs.GetInt("vibration", 1) == 1;

        coins = PlayerPrefs.GetInt("coins", 0);
        selectedMagnetSkin = PlayerPrefs.GetInt("skin", 0);

        // INIT UNLOCKED SKINS
        if (unlockedSkins == null || unlockedSkins.Length != skinCount)
        {
            unlockedSkins = new bool[skinCount];
            unlockedSkins[0] = true; // first skin free
        }

        for (int i = 0; i < unlockedSkins.Length; i++)
            unlockedSkins[i] = PlayerPrefs.GetInt("skin_unlocked_" + i, i == 0 ? 1 : 0) == 1;
    }

    public void Save()
    {
        PlayerPrefs.SetInt("music", musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("sfx", sfxEnabled ? 1 : 0);
        PlayerPrefs.SetInt("vibration", vibrationEnabled ? 1 : 0);

        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.SetInt("skin", selectedMagnetSkin);

        for (int i = 0; i < unlockedSkins.Length; i++)
            PlayerPrefs.SetInt("skin_unlocked_" + i, unlockedSkins[i] ? 1 : 0);

        PlayerPrefs.Save();
    }

    // -------------------------------------------------------------------
    // COIN FUNCTIONS
    // -------------------------------------------------------------------

    /// Get current coins
    
    public int GetCoins()
    {
        return coins;
    }

    // Add coins (reward, win, ad, etc.)
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        coins += amount;
        Save();
    }

    // Spend coins
    public bool SpendCoins(int amount)
    {
        if (amount <= 0) return true;

        if (coins < amount)
            return false;

        coins -= amount;
        Save();
        return true;
    }

    // Check if player has enough coins
    public bool HasEnoughCoins(int amount)
    {
        return coins >= amount;
    }
}
