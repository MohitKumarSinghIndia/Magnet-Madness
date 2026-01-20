using UnityEngine;
using UnityEngine.UI;

public class SkinItemButton : MonoBehaviour
{
    public int skinIndex;
    public Image iconImage;
    public GameObject lockSprite;
    public GameObject selectedSprite;

    private SkinSelectionPanel controller;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Init(int index, Sprite icon, SkinSelectionPanel ui, bool unlocked, bool selected)
    {
        skinIndex = index;
        controller = ui;

        iconImage.sprite = icon;
        lockSprite.SetActive(!unlocked);
        selectedSprite.SetActive(selected);

        button.interactable = true;
    }

    public void Unlock()
    {
        lockSprite.SetActive(false);
    }

    public void SetSelected(bool value)
    {
        selectedSprite.SetActive(value);
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
        controller.OnSkinItemClicked(this);
    }
}
