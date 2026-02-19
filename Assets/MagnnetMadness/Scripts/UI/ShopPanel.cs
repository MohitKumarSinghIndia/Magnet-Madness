using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ShopPanel : MonoBehaviour
{
    #region ===================== INSPECTOR REFERENCES =====================

    [Header("Grid")]
    public Transform gridParent;
    public GameObject skinPrefab;

    [Header("Confirm Buy Panel")]
    public GameObject confirmPanel;
    public TextMeshProUGUI confirmTitleText;
    public TextMeshProUGUI confirmPriceText;
    public Button yesButton;
    public Button noButton;

    [Header("Not Enough Coins Popup")]
    public GameObject notEnoughCoinsPanel;
    public TextMeshProUGUI notEnoughCoinsText;
    public float popupDisplayTime = 1.5f;

    #endregion


    #region ===================== PRIVATE VARIABLES =====================

    private int selectedSkinIndex;
    private SkinItemButton selectedButton;
    private int pendingSkinIndex;

    private Vector3 popupOriginalScale;
    private Sequence popupSequence;

    #endregion


    #region ===================== UNITY METHODS =====================

    private void Awake()
    {
        yesButton.onClick.AddListener(OnConfirmPurchase);
        noButton.onClick.AddListener(HidePanels);

        confirmPanel.SetActive(false);

        if (notEnoughCoinsPanel != null)
        {
            popupOriginalScale = notEnoughCoinsPanel.transform.localScale;
            notEnoughCoinsPanel.SetActive(false);
        }

    }

    private void OnEnable()
    {
        selectedSkinIndex = GameCore.Instance.gameData.selectedMagnetSkin;
        HidePanels();
        LoadGrid();
    }

    private void OnDisable()
    {
        popupSequence?.Kill();
    }

    #endregion


    #region ===================== GRID LOGIC =====================

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

            bool unlocked = data.unlockedSkins[i];
            bool selected = i == selectedSkinIndex;
            int price = lib.GetPrice(i);

            btn.Init(
                i,
                lib.magnetSkins[i],
                this,
                unlocked,
                selected,
                price
            );

            if (selected)
                selectedButton = btn;
        }
    }

    #endregion


    #region ===================== SKIN CLICK HANDLING =====================

    public void OnSkinItemClicked(SkinItemButton btn)
    {
        var data = GameCore.Instance.gameData;
        var lib = GameCore.Instance.skinLibrary;

        int index = btn.skinIndex;
        int price = lib.GetPrice(index);

        if (!data.unlockedSkins[index])
        {
            if (!data.HasEnoughCoins(price))
            {
                ShowNotEnoughCoinsPopup();
                return;
            }

            ShowConfirmPanel(index, price);
            return;
        }

        ApplySkin(index, btn);
    }

    #endregion


    #region ===================== CONFIRM BUY =====================

    private void ShowConfirmPanel(int index, int price)
    {
        pendingSkinIndex = index;

        confirmTitleText.text = "Do you want to buy this skin?";
        confirmPriceText.text = "Cost: " + price + " Coins";

        confirmPanel.SetActive(true);
    }

    private void HidePanels()
    {
        AudioManager.Instance?.PlayButtonClick();

        confirmPanel.SetActive(false);
        notEnoughCoinsPanel.SetActive(false);
    }

    private void OnConfirmPurchase()
    {
        AudioManager.Instance?.PlayButtonClick();

        var data = GameCore.Instance.gameData;
        var lib = GameCore.Instance.skinLibrary;

        int price = lib.GetPrice(pendingSkinIndex);

        if (!data.HasEnoughCoins(price))
        {
            ShowNotEnoughCoinsPopup();
            return;
        }

        data.SpendCoins(price);
        data.unlockedSkins[pendingSkinIndex] = true;

        HidePanels();

        LoadGrid();
        UIManager.Instance.RefreshCoinsUI();
    }

    #endregion


    #region ===================== SELECTION =====================

    private void ApplySkin(int index, SkinItemButton btn)
    {
        if (selectedButton != null)
            selectedButton.SetSelected(false);

        selectedButton = btn;
        selectedButton.SetSelected(true);

        selectedSkinIndex = index;

        GameCore.Instance.gameData.selectedMagnetSkin = selectedSkinIndex;
        GameCore.Instance.gameData.Save();
        HidePanels();
        UIManager.Instance.RefreshCoinsUI();
    }

    #endregion


    #region ===================== NOT ENOUGH COINS =====================

    private void ShowNotEnoughCoinsPopup()
    {
        notEnoughCoinsText.text = "You don't have enough coins!";

        Transform popup = notEnoughCoinsPanel.transform;

        popupSequence?.Kill();
        popup.DOKill();

        notEnoughCoinsPanel.SetActive(true);
        popup.localScale = Vector3.zero;

        popupSequence = DOTween.Sequence();

        popupSequence
            .Append(popup.DOScale(popupOriginalScale * 1.1f, 0.25f)
            .SetEase(Ease.OutBack))

            .Append(popup.DOScale(popupOriginalScale, 0.15f))

            .AppendInterval(popupDisplayTime)

            .Append(popup.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack))

            .OnComplete(() =>
            {
                notEnoughCoinsPanel.SetActive(false);
                popup.localScale = popupOriginalScale;
            });
    }

    #endregion
}
