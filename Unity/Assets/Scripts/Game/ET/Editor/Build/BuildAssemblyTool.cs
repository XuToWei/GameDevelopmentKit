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
            CompileAssemblyHelper.CompileDlls(target, ExtraScriptingDefines, options);
            CompileAssemblyHelper.CopyHotUpdateDlls(target, CodeDir, DllNames);
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

        public static void BuildHotfix()
        {
            // 傻屌Unity在这里搞了个傻逼优化，认为同一个路径的dll，返回的程序集就一样。所以这里每次编译都要随机名字
            string[] logicFiles = Directory.GetFiles(Define.ReloadHotfixDir, "Hotfix_*");
            foreach (string file in logicFiles)
            {
                File.Delete(file);
            }
            int random = RandomGenerator.RandomNumber(100000000, 999999999);

            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            Build(target);
            string buildDir = CompileAssemblyHelper.GetBuildTargetDir(target);
            File.Copy($"{buildDir}/Game.ET.Code.Hotfix.dll", $"{Define.ReloadHotfixDir}/Game.ET.Code.Hotfix_{random}.dll.bytes", true);
            File.Copy($"{buildDir}/Game.ET.Code.Hotfix.pdb", $"{Define.ReloadHotfixDir}/Game.ET.Code.Hotfix_{random}.pdb.bytes", true);
            Debug.Log($"copy:{buildDir}/Game.ET.Code.Hotfix => {Define.ReloadHotfixDir}/Game.ET.Code.Hotfix_{random}");
            File.Copy($"{buildDir}/Game.ET.Code.HotfixView.dll", $"{Define.ReloadHotfixDir}/Game.ET.Code.HotfixView_{random}.dll.bytes", true);
            File.Copy($"{buildDir}/Game.ET.Code.HotfixView.pdb", $"{Define.ReloadHotfixDir}/Game.ET.Code.HotfixView_{random}.pdb.bytes", true);
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
            //删掉Library中Unity编译的dll，不然在编辑器下Assembly.Load多个dll时，dll会与Library中的dll引用错乱
            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.ExitingEditMode)
                {
                    if (CodeRunnerUtility.IsEditorCodeBytesMode())
                    {
                        foreach (var dll in DllNames)
                        {
                            string dllFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.dll";
                            if (File.Exists(dllFile))
                            {
                                string dllDisableFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.dll.DISABLE";
                                if (File.Exists(dllDisableFile))
                                {
                                    File.Delete(dllDisableFile);
                                }

                                File.Move(dllFile, dllDisableFile);
                            }

                            string pdbFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.pdb";
                            if (File.Exists(pdbFile))
                            {
                                string pdbDisableFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.pdb.DISABLE";
                                if (File.Exists(pdbDisableFile))
                                {
                                    File.Delete(pdbDisableFile);
                                }

                                File.Move(pdbFile, pdbDisableFile);
                            }
                        }
                    }
                    else
                    {
                        foreach (var dll in DllNames)
                        {
                            string dllFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.dll";
                            string dllDisableFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.dll.DISABLE";
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

                            string pdbDisableFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.pdb.DISABLE";
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
            };
        }
    }
}
#endif