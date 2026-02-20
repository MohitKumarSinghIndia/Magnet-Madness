using UnityEngine;
using DG.Tweening;

public class RotateUI : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float duration = 150f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        rectTransform
            .DORotate(new Vector3(0, 0, 360f), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
}