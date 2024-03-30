using UnityEditor;

namespace UnityGameFramework.Extension
{
    // [InitializeOnLoad]
    public static class SpriteCollectionInitializeOnLoad
    {
        static SpriteCollectionInitializeOnLoad()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        private static void PlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                SpriteCollectionUtility.RefreshSpriteCollection();
            }
        }
    }
}