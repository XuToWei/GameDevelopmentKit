using System.Linq;
using UnityEditor;

namespace ET.Editor
{
    public static class ChangeDefineToolEditor
    {
#if UNITY_ET_VIEW
        [MenuItem("ET/Define Symbol/Remove UNITY_ET_VIEW")]
        public static void RemoveEnableView()
        {
            EnableView(false);
        }
#else
        [MenuItem("ET/Define Symbol/Add UNITY_ET_VIEW")]
        public static void AddEnableView()
        {
            EnableView(true);
        }
#endif
        private static void EnableView(bool enable)
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var ss = defines.Split(';').ToList();
            if (enable)
            {
                if (ss.Contains("UNITY_ET_VIEW"))
                {
                    return;
                }
                ss.Add("UNITY_ET_VIEW");
            }
            else
            {
                if (!ss.Contains("UNITY_ET_VIEW"))
                {
                    return;
                }
                ss.Remove("UNITY_ET_VIEW");
            }
            
            defines = string.Join(";", ss);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
            AssetDatabase.SaveAssets();
        }
    }
}
