using UnityEditor;

namespace UnityGameFramework.Extension.Editor
{
    public static class SpriteCollectionUtility
    {
        [MenuItem("Game Framework/Refresh SpriteCollection", false, 1)]
        public static void RefreshSpriteCollection()
        {
            string[] guids = AssetDatabase.FindAssets("t:SpriteCollection");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SpriteCollection collection = AssetDatabase.LoadAssetAtPath<SpriteCollection>(path);
                collection.Pack();
            }
        }
    }
}