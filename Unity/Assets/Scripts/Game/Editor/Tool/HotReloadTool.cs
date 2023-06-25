using System;
using System.Reflection;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class HotReloadTool
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            FieldInfo field = Utility.Assembly.GetType("SingularityGroup.HotReload.HotReloadSettingsObject")
                .GetField("editorAssetPath", BindingFlags.Static | BindingFlags.Public);
            field.SetValue(null, "Assets/Res/Resources/HotReloadSettingsObject.asset");
        }
    }
}
