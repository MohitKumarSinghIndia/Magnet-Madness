using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject playerNamePanel;
    public GameObject settingsPanel;
    public GameObject skinSelectionPanel;
    public GameObject aboutPanel;
    public GameObject loadingPanel;

    [Header("Player Input Fields")]
    public TMP_InputField player1Input;
    public TMP_InputField player2Input;

    [Header("Loading UI")]
    public Slider loadingBar;
    public TMP_Text loadingText;

    private bool isLoading = false;

    private void Awake()
    {
        Instance = this;

        ShowPanel(mainMenuPanel);
    }

    private void ShowPanel(GameObject target)
    {
        mainMenuPanel.SetActive(false);
        playerNamePanel.SetActive(false);
        settingsPanel.SetActive(false);
        skinSelectionPanel.SetActive(false);
        aboutPanel.SetActive(false);
        loadingPanel.SetActive(false);

        if (target != null)
            target.SetActive(true);
    }

    public void OnPlayClicked()
    {
        ShowPanel(playerNamePanel);
    }

    public void OnSettingsClicked()
    {
        ShowPanel(settingsPanel);
    }

    public void OnSkinSelectClicked()
    {
        ShowPanel(skinSelectionPanel);
    }

    public void OnAboutClicked()
    {
        ShowPanel(aboutPanel);
    }

    public void OnBackToMainMenu()
    {
        ShowPanel(mainMenuPanel);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    public void OnStartGameClicked()
    {
        if (string.IsNullOrWhiteSpace(player1Input.text) ||
            string.IsNullOrWhiteSpace(player2Input.text))
        {
            Debug.LogWarning("Player names cannot be empty!");
            return;
        }

        GameCore.Instance.gameData.player1Name = player1Input.text;
        GameCore.Instance.gameData.player2Name = player2Input.text;
        GameCore.Instance.gameData.Save();

        StartLoadingGameplay();
    }

    private void StartLoadingGameplay()
    {
        ShowPanel(loadingPanel);

        loadingText.text = "0%";
        loadingBar.value = 0f;
        isLoading = true;

        // Subscribe to progress updates
        SceneController.Instance.OnProgress += UpdateLoadingUI;

        // Start loading scene
        SceneController.Instance.LoadScene("GameplayScene");
    }
    private void UpdateLoadingUI(float progress)
    {
        if (!isLoading) return;

        float pct = progress * 100f;

        loadingBar.value = progress; // 0–1
        loadingText.text = pct.ToString("0") + "%";

        if (progress >= 1f)
        {
            isLoading = false;
            SceneController.Instance.OnProgress -= UpdateLoadingUI;
        }
    }

}
