#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class AssetItemPrefabFile : AssetItemPrefab
    {

        public AssetItemPrefabFile(FileInfo fileInfo, float scale = 1) : base(fileInfo, scale)
        {
            ResourceType = ResourceType.Prefab;
        }

    }
}
#endif