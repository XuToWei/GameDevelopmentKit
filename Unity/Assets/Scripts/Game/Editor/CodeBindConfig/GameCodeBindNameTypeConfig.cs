using System;
using System.Collections.Generic;
using CodeBind;

namespace Game
{
    sealed class GameCodeBindNameTypeConfig
    {
        [CodeBindNameType]
        static Dictionary<string, Type> BindNameTypeDict = new Dictionary<string, Type>()
        {
            { "UIWidget", typeof(UIWidget) },
            { "TMPText", typeof(TMPro.TMP_Text) },
            { "TMPInputField", typeof(TMPro.TMP_InputField) },
            { "TMPProText", typeof(TMPro.TextMeshProUGUI) },
        };
    }
}