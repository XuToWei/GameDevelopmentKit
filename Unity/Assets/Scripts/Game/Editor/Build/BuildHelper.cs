using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Extension.Editor;

namespace Game.Editor
{
    public enum PlatformType
    {
        None,
        Android,
        IOS,
        Windows,
        MacOS,
        Linux
    }

    public static class BuildHelper
    {
        private const string relativeDirPrefix = "../../Temp";

        public static string BuildFolder = "../../Temp";

        public static void BuildResource()
        {
        }

        public static void BuildPkg(PlatformType type, bool isBuildExe, bool buildResource, bool clearFolder)
        {
            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
            string exeName = string.Empty;
            switch (type)
            {
                case PlatformType.Windows:
                    buildTarget = BuildTarget.StandaloneWindows64;
                    exeName = Application.productName + ".exe";
                    break;
                case PlatformType.Android:
                    buildTarget = BuildTarget.Android;
                    exeName += Application.productName + ".apk";
                    break;
                case PlatformType.IOS:
                    buildTarget = BuildTarget.iOS;
                    break;
                case PlatformType.MacOS:
                    buildTarget = BuildTarget.StandaloneOSX;
                    break;

                case PlatformType.Linux:
                    buildTarget = BuildTarget.StandaloneLinux64;
                    break;
            }

            string fold = string.Format(BuildFolder, type);

            if (clearFolder && Directory.Exists(fold))
            {
                Directory.Delete(fold, true);
            }

            Directory.CreateDirectory(fold);

            if (buildResource)
            {
                Debug.Log("start build resource");
                ResourceBuildHelper.StartBuild();
                Debug.Log("finish build resource");
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            string[] levels =
            {
                "Assets/Launcher.unity",
            };
            Debug.Log("start build pkg");
            BuildPipeline.BuildPlayer(levels, $"{relativeDirPrefix}/{exeName}", buildTarget, BuildOptions.None);
            Debug.Log("finish build pkg");
            
            if (isBuildExe)
            {
                
            }
            else
            {
                if (buildResource && type == PlatformType.Windows)
                {
                    string targetPath = Path.Combine(relativeDirPrefix, $"{Application.productName}_Data/StreamingAssets/");
                    Directory.Delete(targetPath, true);
                    Debug.Log($"src dir: {fold}    target: {targetPath}");
                    FileHelper.CopyDirectory(fold, targetPath);
                }
            }
        }
    }
}