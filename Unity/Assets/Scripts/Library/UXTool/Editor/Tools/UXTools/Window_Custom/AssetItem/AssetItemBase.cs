#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public enum ResourceType
    {
        Default,
        Picture,
        Prefab,
        Anim,
        Material,
        Others
    }

    /// <summary>
    /// 界面元素基类
    /// </summary>
    public class AssetItemBase : VisualElement
    {
        private const int BorderWidth = 1;
        private const int SizeW = 156; //资产块初始大小
        private const int LabelHeight = 20; // 名称高度
        private readonly Image _thumbnail; // 缩略图
        private readonly Image _itemIcon;
        private readonly VisualElement _upContainer;

        protected readonly UXBuilderDiv Row;

        public readonly string FilePath = string.Empty;
        public readonly string FileName = string.Empty;
        public ResourceType ResourceType = ResourceType.Default;
        public bool Selected;

        protected AssetItemBase(FileInfo fileInfo, float scale = 1)
        {
            if (fileInfo.DirectoryName == null) return;
            var tmp = fileInfo.DirectoryName.Replace("\\", "/");
            FilePath = FileUtil.GetProjectRelativePath(tmp) + "/" + fileInfo.Name;
            FileName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            style.width = scale == 0 ? Length.Percent(100) : SizeW * scale;
            style.marginRight = 12;
            style.marginBottom = scale == 0 ? 0 : 12;

            Row = UXBuilder.Div(this, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    width = Length.Percent(100),
                    borderTopWidth = BorderWidth,
                    borderBottomWidth = BorderWidth,
                    borderLeftWidth = BorderWidth,
                    borderRightWidth = BorderWidth,
                    paddingBottom = 1,
                    paddingLeft = 1,
                    paddingRight = 1,
                    paddingTop = 1,
                    flexDirection = scale == 0 ? FlexDirection.Row : FlexDirection.Column,
                }
            });
            Row.tooltip = FileName;

            // 列表模式下不显示缩略图
            if (scale != 0)
            {
                _upContainer = UXBuilder.Div(Row, new UXBuilderDivStruct()
                {
                    style = new UXStyle()
                    {
                        alignItems = Align.Center,
                        backgroundColor = new Color(63f / 255f, 63f / 255f, 63f / 255f),
                        height = scale == 0 ? 20 : 154 * scale,
                    }
                });
                _upContainer.name = "UpContainer";

                // 缩略图设置
                _thumbnail = new Image();
                _upContainer.Add(_thumbnail);
                _thumbnail.style.height = Length.Percent(100);
                _thumbnail.style.width = Length.Percent(100);
            }

            // label区域
            VisualElement downContainer = UXBuilder.Div(Row, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    width = Length.Percent(100),
                    height = LabelHeight,
                    flexDirection = FlexDirection.Row,
                }
            });
            downContainer.name = "DownContainer";

            _itemIcon = new Image()
            {
                style =
                {
                    width = LabelHeight,
                    height = LabelHeight,
                }
            };
            downContainer.Add(_itemIcon);

            // label
            UXBuilder.Text(downContainer, new UXBuilderTextStruct()
            {
                style = new UXStyle()
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    fontSize = 14,
                    color = Color.white,
                    unityTextAlign = scale == 0 ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter,
                    overflow = Overflow.Hidden,
#if UNITY_2020_3_OR_NEWER
                    textOverflow = TextOverflow.Ellipsis,
#endif
                    whiteSpace = WhiteSpace.NoWrap,
                },
                text = FileName,
            });

            Row.RegisterCallback<MouseEnterEvent>(OnHoverStateChange);
            Row.RegisterCallback<MouseLeaveEvent>(OnHoverStateChange);
            Row.RegisterCallback<MouseDownEvent>(OnClick);
        }

        protected virtual void OnClick(MouseDownEvent e)
        {
        }

        private void OnHoverStateChange(EventBase e)
        {
            if (e.eventTypeId == MouseEnterEvent.TypeId())
            {
                Row.style.backgroundColor = new Color(36f / 255f, 99f / 255f, 193f / 255f, 0.5f);
                if (_upContainer != null)
                {
                    _upContainer.style.backgroundColor = Color.clear;
                }
            }
            else if (e.eventTypeId == MouseLeaveEvent.TypeId())
            {
                Row.style.backgroundColor = Color.clear;
                if (_upContainer != null)
                {
                    _upContainer.style.backgroundColor = new Color(63f / 255f, 63f / 255f, 63f / 255f);
                }

            }
        }

        protected void SetThumbnail(Texture texture)
        {
            if (_thumbnail != null)
            {
                _thumbnail.image = texture;
            }
        }

        protected void SetIcon(Texture iconTexture)
        {
            if (_itemIcon != null)
            {
                _itemIcon.image = iconTexture;
            }
        }

        /// <summary>
        /// 选择界面资源后，更改其选中状态，且在info区域显示信息
        /// </summary>
        /// <param name="selected">是否选择</param>
        /// <param name="icon">资源图标</param>
        /// <param name="filePath">资源路径</param>
        public void SetSelected(bool selected, out Texture icon, out string filePath)
        {
            SetSelectedUI(selected);
            icon = _itemIcon.image;
            filePath = FilePath;
        }

        /// <summary>
        /// 选择界面资源，更改其选中状态
        /// </summary>
        /// <param name="selected"></param>
        public void SetSelected(bool selected)
        {
            SetSelectedUI(selected);
        }

        private void SetSelectedUI(bool selected)
        {
            Selected = selected;
            if (Selected)
            {
                Color myColor = new Color(27f / 255f, 150f / 255f, 233f / 255f, 1f);
                Row.style.borderTopColor = myColor;
                Row.style.borderBottomColor = myColor;
                Row.style.borderLeftColor = myColor;
                Row.style.borderRightColor = myColor;
            }
            else
            {
                Color myColor = Color.clear;
                Row.style.borderTopColor = myColor;
                Row.style.borderBottomColor = myColor;
                Row.style.borderLeftColor = myColor;
                Row.style.borderRightColor = myColor;
            }
        }
    }
}
#endif