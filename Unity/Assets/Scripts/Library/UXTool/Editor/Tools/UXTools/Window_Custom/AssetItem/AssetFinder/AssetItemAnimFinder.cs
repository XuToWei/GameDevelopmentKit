#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class AssetItemAnimFinder : AssetItemBase
    {
        private readonly AnimationClip _assetObj;

        public AssetItemAnimFinder(FileInfo fileInfo, float scale = 1) : base(fileInfo, scale)
        {
            ResourceType = ResourceType.Anim;

            _assetObj = AssetDatabase.LoadAssetAtPath<AnimationClip>(FilePath);
            // 获取预览图
            var animPreview = ToolUtils.GetIcon("AnimationClipIcon");
            SetThumbnail(animPreview);
            SetIcon(animPreview);
        }

        protected override void OnClick(MouseDownEvent e)
        {
            switch (e.button)
            {
                case 0:
                    {
                        // Left Mouse Button
                        if (e.clickCount == 2)
                        {
                            Selection.activeObject = _assetObj;
                        }

                        break;
                    }
                case 1:
                    break;
            }
        }
    }
}
#endif