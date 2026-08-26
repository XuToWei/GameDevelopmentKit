using UnityEditor;

namespace TMPro.Extension.Editor
{
    internal static class TMPFontAssetPlayModeLock
    {
        public static bool IsActive => EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode;
    }
}
