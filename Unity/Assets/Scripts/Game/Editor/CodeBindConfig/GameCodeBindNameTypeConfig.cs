using System;
using System.Collections.Generic;
using CodeBind;

namespace Game
{
    sealed class GameCodeBindNameTypeConfig
    {
        [CodeBindNameType]
        public static Dictionary<string, Type> BindNameTypeDict = new Dictionary<string, Type>()
        {
            { "UIWidget", typeof(AUIWidget) },
            // TMP
            { "TMPText", typeof(TMPro.TMP_Text) },
            { "TMPInputField", typeof(TMPro.TMP_InputField) },
            { "TextMeshProUGUI", typeof(TMPro.TextMeshProUGUI) },
            { "TextMeshPro", typeof(TMPro.TextMeshPro) },
            // UXTool
            { "UXImage", typeof(UnityEngine.UI.UXImage) },
            { "UXText", typeof(UnityEngine.UI.UXText) },
            { "UXTextMeshPro", typeof(UnityEngine.UI.UXTextMeshPro) },
            { "UXToggle", typeof(UnityEngine.UI.UXToggle) },
            { "UXToggleGroup", typeof(UnityEngine.UI.UXToggleGroup) },
            { "UXScrollRect", typeof(UnityEngine.UI.UXScrollRect) },
            { "UXRolling", typeof(UXRolling) },
            { "UIValueAdapter", typeof(UIValueAdapter) },
            { "LXRolling", typeof(LXRolling) },
            { "UIStateAnimator", typeof(UIStateAnimator) },
            { "UIAdapter", typeof(UIAdapter) },
            { "IgnoreUIAdapter", typeof(IgnoreUIAdapter) },
            { "UIAdapterScaleScreenRate", typeof(UIAdapterScaleScreenRate) },
        };
    }
}