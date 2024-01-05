using System;
using System.IO;
using System.Threading;
using GameFramework;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Extension.Editor;

namespace Game.Editor
{
    public static class BuildAssemblyHelper
    {
        public static string BuildOutputDir => "./Temp/Bin/Debug";
        private static SynchronizationContext s_UnitySynchronizationContext;

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            s_UnitySynchronizationContext = SynchronizationContext.Current;
        }

        public static string GetBuildTargetDir(BuildTarget target)
        {
            return $"{BuildOutputDir}/{target}";
        }

        public static void CompileDlls(BuildTarget target, string[] extraScriptingDefines = null, ScriptCompilationOptions options = ScriptCompilationOptions.None)
        {
            //防止编辑器关闭了Auto Refresh
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            SynchronizationContext lastSynchronizationContext = null;
            if (Application.isPlaying) //运行时编译需要UnitySynchronizationContext
            {
                lastSynchronizationContext = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(s_UnitySynchronizationContext);
            }
            else
            {
                SynchronizationContext.SetSynchronizationContext(s_UnitySynchronizationContext);
            }
            try
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
            finally
            {
                if (Application.isPlaying && lastSynchronizationContext != null)
                {
                    SynchronizationContext.SetSynchronizationContext(lastSynchronizationContext);
                }
            }
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

        /// <summary>
        /// 处理编辑器运行时编辑器dll和加载的dll重复问题
        /// </summary>
        /// <param name="dllNames"></param>
        public static void HandleRuntimeDlls(string[] dllNames)
        {
            //删掉Library中Unity编译的dll，不然在编辑器下Assembly.Load多个dll时，dll会与Library中的dll引用错乱
            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.ExitingEditMode)
                {
                    DisableRuntimeDlls(dllNames);
                }
                else if (change == PlayModeStateChange.ExitingPlayMode)
                {
                    EnableRuntimeDlls(dllNames);
                }
            };
        }

        private static void DisableRuntimeDlls(string[] dllNames)
        {
            if (CodeRunnerUtility.IsEnableEditorCodeBytesMode())
            {
                foreach (var dll in dllNames)
                {
                    string dllFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.dll";
                    if (File.Exists(dllFile))
                    {
                        string dllDisableFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.dll.DISABLED";
                        if (File.Exists(dllDisableFile))
                        {
                            File.Delete(dllDisableFile);
                        }

                        File.Move(dllFile, dllDisableFile);
                    }

                    string pdbFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.pdb";
                    if (File.Exists(pdbFile))
                    {
                        string pdbDisableFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.pdb.DISABLED";
                        if (File.Exists(pdbDisableFile))
                        {
                            File.Delete(pdbDisableFile);
                        }

                        File.Move(pdbFile, pdbDisableFile);
                    }
                }
            }
        }

        private static void EnableRuntimeDlls(string[] dllNames)
        {
            foreach (var dll in dllNames)
            {
                string dllFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.dll";
                string dllDisableFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.dll.DISABLED";
                if (File.Exists(dllFile))
                {
                    if (File.Exists(dllDisableFile))
                    {
                        File.Delete(dllDisableFile);
                    }
                }
                else
                {
                    if (File.Exists(dllDisableFile))
                    {
                        File.Move(dllDisableFile, dllFile);
                    }
                }

                string pdbDisableFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.pdb.DISABLED";
                string pdbFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.pdb";
                if (File.Exists(pdbFile))
                {
                    if (File.Exists(pdbDisableFile))
                    {
                        File.Delete(pdbDisableFile);
                    }
                }
                else
                {
                    if (File.Exists(pdbDisableFile))
                    {
                        File.Move(pdbDisableFile, pdbFile);
                    }
                }
            }
        }
    }
}
