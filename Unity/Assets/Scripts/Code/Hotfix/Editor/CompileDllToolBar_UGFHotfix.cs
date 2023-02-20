#if UNITY_HOTFIX
using System;
using System.Collections.Generic;
using System.IO;
using Game.Editor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityToolbarExtender;

namespace Game.Hotfix.Editor
{
    [InitializeOnLoad]
    internal sealed class CompileDllToolBar_UGFHotfix
    {
        private static readonly List<string> s_Codes = new List<string>()
        {
            "Assets/Scripts/Code/Hotfix/Code/Runtime",
        };
        
        private const string s_CodeDir = "Assets/Res/Hotfix";
        
        private const CodeOptimization s_CodeOptimization = CodeOptimization.Debug;
        
        private const string s_DllName = "UGF.Hotfix";
        
        private static readonly GUIContent s_ButtonGUIContent;
        
        static CompileDllToolBar_UGFHotfix()
        {
            s_ButtonGUIContent = new GUIContent($"Compile {s_DllName}", $"Compile {s_DllName} Dll.");
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_ButtonGUIContent))
            {
                BuildAssemblyTool.Build(s_DllName, s_Codes, Array.Empty<string>(), Array.Empty<string>(), s_CodeOptimization);
                File.Copy(Path.Combine(BuildAssemblyTool.BuildOutputDir, $"{s_DllName}.dll"), Path.Combine(s_CodeDir, $"{s_DllName}.dll.bytes"), true);
                File.Copy(Path.Combine(BuildAssemblyTool.BuildOutputDir, $"{s_DllName}.pdb"), Path.Combine(s_CodeDir, $"{s_DllName}.pdb.bytes"), true);
                AssetDatabase.Refresh();
                Debug.Log($"Build dll {s_DllName} to {s_CodeDir} success!");
            }
        }
    }
}
#endif