using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMPro.Extension.Editor
{
    internal static class TMPFontAssetEditLock
    {
        private static readonly Dictionary<EntityId, EntityId> s_UnlockedFontAssetIdsByInspector = new Dictionary<EntityId, EntityId>();

        public static void Unlock(EntityId inspectorInstanceId, TMP_FontAsset fontAsset)
        {
            s_UnlockedFontAssetIdsByInspector[inspectorInstanceId] = fontAsset.GetEntityId();
        }

        public static void Lock(EntityId inspectorInstanceId)
        {
            s_UnlockedFontAssetIdsByInspector.Remove(inspectorInstanceId);
        }

        public static bool IsUnlocked(TMP_FontAsset fontAsset)
        {
            EntityId fontAssetInstanceId = fontAsset.GetEntityId();
            foreach (EntityId unlockedFontAssetId in s_UnlockedFontAssetIdsByInspector.Values)
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
