#if UNITY_HOTFIX
using System.IO;
using Game.Editor;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityGameFramework.Extension.Editor;

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
            CopyReloadHotfixDlls(target);
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

        private static void CopyReloadHotfixDlls(BuildTarget target)
        {
            // 傻屌Unity在这里搞了个傻逼优化，认为同一个路径的dll，返回的程序集就一样。所以这里每次编译都要随机名字
            Directory.CreateDirectory(Define.ReloadHotfixDir);
            FileHelper.CleanDirectory(Define.ReloadHotfixDir);
            int random = RandomGenerator.RandomNumber(100000000, 999999999);
            string buildDir = BuildAssemblyHelper.GetBuildTargetDir(target);
            File.Copy($"{buildDir}/Game.ET.Code.Hotfix.dll", $"{Define.ReloadHotfixDir}/Game.ET.Code.Hotfix_{random}.dll", true);
            File.Copy($"{buildDir}/Game.ET.Code.Hotfix.pdb", $"{Define.ReloadHotfixDir}/Game.ET.Code.Hotfix_{random}.pdb", true);
            Debug.Log($"copy:{buildDir}/Game.ET.Code.Hotfix => {Define.ReloadHotfixDir}/Game.ET.Code.Hotfix_{random}");
            File.Copy($"{buildDir}/Game.ET.Code.HotfixView.dll", $"{Define.ReloadHotfixDir}/Game.ET.Code.HotfixView_{random}.dll", true);
            File.Copy($"{buildDir}/Game.ET.Code.HotfixView.pdb", $"{Define.ReloadHotfixDir}/Game.ET.Code.HotfixView_{random}.pdb", true);
            Debug.Log($"copy:{buildDir}/Game.ET.Code.HotfixView => {Define.ReloadHotfixDir}/Game.ET.Code.HotfixView_{random}");
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