#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class AssetItemMatFile : AssetItemBase
    {
        private readonly Material _assetObj;

        public AssetItemMatFile(FileInfo fileInfo, float scale = 1) : base(fileInfo, scale)
        {
            ResourceType = ResourceType.Material;
            _assetObj = AssetDatabase.LoadAssetAtPath<Material>(FilePath);

            // 获取材质预览图
            Texture2D materialPreview = AssetPreview.GetAssetPreview(_assetObj);
            SetThumbnail(materialPreview);

            var materialIcon = ToolUtils.GetIcon("MaterialIcon");
            SetIcon(materialIcon);
        }


    }
}
#endif