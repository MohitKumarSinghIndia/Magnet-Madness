using UnityEngine;

public static class HapticManager
{
    public static void VibrateLight()
    {
        if (!GameData.VibrationEnabled) return;

#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    public static void VibrateMedium()
    {
        if (!GameData.VibrationEnabled) return;

#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }
}
