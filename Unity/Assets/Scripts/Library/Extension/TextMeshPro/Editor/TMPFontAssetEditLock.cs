using System.Collections.Generic;
using TMPro;

namespace TMPro.Extension.Editor
{
    internal static class TMPFontAssetEditLock
    {
        private static readonly Dictionary<int, int> s_UnlockedFontAssetIdsByInspector = new Dictionary<int, int>();

        public static void Unlock(int inspectorInstanceId, TMP_FontAsset fontAsset)
        {
            s_UnlockedFontAssetIdsByInspector[inspectorInstanceId] = fontAsset.GetInstanceID();
        }

        public static void Lock(int inspectorInstanceId)
        {
            s_UnlockedFontAssetIdsByInspector.Remove(inspectorInstanceId);
        }

        public static bool IsUnlocked(TMP_FontAsset fontAsset)
        {
            int fontAssetInstanceId = fontAsset.GetInstanceID();
            foreach (int unlockedFontAssetId in s_UnlockedFontAssetIdsByInspector.Values)
            {
                if (unlockedFontAssetId == fontAssetInstanceId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
