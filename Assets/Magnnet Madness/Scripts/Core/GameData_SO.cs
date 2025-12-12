using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "MagnetMadness/GameData")]
public class GameData_SO : ScriptableObject
{
    public string player1Name = "Player 1";
    public string player2Name = "Player 2";

    public bool musicEnabled = true;
    public bool sfxEnabled = true;
    public bool vibrationEnabled = true;

    public int selectedMagnetSkin = 0;

    public void LoadFromPrefs()
    {
        player1Name = PlayerPrefs.GetString("p1name", player1Name);
        player2Name = PlayerPrefs.GetString("p2name", player2Name);

        musicEnabled = PlayerPrefs.GetInt("music", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("sfx", 1) == 1;
        vibrationEnabled = PlayerPrefs.GetInt("vibration", 1) == 1;

        selectedMagnetSkin = PlayerPrefs.GetInt("skin", 0);
    }

    public void Save()
    {
        PlayerPrefs.SetString("p1name", player1Name);
        PlayerPrefs.SetString("p2name", player2Name);

        PlayerPrefs.SetInt("music", musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("sfx", sfxEnabled ? 1 : 0);
        PlayerPrefs.SetInt("vibration", vibrationEnabled ? 1 : 0);

        PlayerPrefs.SetInt("skin", selectedMagnetSkin);

        PlayerPrefs.Save();
    }
}
