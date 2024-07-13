#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    public class AssetItemPrefab : AssetItemBase
    {
        protected readonly GameObject AssetObj;

        protected AssetItemPrefab(FileInfo fileInfo, float scale = 1) : base(fileInfo, scale)
        {
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(FilePath);
            AssetObj = obj;

            // 获取缩略图
            if (string.IsNullOrEmpty(FilePath)) return;
            var guid = AssetDatabase.AssetPathToGUID(FilePath);
            var previewTex = Utils.GetAssetsPreviewTexture(guid, 144);
            SetThumbnail(previewTex);

            // 获取图标
            var icon = ToolUtils.GetIcon("PrefabIcon");
            SetIcon(icon);
        }
    }
}
#endif