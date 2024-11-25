using System.IO;
using System.Text;
using UnityEditor;

namespace Game.Editor
{
    /// <summary>
    /// 方便导入导出处理收费插件在git上版权问题
    /// </summary>
    public static class PluginsForGitDevTool
    {
        //需要被导出的文件或文件夹
        private static readonly string[] s_IncludePlugins = new string[]
        {
            "Assets/Scripts/Library/EnhancedScroller",
            "Assets/Scripts/Library/SLATE Cinematic Sequencer",
            "Assets/Scripts/Library/SmartUiSelection",
            "Assets/Scripts/Library/StompyRobot",
            "Assets/Scripts/Library/uNode",
            "Assets/Plugins/Demigiant",
            "Assets/Plugins/Sirenix",
            "Assets/Plugins/Animancer",
        };

        //需要被导出的Demo
        private static readonly string[] s_IncludePluginsDemos = new string[]
        {
            "Assets/Scripts/Library/EnhancedScroller/Demos",
            "Assets/Plugins/Demigiant/DOTweenPro Examples",
            "Assets/Plugins/Sirenix/Demos",
            "Assets/Plugins/Animancer/Examples",
        };

        private static readonly string s_UnityPackageFile = "../Tools/UnityPlugins/Useful.unitypackage";
        private static readonly string s_UnityPackageDemoFile = "../Tools/UnityPlugins/Useful_Demo.unitypackage";
        private static readonly string s_GitIgnoreFile = "Assets/.gitignore";

        [MenuItem("Game/For Git Dev/Export All Not Free Plugins", false, 990)]
        static void ExportAllNotFreePlugins()
        {
            AssetDatabase.ExportPackage(s_IncludePluginsDemos, s_UnityPackageDemoFile, ExportPackageOptions.Recurse);
            RemovePluginsDemo();
            AssetDatabase.ExportPackage(s_IncludePlugins, s_UnityPackageFile, ExportPackageOptions.Recurse);
            MakeGitIgnore(s_IncludePlugins);
        }

        [MenuItem("Game/For Git Dev/Make .gitignore For Not Free Plugins", false, 991)]
        static void MakeGitIgnoreForAllNotFreePlugins()
        {
            MakeGitIgnore(s_IncludePlugins);
        }

        [MenuItem("Game/For Git Dev/Remove Plugins Demo", false, 992)]
        static void RemovePluginsDemo()
        {
            foreach (var dir in s_IncludePluginsDemos)
            {
                if (Directory.Exists(dir))
                {
                    foreach (string subDir in Directory.GetDirectories(dir))
                    {
                        Directory.Delete(subDir, true);
                    }
                    foreach (string subFile in Directory.GetFiles(dir))
                    {
                        File.Delete(subFile);
                    }
                }
                string metaFile = $"{dir}.meta";
                if (File.Exists(metaFile))
                {
                    File.Delete(metaFile);
                }
            }
        }

        static void MakeGitIgnore(string[] pluginsInclude)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var include in pluginsInclude)
            {
                string str = include.Replace("Assets", "");
                stringBuilder.AppendLine(str);
                stringBuilder.AppendLine($"{str}.meta");
            }
            if (File.Exists(s_GitIgnoreFile))
            {
                File.Delete(s_GitIgnoreFile);
            }
            File.WriteAllText(s_GitIgnoreFile, stringBuilder.ToString());
        }
    }
}