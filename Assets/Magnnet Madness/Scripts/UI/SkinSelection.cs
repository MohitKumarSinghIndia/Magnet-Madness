using UnityEngine;
using UnityEngine.UI;

public class SkinSelection : MonoBehaviour
{
    public Transform gridParent;
    public GameObject skinPrefab;
    public Button selectButton;
    public Button backButton;

    int selectedSkinIndex = 0;

    void OnEnable()
    {
        LoadGrid();
        selectButton.onClick.AddListener(ApplySkin);
        backButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    void OnDisable()
    {
        selectButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
    }

    void LoadGrid()
    {
        foreach (Transform t in gridParent)
            Destroy(t.gameObject);

        var lib = GameCore.Instance.skinLibrary;

        for (int i = 0; i < lib.magnetSkins.Length; i++)
        {
            GameObject item = Instantiate(skinPrefab, gridParent);
            SkinItemButton btn = item.GetComponent<SkinItemButton>();

            btn.Init(i, lib.magnetSkins[i], this);

            if (i == GameCore.Instance.gameData.selectedMagnetSkin)
            {
                selectedSkinIndex = i;
                btn.selectionHighlight.SetActive(true);
            }
        }
    }

    public void OnSkinSelected(int index)
    {
        selectedSkinIndex = index;

        foreach (Transform t in gridParent)
        {
            SkinItemButton btn = t.GetComponent<SkinItemButton>();
            btn.selectionHighlight.SetActive(btn.skinIndex == index);
        }
    }

    void ApplySkin()
    {
        GameCore.Instance.gameData.selectedMagnetSkin = selectedSkinIndex;
        GameCore.Instance.gameData.Save();

        gameObject.SetActive(false);
        MainMenuManager.Instance.OnBackToMainMenu();
    }
}
