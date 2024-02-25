#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ThunderFireUITool
{
    public class WidgetRepositoryConfig
    {
        //label
        public static readonly string AddNewLabelText = "+";
        public static readonly string NoneLabelText = "All";

        //Pack
        public static readonly string UnpackText = "Unpack";
        public static readonly string PackText = "Pack";
    }
}
#endif