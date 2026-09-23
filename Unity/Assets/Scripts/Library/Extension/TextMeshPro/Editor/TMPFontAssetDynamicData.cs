using TMPro;
using UnityEditor;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Extension.Editor;

namespace TMPro.Extension.Editor
{
    internal static class TMPFontAssetDynamicData
    {
        public static bool IsClearOnBuildEnabled(TMP_FontAsset fontAsset)
        {
            if (fontAsset.atlasPopulationMode != AtlasPopulationMode.Dynamic &&
                fontAsset.atlasPopulationMode != AtlasPopulationMode.DynamicOS)
            {
                return false;
            }

            using (SerializedObject serializedFontAsset = new SerializedObject(fontAsset))
            {
                return serializedFontAsset.FindProperty("m_ClearDynamicDataOnBuild").boolValue;
            }
        }

        public static void Clear(TMP_FontAsset fontAsset)
        {
            if (!IsClearOnBuildEnabled(fontAsset))
            {
                return;
            }

            fontAsset.ClearFontAssetData(true);
            EditorUtility.SetDirty(fontAsset);
            EditorUtility.SetDirty(fontAsset.atlasTexture);

            // Save only this opted-in font, bypassing OnWillSaveAssets without changing its edit lock.
            AssetDatabase.SaveAssetIfDirty(fontAsset);
        }

        [UGFBuildOnPreprocessPlatform(int.MaxValue)]
        private static void ClearBeforeAssetBundleBuild(Platform platform)
        {
            string[] fontAssetGuids = AssetDatabase.FindAssets("t:TMP_FontAsset");
            foreach (string guid in fontAssetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                if (fontAsset != null)
                {
                    Clear(fontAsset);
                }
            }
        }
    }
}
