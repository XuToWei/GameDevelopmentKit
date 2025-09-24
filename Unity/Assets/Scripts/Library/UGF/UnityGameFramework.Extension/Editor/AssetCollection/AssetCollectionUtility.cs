using UnityEditor;

namespace UnityGameFramework.Extension.Editor
{
    public static class AssetCollectionUtility
    {
        [MenuItem("Game Framework/Refresh AssetCollection", false, 2)]
        public static void RefreshAssetCollection()
        {
            string[] guids = AssetDatabase.FindAssets("t:AssetCollection");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssetCollection collection = AssetDatabase.LoadAssetAtPath<AssetCollection>(path);
                collection.Pack();
            }
        }
    }
}