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

            if (GUILayout.Button("BuildModelAndHotfix"))
            {
                if (!Define.EnableHotfix)
                {
                    throw new Exception("now in UNITY_ET_CODE mode, do not need Build!");
                }

                BuildAssemblyHelper.BuildModel(this.codeOptimization, this.codeMode);
                BuildAssemblyHelper.BuildHotfix(this.codeOptimization, this.codeMode);

                AfterCompiling();

                ShowNotification("Build Model And Hotfix Success!");
            }

            if (GUILayout.Button("BuildModel"))
            {
                if (!Define.EnableHotfix)
                {
                    throw new Exception("now in UNITY_ET_CODE mode, do not need Build!");
                }

                BuildAssemblyHelper.BuildModel(this.codeOptimization, this.codeMode);

                AfterCompiling();

                ShowNotification("Build Model Success!");
            }

            if (GUILayout.Button("BuildHotfix"))
            {
                if (!Define.EnableHotfix)
                {
                    throw new Exception("now in UNITY_ET_CODE mode, do not need Build!");
                }

                BuildAssemblyHelper.BuildHotfix(this.codeOptimization, this.codeMode);

                AfterCompiling();

                ShowNotification("Build Hotfix Success!");
            }

            GUILayout.Label("Tool", titleGUIStyle);
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

        private static void ShowNotification(string tips)
        {
            Debug.Log(tips);
            EditorWindow game = GetWindow(typeof (EditorWindow).Assembly.GetType("UnityEditor.GameView"));
            if (game != null) game.ShowNotification(new GUIContent($"{tips}"));
        }
    }
}