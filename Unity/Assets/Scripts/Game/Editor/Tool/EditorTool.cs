using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using ThunderFireUITool;
using UnityEditor;

namespace Game.Editor
{
    public static class EditorTool
    {
        [MenuItem("Game/Tool/ExcelExporter")]
        public static void ExcelExporter()
        {
            async UniTaskVoid RunAsync()
            {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                const string TOOL = "./Tool";
#else
                const string TOOL = ".\\Tool.exe";
#endif
                Stopwatch stopwatch = Stopwatch.StartNew();
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                await ShellTool.RunAsync($"{TOOL} --AppType=ExcelExporter --Console=1", "../Bin/");
#else
                await ShellTool.RunAsync($"{TOOL} --AppType=ExcelExporter --Console=1 --Customs=GB2312", "../Bin/");
#endif
                stopwatch.Stop();
                UnityEngine.Debug.Log($"Export cost {stopwatch.ElapsedMilliseconds} Milliseconds!");
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                EditorLocalizationTool.Clear();
                var activeObject = Selection.activeObject;
                if (activeObject != null)
                {
                    Selection.activeObject = null;
                    await UniTask.DelayFrame(2);
                    Selection.activeObject = activeObject;
                }
            }
            RunAsync().Forget();
        }
        
        [MenuItem("Game/Tool/ExcelExporterForJson")]
        public static void ExcelExporterForJson()
        {
            async UniTaskVoid RunAsync()
            {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                const string TOOL = "./Tool";
#else
                const string TOOL = ".\\Tool.exe";
#endif
                Stopwatch stopwatch = Stopwatch.StartNew();
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                await ShellTool.RunAsync($"{TOOL} --AppType=ExcelExporter --Console=1 --Customs=Json", "../Bin/");
#else
                await ShellTool.RunAsync($"{TOOL} --AppType=ExcelExporter --Console=1 --Customs=Json,GB2312", "../Bin/");
#endif
                stopwatch.Stop();
                UnityEngine.Debug.Log($"Export cost {stopwatch.ElapsedMilliseconds} Milliseconds!");
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                EditorLocalizationTool.Clear();
                var activeObject = Selection.activeObject;
                if (activeObject != null)
                {
                    Selection.activeObject = null;
                    await UniTask.DelayFrame(2);
                    Selection.activeObject = activeObject;
                }
            }
            RunAsync().Forget();
        }
        
        [MenuItem("Game/Tool/Proto2CS")]
        public static void Proto2CS()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string TOOL = "./Tool";
#else
            const string TOOL = ".\\Tool.exe";
#endif
            ShellTool.Run($"{TOOL} --AppType=Proto2CS --Console=1", "../Bin/");
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
