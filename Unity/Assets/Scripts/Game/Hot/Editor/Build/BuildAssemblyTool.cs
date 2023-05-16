using System;
using System.Collections.Generic;
using System.IO;
using Game.Editor;
using UnityEditor;
using UnityEditor.Compilation;

namespace Game.Hot.Editor
{
    public static class BuildAssemblyTool
    {
        private static readonly List<string> s_Codes = new List<string>()
        {
            "Assets/Scripts/Game/Hot/Code/Runtime",
        };
        
        private static string s_CodeDir = "Assets/Res/Hot/Code";

        public const string DllName = "GameHot";
        
        private const CodeOptimization s_CodeOptimization = CodeOptimization.Release;
        
        [MenuItem("GameHot/Build GameHotCode Dll")]
        public static void Build()
        {
            BuildAssemblyHelper.Build(DllName, s_Codes, Array.Empty<string>(), Array.Empty<string>(), s_CodeOptimization);
            File.Copy($"{BuildAssemblyHelper.BuildOutputDir}/{DllName}.dll", $"{s_CodeDir}/{DllName}.dll.bytes", true);
            File.Copy($"{BuildAssemblyHelper.BuildOutputDir}/{DllName}.pdb", $"{s_CodeDir}/{DllName}.pdb.bytes", true);
            AssetDatabase.Refresh();
        }
    }
}