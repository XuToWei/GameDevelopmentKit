using System;
using Game.Editor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace ET.Editor
{
    public class BuildToolEditor: EditorWindow
    {
        private CodeOptimization codeOptimization = CodeOptimization.Debug;
        private CodeMode codeMode;

        [MenuItem("ET/Build Tool")]
        public static void ShowWindow()
        {
            GetWindow<BuildToolEditor>("Build Tool", DockDefine.Types);
        }

        private void OnEnable()
        {
            this.codeMode = Define.CodeMode;
        }

        private void OnGUI()
        {
            GUIStyle titleGUIStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            
            GUILayout.Label("Code Compile", titleGUIStyle);
            
            this.codeOptimization = (CodeOptimization)EditorGUILayout.EnumPopup("CodeOptimization:", this.codeOptimization);

            this.codeMode = (CodeMode)EditorGUILayout.EnumPopup("CodeMode:", this.codeMode);
            if (this.codeMode != Define.CodeMode)
            {
#if UNITY_ET_CODEMODE_CLIENT
                if (this.codeMode == CodeMode.Server)
                {
                    CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_SERVER();
                }
                else if (this.codeMode == CodeMode.ClientServer)
                {
                    CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_CLIENTSERVER();
                }
#elif UNITY_ET_CODEMODE_SERVER
                if (this.codeMode == CodeMode.Client)
                {
                    CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_CLIENT();
                }
                else if (this.codeMode == CodeMode.ClientServer)
                {
                    CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_CLIENTSERVER();
                }
#elif  UNITY_ET_CODEMODE_CLIENTSERVER
                if (this.codeMode == CodeMode.Client)
                {
                    CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_CLIENT();
                }
                else if (this.codeMode == CodeMode.Server)
                {
                    CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_SERVER();
                }
#endif
            }
#if UNITY_HOTFIX
            if (GUILayout.Button("Compile Dll"))
            {
                if (!Define.EnableHotfix)
                {
                    throw new Exception("now in UNITY_ET_CODE mode, do not need Build!");
                }

                BuildAssemblyTool.Build();

                AfterCompiling();

                Debug.Log("Build Hotfix Success!");
            }
#endif
            GUILayout.Label("Tool", titleGUIStyle);
            
            if (GUILayout.Button("ReGenerateProjectFiles"))
            {
                BuildHelper.ReGenerateProjectFiles();
            }
            
            if (GUILayout.Button("ExcelExporter"))
            {
                ToolEditor.ExcelExporter();
            }

            if (GUILayout.Button("Proto2CS"))
            {
                ToolEditor.Proto2CS();
            }

            GUILayout.Space(5);
        }

        private static void AfterCompiling()
        {
            AssetDatabase.Refresh();

            Debug.Log("build success!");
        }
    }
}