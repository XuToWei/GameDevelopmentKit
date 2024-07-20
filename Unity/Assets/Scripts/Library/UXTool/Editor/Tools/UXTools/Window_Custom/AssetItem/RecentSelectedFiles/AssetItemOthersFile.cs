#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class AssetItemOthersFile : AssetItemBase
    {
        private readonly Object _assetObj;

        public AssetItemOthersFile(FileInfo fileInfo, float scale = 1) : base(fileInfo, scale)
        {
            ResourceType = ResourceType.Others;
            _assetObj = AssetDatabase.LoadAssetAtPath<Object>(FilePath);

            // 获取材质预览图
            Texture2D filePreview = AssetPreview.GetAssetPreview(_assetObj) ?? AssetPreview.GetMiniThumbnail(_assetObj);
            SetThumbnail(filePreview);
            SetIcon(filePreview);
        }

    }
}
#endif