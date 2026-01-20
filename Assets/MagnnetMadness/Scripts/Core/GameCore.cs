using UnityEngine;

public class GameCore : MonoBehaviour
{
    public static GameCore Instance;

    public GameData_SO gameData;
    public MagnetSkinLibrary_SO skinLibrary;

    private void Awake()
    {
        Instance = this;

        // Make sure prices exist
        skinLibrary.AutoSetupPrices();

        // Load saved data
        gameData.LoadFromPrefs(skinLibrary.magnetSkins.Length);
    }
}
