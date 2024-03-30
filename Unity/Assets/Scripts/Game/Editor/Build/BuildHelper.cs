using System.IO;
using GameFramework;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Extension.Editor;

namespace Game.Editor
{
    public static class BuildHelper
    {
        public static readonly string BuildPkgFolder = Path.GetFullPath("../Temp/Pkg");

        // [InitializeOnLoadMethod]
        public static void ReGenerateProjectFiles()
        {
            Unity.CodeEditor.CodeEditor.CurrentEditor.SyncAll();
            Debug.Log("ReGenerateProjectFiles finished.");
        }

        public static void BuildPkg(Platform platform)
        {
            BuildTarget buildTarget = BuildTarget.NoTarget;
            string appName = Application.productName;
            switch (platform)
            {
                case Platform.Windows:
                    buildTarget = BuildTarget.StandaloneWindows;
                    appName += ".exe";
                    break;
                case Platform.Windows64:
                    buildTarget = BuildTarget.StandaloneWindows64;
                    appName += ".exe";
                    break;
                case Platform.Android:
                    buildTarget = BuildTarget.Android;
                    appName += ".apk";
                    break;
                case Platform.IOS:
                    buildTarget = BuildTarget.iOS;
                    break;
                case Platform.MacOS:
                    buildTarget = BuildTarget.StandaloneOSX;
                    break;
                case Platform.Linux:
                    buildTarget = BuildTarget.StandaloneLinux64;
                    break;
                case Platform.WebGL:
                    buildTarget = BuildTarget.WebGL;
                    break;
                default:
                    throw new GameFrameworkException($"No Support {platform}!");
            }

            Debug.Log($"start build {platform}");
            
            string fold = Utility.Path.GetRegularPath($"{BuildPkgFolder}/{platform}");

            if (Directory.Exists(fold))
            {
                FileTool.CleanDirectory(fold);
            }
            else
            {
                Directory.CreateDirectory(fold);
            }
            
            BuildResource(platform);

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            
            string[] levels =
            {
                EntryUtility.EntryScenePath,
            };
            Debug.Log("start build pkg");
            string locationPathName = $"{fold}/{appName}";
#if !UNITY_HOTFIX && UNITY_ET && UNITY_2021//兼容ET的Bson
            EditorUserBuildSettings.il2CppCodeGeneration = Il2CppCodeGeneration.OptimizeSize;
#elif UNITY_2021
            EditorUserBuildSettings.il2CppCodeGeneration = Il2CppCodeGeneration.OptimizeSpeed;
#endif
#if !UNITY_HOTFIX && UNITY_ET && UNITY_2022_1_OR_NEWER//兼容ET的Bson
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(buildTarget)), Il2CppCodeGeneration.OptimizeSize);
#elif UNITY_2022_1_OR_NEWER
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(buildTarget)), Il2CppCodeGeneration.OptimizeSpeed);
#endif
            BuildReport buildReport = BuildPipeline.BuildPlayer(levels, locationPathName, buildTarget, BuildOptions.None);
            if (buildReport.summary.result != BuildResult.Succeeded)
            {
                throw new GameFrameworkException($"build pkg fail : { buildReport.summary.result }");
            }
            Debug.Log($"finish build pkg at {locationPathName}");
            EditorUtility.OpenWithDefaultApp(fold);
        }

        public static void BuildResource(Platform platform)
        {
            Debug.Log("start refresh resource collection");
            ResourceRuleEditorUtility.RefreshResourceCollection();
            Debug.Log("finish refresh resource collection");
            
            Debug.Log("start build resource");
            ResourceBuildHelper.StartBuild(platform);
            Debug.Log("finish build resource");
        }

        public static void RefreshWindowsPkgResource()
        {
            string targetPath = $"{BuildPkgFolder}/{Platform.Windows}/{Application.productName}_Data/StreamingAssets/";
            if (!Directory.Exists(targetPath))
            {
                throw new GameFrameworkException($"RefreshExePkgResource fail! {targetPath} not exist!");
            }
            
            BuildResource(Platform.Windows);
            
            FileTool.CleanDirectory(targetPath);
            string bundleFold = Path.Combine(ResourceBuildHelper.GetNewestBundlePath(), Platform.Windows.ToString());
            bundleFold = Utility.Path.GetRegularPath(bundleFold);
            FileTool.CopyDirectory(bundleFold, targetPath);
            Debug.Log($"src dir: {bundleFold}    target: {targetPath}");
        }
        
        public static void RefreshWindows64PkgResource()
        {
            string targetPath = $"{BuildPkgFolder}/{Platform.Windows64}/{Application.productName}_Data/StreamingAssets/";
            if (!Directory.Exists(targetPath))
            {
                throw new GameFrameworkException($"RefreshExePkgResource fail! {targetPath} not exist!");
            }
            
            BuildResource(Platform.Windows64);
            
            FileTool.CleanDirectory(targetPath);
            string bundleFold = Path.Combine(ResourceBuildHelper.GetNewestBundlePath(), Platform.Windows64.ToString());
            bundleFold = Utility.Path.GetRegularPath(bundleFold);
            FileTool.CopyDirectory(bundleFold, targetPath);
            Debug.Log($"src dir: {bundleFold}    target: {targetPath}");
        }
    }
}