using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class ToolEditor
    {
        [MenuItem("Game/Tool/ExcelExporter")]
        public static async void ExcelExporter()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            Stopwatch stopwatch = Stopwatch.StartNew();
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            await ShellTool.RunAsync($"{tools} --AppType=ExcelExporter --Console=1", "../Bin/");
#else
            await ShellTool.RunAsync($"{tools} --AppType=ExcelExporter --Console=1 --Customs=GB2312", "../Bin/");
#endif
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Export cost {stopwatch.ElapsedMilliseconds} Milliseconds!");
            LocalizationTool.RefreshData();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        
        [MenuItem("Game/Tool/ExcelExporterForJson")]
        public static async void ExcelExporterForJson()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            Stopwatch stopwatch = Stopwatch.StartNew();
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            await ShellTool.RunAsync($"{tools} --AppType=ExcelExporter --Console=1 --Customs=Json", "../Bin/");
#else
            await ShellTool.RunAsync($"{tools} --AppType=ExcelExporter --Console=1 --Customs=Json,GB2312", "../Bin/");
#endif
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Export cost {stopwatch.ElapsedMilliseconds} Milliseconds!");
            LocalizationTool.RefreshData();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        
        [MenuItem("Game/Tool/Proto2CS")]
        public static void Proto2CS()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            ShellTool.Run($"{tools} --AppType=Proto2CS --Console=1", "../Bin/");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        
        [MenuItem("Game/Tool/StartFileServer")]
        public static void StartFileServer()
        {
            string dotnet = "dotnet.exe";
#if UNITY_EDITOR_OSX
            dotnet = "dotnet";
#endif
            string arguments = "FileServer.dll";
            ProcessTool.Run(dotnet, arguments, "../Bin/");
        }
    }
}
