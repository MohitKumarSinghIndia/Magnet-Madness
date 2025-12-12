using UnityEngine;

public static class HapticManager
{
    public static void VibrateLight()
    {
        if (!GameCore.Instance.gameData.vibrationEnabled) return;

#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public static void VibrateMedium()
    {
        if (!GameCore.Instance.gameData.vibrationEnabled) return;

#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }
}
