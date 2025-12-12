using UnityEngine;

[CreateAssetMenu(fileName = "MagnetSkins", menuName = "MagnetMadness/MagnetSkinLibrary")]
public class MagnetSkinLibrary_SO : ScriptableObject
{
    public Sprite[] magnetSkins;

    public Sprite GetSkin(int index)
    {
        return magnetSkins[Mathf.Clamp(index, 0, magnetSkins.Length - 1)];
    }
}
