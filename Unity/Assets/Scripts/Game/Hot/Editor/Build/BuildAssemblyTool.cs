#if UNITY_HOTFIX
using System.IO;
using Game.Editor;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

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
            //删掉Library中Unity编译的dll，不然在编辑器下Assembly.Load多个dll时，dll会与Library中的dll引用错乱
            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.ExitingEditMode)
                {
                    if (CodeRunnerUtility.IsEnableEditorCodeBytesMode())
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