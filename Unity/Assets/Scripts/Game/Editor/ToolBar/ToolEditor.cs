using System.Diagnostics;
using UnityEditor;

namespace Game.Editor
{
    public static class ToolEditor
    {
        [MenuItem("Tools/Tool/ExcelExporter")]
        public static void ExcelExporter()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            Stopwatch stopwatch = Stopwatch.StartNew();
            ShellUtility.Run($"{tools} --AppType=ExcelExporter --Console=1", "../Bin/");
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Export cost {stopwatch.ElapsedMilliseconds} Milliseconds!");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        
        [MenuItem("Tools/Tool/ExcelExporterForJson")]
        public static void ExcelExporterForJson()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            Stopwatch stopwatch = Stopwatch.StartNew();
            ShellUtility.Run($"{tools} --AppType=ExcelExporter --Console=1 --Custom=Json", "../Bin/");
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Export cost {stopwatch.ElapsedMilliseconds} Milliseconds!");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        
        [MenuItem("Tools/Tool/Proto2CS")]
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
