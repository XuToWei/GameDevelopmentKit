using System.Linq;
using UnityEditor;

namespace ET.Editor
{
    public static class ChangeDefineToolEditor
    {
                
#if UNITY_ET_CODE
        [MenuItem("ET/ChangeDefine/Remove UNITY_ET_CODE")]
        public static void RemoveEnableCodes()
        {
            EnableCode(false);
        }
#else
        [MenuItem("ET/ChangeDefine/Add UNITY_ET_CODE")]
        public static void AddEnableCodes()
        {
            EnableCode(true);
        }
#endif
        private static void EnableCode(bool enable)
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var ss = defines.Split(';').ToList();
            if (enable)
            {
                if (ss.Contains("UNITY_ET_CODE"))
                {
                    return;
                }
                ss.Add("UNITY_ET_CODE");
            }
            else
            {
                if (!ss.Contains("UNITY_ET_CODE"))
                {
                    return;
                }
                ss.Remove("UNITY_ET_CODE");
            }
            defines = string.Join(";", ss);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
            AssetDatabase.SaveAssets();
        }

#if UNITY_ET_VIEW
        [MenuItem("ET/ChangeDefine/Remove UNITY_ET_VIEW")]
        public static void RemoveEnableView()
        {
            EnableView(false);
        }
#else
        [MenuItem("ET/ChangeDefine/Add UNITY_ET_VIEW")]
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
