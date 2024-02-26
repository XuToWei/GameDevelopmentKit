using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Extension
{
    public class AssetCollectionUtility : MonoBehaviour
    {
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
