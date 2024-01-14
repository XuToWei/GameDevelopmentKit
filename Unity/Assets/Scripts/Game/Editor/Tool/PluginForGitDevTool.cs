using System.IO;
using System.Text;
using UnityEditor;

namespace Game.Editor
{
    /// <summary>
    /// 方便导入导出处理收费插件在git上版权问题
    /// </summary>
    public class PluginForGitDevTool
    {
        //需要被导出的文件或文件夹
        private readonly string[] plugins_include = new string[]
        {
            "Assets/Scripts/Game/Editor/Tool/PluginForGitDevTool.cs",
            "Assets/Scripts/Game/Debugger/ToolDebuggerWindow.cs",
            "Assets/Scripts/Library/EnhancedScroller",
            "Assets/Scripts/Library/SLATE Cinematic Sequencer",
            "Assets/Scripts/Library/SmartUiSelection",
            "Assets/Scripts/Library/StompyRobot",
            "Assets/Scripts/Plugins/Demigiant",
            "Assets/Scripts/Plugins/Sirenix",
        };
        
        
        [MenuItem("Game/For Git Dev/Export All Not Free Plugins", false, 999)]
        static void ExportAllNotFreePlugins()
        {
            PluginForGitDevTool tool = new PluginForGitDevTool();
            AssetDatabase.ExportPackage(tool.plugins_include, "../Tools/UnityPlugins/Useful.unitypackage", ExportPackageOptions.Recurse);
            MakeGitIgnore(tool.plugins_include);
        }

        [MenuItem("Game/For Git Dev/Make .gitignore For Not Free Plugins", false, 998)]
        static void MakeGitIgnoreForAllNotFreePlugins()
        {
            PluginForGitDevTool tool = new PluginForGitDevTool();
            MakeGitIgnore(tool.plugins_include);
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

            string gitignoreFile = "Assets/.gitignore";
            if (File.Exists(gitignoreFile))
            {
                File.Delete(gitignoreFile);
            }
            File.WriteAllText(gitignoreFile, stringBuilder.ToString());
        }
    }
}