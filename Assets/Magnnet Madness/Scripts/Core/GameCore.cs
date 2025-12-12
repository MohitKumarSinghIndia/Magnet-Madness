using UnityEngine;

public class GameCore : MonoBehaviour
{
    public static GameCore Instance;

    public GameData_SO gameData;
    public MagnetSkinLibrary_SO skinLibrary;

    private void Awake()
    {
        Instance = this;
        //if (Instance != null)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //Instance = this;
        //DontDestroyOnLoad(gameObject);

        // Load saved preferences
        gameData.LoadFromPrefs();
    }
}