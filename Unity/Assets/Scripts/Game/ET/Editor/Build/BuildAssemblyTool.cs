#if UNITY_HOTFIX
using Game.Editor;
using UnityEditor;
using UnityEditor.Build.Player;

namespace ET.Editor
{
    public static class BuildAssemblyTool
    {
        public static readonly string CodeDir = "Assets/Res/ET/Code";
        public static readonly string[] ExtraScriptingDefines = new[] { "UNITY_COMPILE", "UNITY_ET" };
        public static readonly string[] DllNames = new[] { "Game.ET.Code.Model", "Game.ET.Code.ModelView", "Game.ET.Code.Hotfix", "Game.ET.Code.HotfixView" };

        public static void Build(BuildTarget target, ScriptCompilationOptions options)
        {
            BuildAssemblyHelper.CompileDlls(target, ExtraScriptingDefines, options);
            BuildAssemblyHelper.CopyHotUpdateDlls(target, CodeDir, DllNames);
            AssetDatabase.Refresh();
        }

        [MenuItem("ET/Compile Dll")]
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