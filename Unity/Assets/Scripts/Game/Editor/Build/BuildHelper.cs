using System.IO;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Extension.Editor;

namespace Game.Editor
{
    public static class BuildHelper
    {
        private static readonly string BuildFolder = "../Temp/Pkg";
        
        public static void BuildPkg(Platform platform)
        {
            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
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
            
            string fold = $"{BuildFolder}/{platform}";

            if (Directory.Exists(fold))
            {
                FileTool.CleanDirectory(fold);
            }
            else
            {
                Directory.CreateDirectory(fold);
            }

            Debug.Log("start build resource");
            ResourceBuildHelper.StartBuild(platform);
            Debug.Log("finish build resource");

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            
            string[] levels =
            {
                "Assets/Launcher.unity",
            };
            Debug.Log("start build pkg");
            string locationPathName = $"{fold}/{appName}";
            BuildPipeline.BuildPlayer(levels, locationPathName, buildTarget, BuildOptions.None);
            Debug.Log($"finish build pkg at {locationPathName}");
        }

        public static void RefreshWindowsPkgResource()
        {
            string fold = $"{BuildFolder}/{Platform.Windows}";
            string targetPath = Path.Combine(fold, $"{Application.productName}_Data/StreamingAssets/");
            if (!Directory.Exists(targetPath))
            {
                throw new GameFrameworkException($"RefreshExePkgResource fail! {targetPath} not exist!");
            }
            
            Debug.Log("start build resource");
            ResourceBuildHelper.StartBuild(Platform.Windows);
            Debug.Log("finish build resource");
            
            FileTool.CleanDirectory(targetPath);
            FileTool.CopyDirectory(fold, targetPath);
            Debug.Log($"src dir: {fold}    target: {targetPath}");
        }
        
        public static void RefreshWindows64PkgResource()
        {
            string fold = $"{BuildFolder}/{Platform.Windows64}";
            string targetPath = Path.Combine(fold, $"{Application.productName}_Data/StreamingAssets/");
            if (!Directory.Exists(targetPath))
            {
                throw new GameFrameworkException($"RefreshExePkgResource fail! {targetPath} not exist!");
            }
            
            Debug.Log("start build resource");
            ResourceBuildHelper.StartBuild(Platform.Windows64);
            Debug.Log("finish build resource");
            
            FileTool.CleanDirectory(targetPath);
            FileTool.CopyDirectory(fold, targetPath);
            Debug.Log($"src dir: {fold}    target: {targetPath}");
        }
    }
}