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
            "Assets/Plugins/Demigiant",
            "Assets/Plugins/Sirenix",
        };

        private static readonly string s_UnityPackageFile = "../Tools/UnityPlugins/Useful.unitypackage";
        private static readonly string s_GitIgnoreFile = "Assets/.gitignore";

        [MenuItem("Game/For Git Dev/Export All Not Free Plugins", false, 999)]
        static void ExportAllNotFreePlugins()
        {
            AssetDatabase.ExportPackage(s_IncludePlugins, s_UnityPackageFile, ExportPackageOptions.Recurse);
            MakeGitIgnore(s_IncludePlugins);
        }

        [MenuItem("Game/For Git Dev/Make .gitignore For Not Free Plugins", false, 998)]
        static void MakeGitIgnoreForAllNotFreePlugins()
        {
            MakeGitIgnore(s_IncludePlugins);
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