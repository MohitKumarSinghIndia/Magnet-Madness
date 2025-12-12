using UnityEngine;
using UnityEngine.UI;

public class SkinItemButton : MonoBehaviour
{
    public int skinIndex;
    public Image iconImage;
    public GameObject selectionHighlight;

    private SkinSelection controller;
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

    public void Init(int index, Sprite icon, SkinSelection ui)
    {
        skinIndex = index;
        controller = ui;

        iconImage.sprite = icon;
        selectionHighlight.SetActive(false);
    }

    public void OnClick()
    {
        controller.OnSkinSelected(skinIndex);
    }
}