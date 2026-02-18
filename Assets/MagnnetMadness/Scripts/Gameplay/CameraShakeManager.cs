using UnityEngine;
using DG.Tweening;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

    [Header("Shake Settings")]
    public RectTransform shakeRect;   // Drag Gameplay Panel here
    public float defaultDuration = 0.25f;
    public float defaultStrength = 30f; // UI needs higher value
    public int vibrato = 20;
    public float randomness = 90f;

    private Vector2 originalPos;
    private Tween shakeTween;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (shakeRect != null)
            originalPos = shakeRect.anchoredPosition;
    }

    public void ShakeCamera(
        float duration = -1f,
        float strength = -1f,
        bool useHaptic = true)
    {
        if (shakeRect == null) return;

        if (duration <= 0) duration = defaultDuration;
        if (strength <= 0) strength = defaultStrength;

        shakeTween?.Kill();

        shakeTween = shakeRect.DOShakeAnchorPos(
            duration,
            strength,
            vibrato,
            randomness,
            false,
            true
        ).OnComplete(() =>
        {
            shakeRect.anchoredPosition = originalPos;
        });

        if (useHaptic)
            HapticManager.VibrateMedium();
    }
}
