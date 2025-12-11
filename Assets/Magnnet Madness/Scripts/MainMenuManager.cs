using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject playerNamePanel;
    public GameObject settingsPanel;
    public GameObject aboutPanel;
    public GameObject loadingPanel;

    [Header("Player Name Inputs")]
    public TMP_InputField player1Input;
    public TMP_InputField player2Input;

    [Header("Loading UI")]
    public Slider loadingBar;
    public TMP_Text loadingText;

    void Start()
    {
        mainMenuPanel.SetActive(true);
        playerNamePanel.SetActive(false);
        settingsPanel.SetActive(false);
        aboutPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    // ========= MENU NAVIGATION =========

    public void OnPlayClicked()
    {
        mainMenuPanel.SetActive(false);
        playerNamePanel.SetActive(true);
    }

    public void OnSettingsClicked()
    {
        settingsPanel.SetActive(true);
    }

    public void OnAboutClicked()
    {
        aboutPanel.SetActive(true);
    }

    public void OnBackToMenu()
    {
        mainMenuPanel.SetActive(true);
        playerNamePanel.SetActive(false);
        settingsPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    // ========= START GAME =========

    public void OnStartGameClicked()
    {
        if (player1Input.text == "" || player2Input.text == "")
        {
            Debug.LogWarning("Names cannot be empty!");
            return;
        }

        GameData.Player1Name = player1Input.text;
        GameData.Player2Name = player2Input.text;

        StartCoroutine(LoadGameAsync());
    }

    IEnumerator LoadGameAsync()
    {
        loadingPanel.SetActive(true);
        playerNamePanel.SetActive(false);

        AsyncOperation operation = SceneManager.LoadSceneAsync("GameplayScene");

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progress;
            loadingText.text = (progress * 100f).ToString("0") + "%";
            yield return null;
        }
    }
}
