using UnityEngine;
namespace settings
{
    public static class Settings
    {
        public static bool ShouldUseCameraShake { get; private set; } = true;
        public static void ToggleCameraShake(bool value)
        {
            ShouldUseCameraShake = value;
        }
    }
}

