using System.IO;
using System.Threading;
using GameFramework;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityGameFramework.Editor.ResourceTools;

namespace Game.Editor
{
    public static class BuildAssemblyHelper
    {
        public static string BuildOutputDir => "./Temp/GameBin";

        public static string GetBuildTargetDir(BuildTarget target)
        {
            return $"{BuildOutputDir}/{target}";
        }

        public static void CompileDlls(BuildTarget target, string[] extraScriptingDefines = null, ScriptCompilationOptions options = ScriptCompilationOptions.None)
        {
            string outDir = GetBuildTargetDir(target);
            Directory.CreateDirectory(outDir);
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
            ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings();
            scriptCompilationSettings.group = group;
            scriptCompilationSettings.target = target;
            scriptCompilationSettings.extraScriptingDefines = extraScriptingDefines;
            scriptCompilationSettings.options = options;
            PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, outDir);
#if UNITY_2022
            EditorUtility.ClearProgressBar();
#endif
            Debug.Log("compile finish!!!");
        }

        public static void CopyHotUpdateDlls(BuildTarget target, string desDir, string[] dllNames)
        {
            Directory.CreateDirectory(desDir);
            Directory.CreateDirectory(HybridCLRTool.ExternalHotUpdateAssemblyDir);
            FileTool.CleanDirectory(desDir);
            FileTool.CleanDirectory(HybridCLRTool.ExternalHotUpdateAssemblyDir);
            string buildDir = GetBuildTargetDir(target);
            foreach (var dllName in dllNames)
            {
                string sourceDll = $"{buildDir}/{dllName}.dll";
                string sourcePdb = $"{buildDir}/{dllName}.pdb";
                File.Copy(sourceDll, $"{desDir}/{dllName}.dll.bytes", true);
                File.Copy(sourcePdb, $"{desDir}/{dllName}.pdb.bytes", true);
                //更新HybridCLR热更dlls
                File.Copy(sourceDll, $"{HybridCLRTool.ExternalHotUpdateAssemblyDir}/{dllName}.dll", true);
                File.Copy(sourcePdb, $"{HybridCLRTool.ExternalHotUpdateAssemblyDir}/{dllName}.pdb", true);
                Debug.Log($"copy:{buildDir}/{dllName} => {desDir}/{dllName} , {HybridCLRTool.ExternalHotUpdateAssemblyDir}/{dllName}");
            }
            Debug.Log("copy finish!!!");
        }

        public static BuildTarget GetBuildTarget(Platform platform)
        {
            switch (platform)
            {
                case Platform.Android:
                    return BuildTarget.Android;
                case Platform.Linux:
                    return BuildTarget.StandaloneLinux64;
                case Platform.IOS:
                    return BuildTarget.iOS;
                case Platform.Windows:
                    return BuildTarget.StandaloneWindows;
                case Platform.Windows64:
                    return BuildTarget.StandaloneWindows64;
                case Platform.MacOS:
                    return BuildTarget.StandaloneOSX;
                case Platform.WebGL:
                    return BuildTarget.WebGL;
                default:
                    throw new GameFrameworkException($"{platform} can't support!");
            }
        }
        
        public static void ClearBuildDir()
        {
            FileTool.CleanDirectory(BuildOutputDir);
        }
    }
}
