#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class AssetItemAnimFile : AssetItemBase
    {
        private readonly AnimationClip _assetObj;

        public AssetItemAnimFile(FileInfo fileInfo, float scale = 1) : base(fileInfo, scale)
        {
            ResourceType = ResourceType.Anim;

            _assetObj = AssetDatabase.LoadAssetAtPath<AnimationClip>(FilePath);
            // 获取预览图
            var animPreview = ToolUtils.GetIcon("AnimationClipIcon");
            SetThumbnail(animPreview);
            SetIcon(animPreview);
        }


    }
}
#endif