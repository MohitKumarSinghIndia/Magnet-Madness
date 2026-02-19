using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinItemButton : MonoBehaviour
{
    [Header("Basic")]
    public int skinIndex;
    public Image iconImage;
    public GameObject lockSprite;
    public GameObject selectedWave;
    public GameObject selectedUI;

    [Header("Price UI")]
    public GameObject priceRoot;
    public TextMeshProUGUI priceText;

    private ShopPanel controller;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Init(
        int index,
        Sprite icon,
        ShopPanel ui,
        bool unlocked,
        bool selected,
        int price)
    {
        skinIndex = index;
        controller = ui;

        iconImage.sprite = icon;

        lockSprite.SetActive(!unlocked);
        priceRoot.SetActive(!unlocked);
        priceText.text = price.ToString();

        selectedWave.SetActive(selected);
        selectedUI.SetActive(selected);

        button.interactable = true;
    }

    public void Unlock()
    {
        lockSprite.SetActive(false);
        priceRoot.SetActive(false);
    }

    public void SetSelected(bool value)
    {
        selectedWave.SetActive(value);
        selectedUI.SetActive(value);
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        AudioManager.Instance?.PlayButtonClick();

        controller.OnSkinItemClicked(this);
    }
}
