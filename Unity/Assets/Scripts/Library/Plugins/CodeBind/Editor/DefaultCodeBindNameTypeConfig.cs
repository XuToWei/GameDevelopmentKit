using System;
using System.Collections.Generic;

namespace CodeBind.Editor
{
    public class DefaultCodeBindNameTypeConfig: ICodeBindNameTypeConfig
    {
        public Dictionary<string, Type> BindNameTypeDict { get; } = new Dictionary<string, Type>()
        {
            { "Transform", typeof (UnityEngine.Transform) },
            { "Animation", typeof (UnityEngine.Animation) },
            { "Animator", typeof (UnityEngine.Animator) },
            { "RectTransform", typeof (UnityEngine.RectTransform) },
            { "Canvas", typeof (UnityEngine.Canvas) },
            { "CanvasGroup", typeof (UnityEngine.CanvasGroup) },
            { "VerticalLayoutGroup", typeof (UnityEngine.UI.VerticalLayoutGroup) },
            { "HorizontalLayoutGroup", typeof (UnityEngine.UI.HorizontalLayoutGroup) },
            { "GridLayoutGroup", typeof (UnityEngine.UI.GridLayoutGroup) },
            { "ToggleGroup", typeof (UnityEngine.UI.ToggleGroup) },
            { "Button", typeof (UnityEngine.UI.Button) },
            { "Image", typeof (UnityEngine.UI.Image) },
            { "RawImage", typeof (UnityEngine.UI.RawImage) },
            { "Text", typeof (UnityEngine.UI.Text) },
            { "InputField", typeof (UnityEngine.UI.InputField) },
            { "Slider", typeof (UnityEngine.UI.Slider) },
            { "Mask", typeof (UnityEngine.UI.Mask) },
            { "RectMask2D", typeof (UnityEngine.UI.RectMask2D) },
            { "Toggle", typeof (UnityEngine.UI.Toggle) },
            { "Scrollbar", typeof (UnityEngine.UI.Scrollbar) },
            { "ScrollRect", typeof (UnityEngine.UI.ScrollRect) },
            { "Dropdown", typeof (UnityEngine.UI.Dropdown) },
            { "TMPText", typeof (TMPro.TMP_Text) },
            { "TMPInputField", typeof (TMPro.TMP_InputField) },
            { "TMPProText", typeof (TMPro.TextMeshProUGUI) },
        };
    }
}