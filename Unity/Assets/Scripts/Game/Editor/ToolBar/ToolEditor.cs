using UnityEditor;

namespace Game.Editor
{
    public static class ToolEditor
    {
        public static void ExcelExporter()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            ShellUtility.Run($"{tools} --AppType=ExcelExporter --Console=1", "../Bin/");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        
        public static void Proto2CS()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            ShellUtility.Run($"{tools} --AppType=Proto2CS --Console=1", "../Bin/");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}
