using UnityEngine;
using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LabelTextAttribute : PropertyAttribute
    {
        public string LabelName;
        public string ToolTip;
        public LabelTextAttribute(string name, string tooltip = "")
        {
            LabelName = name;
            ToolTip = tooltip;
        }
    }
}
