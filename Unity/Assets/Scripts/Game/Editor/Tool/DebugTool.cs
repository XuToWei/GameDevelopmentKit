using UnityEditor;

namespace Game.Editor
{
    public static class DebugTool
    {
        [MenuItem("Game/Debug Tool/Enable Debug-Internal")]
        public static void EnableDebugInternal()
        {
            EditorPrefs.SetBool("DeveloperMode", true);
        }

        [MenuItem("Game/Debug Tool/Disable Debug-Internal")]
        public static void DisableDebugInternal()
        {
            EditorPrefs.SetBool("DeveloperMode", false);
        }
    }
}