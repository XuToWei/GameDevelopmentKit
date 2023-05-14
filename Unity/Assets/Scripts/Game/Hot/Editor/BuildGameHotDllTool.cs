using System;
using System.Collections.Generic;
using System.IO;
using Game.Editor;
using UnityEditor;
using UnityEditor.Compilation;

namespace Game.Hot.Editor
{
    internal static class BuildGameHotDllTool
    {
        private static readonly List<string> s_Codes = new List<string>()
        {
            "Assets/Scripts/Game/Hot/Code/Runtime",
        };
        
        private static string s_CodeDir = "Assets/Res/Hot";

        internal const string DllName = "Game.Hot";
        
        [MenuItem("GameHot/Build GameHot Dll")]
        internal static void Build()
        {
            BuildAssemblyHelper.Build(DllName, s_Codes, Array.Empty<string>(), Array.Empty<string>(), CompilationPipeline.codeOptimization);
            File.Copy(Path.Combine(BuildAssemblyHelper.BuildOutputDir, $"{DllName}.dll"), Path.Combine(s_CodeDir, $"{DllName}.dll.bytes"), true);
            File.Copy(Path.Combine(BuildAssemblyHelper.BuildOutputDir, $"{DllName}.pdb"), Path.Combine(s_CodeDir, $"{DllName}.pdb.bytes"), true);
            AssetDatabase.Refresh();
        }
    }
}