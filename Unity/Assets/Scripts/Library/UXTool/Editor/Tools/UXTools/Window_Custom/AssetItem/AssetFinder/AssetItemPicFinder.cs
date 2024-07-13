#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class AssetItemPicFinder : AssetItemBase
    {
        private readonly Texture2D _assetObj;
        public AssetItemPicFinder(FileInfo fileInfo, float scale = 1) : base(fileInfo, scale)
        {
            ResourceType = ResourceType.Picture;

            _assetObj = AssetDatabase.LoadAssetAtPath<Texture2D>(FilePath);

            var pictureIcon = ToolUtils.GetIcon("PictureIcon");

            SetThumbnail(_assetObj ? _assetObj : pictureIcon);

            SetIcon(pictureIcon);

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