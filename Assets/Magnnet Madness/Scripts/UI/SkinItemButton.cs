using UnityEngine;
using UnityEngine.UI;

public class SkinItemButton : MonoBehaviour
{
    public int skinIndex;
    public Image iconImage;
    public GameObject lockSprite;

    private SkinSelectionPanel controller;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    public void Init(int index, Sprite icon, SkinSelectionPanel ui, bool unlocked)
    {
        skinIndex = index;
        controller = ui;

        iconImage.sprite = icon;
        lockSprite.SetActive(!unlocked);

        button.interactable = true;
    }

    public void Unlock()
    {
        lockSprite.SetActive(false);
    }

    private void OnClick()
    {
        controller.OnSkinItemClicked(this);
    }
}
