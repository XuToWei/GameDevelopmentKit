#if UNITY_HOTFIX
using System.IO;
using Game.Editor;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityGameFramework.Extension.Editor;

namespace Game.Hot.Editor
{
    public static class BuildAssemblyTool
    {
        public static readonly string CodeDir = "Assets/Res/Hot/Code";
        public static readonly string[] ExtraScriptingDefines = new[] { "UNITY_COMPILE", "UNITY_GAMEHOT" };
        public static readonly string[] DllNames = new[] { "Game.Hot.Code" };

        public static void Build(BuildTarget target, ScriptCompilationOptions options)
        {
            BuildAssemblyHelper.CompileDlls(target, ExtraScriptingDefines, options);
            BuildAssemblyHelper.CopyHotUpdateDlls(target, CodeDir, DllNames);
            AssetDatabase.Refresh();
        }
        
        [MenuItem("GameHot/Compile Dll")]
        public static void Build()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            ScriptCompilationOptions options = EditorUserBuildSettings.development
                ? ScriptCompilationOptions.DevelopmentBuild
                : ScriptCompilationOptions.None;
            Build(target, options);
        }
        
        public static void Build(BuildTarget target)
        {
            ScriptCompilationOptions options = EditorUserBuildSettings.development
                ? ScriptCompilationOptions.DevelopmentBuild
                : ScriptCompilationOptions.None;
            Build(target, options);
        }
        
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            BuildAssemblyHelper.HandleRuntimeDlls(DllNames);
        }
    }
}
#endif