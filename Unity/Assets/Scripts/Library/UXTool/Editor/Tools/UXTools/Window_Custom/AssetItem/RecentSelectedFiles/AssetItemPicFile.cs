#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class AssetItemPicFile : AssetItemBase
    {
        private readonly Texture2D _assetObj;
        public AssetItemPicFile(FileInfo fileInfo, float scale = 1) : base(fileInfo, scale)
        {
            ResourceType = ResourceType.Picture;

            _assetObj = AssetDatabase.LoadAssetAtPath<Texture2D>(FilePath);

            var pictureIcon = ToolUtils.GetIcon("PictureIcon");

            SetThumbnail(_assetObj ? _assetObj : pictureIcon);

            SetIcon(pictureIcon);

        }


    }
}
#endif