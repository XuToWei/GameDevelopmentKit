using System;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace ET.Editor
{
    public class BuildToolEditor: EditorWindow
    {
        private CodeOptimization codeOptimization = CodeOptimization.Debug;
        private GlobalConfig globalConfig;

        [MenuItem("ET/Build Tool")]
        public static void ShowWindow()
        {
            GetWindow<BuildToolEditor>("Build Tool", DockDefine.Types);
        }

        private void OnEnable()
        {
            globalConfig = Resources.Load<GlobalConfig>("ET/GlobalConfig");
            
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

            this.globalConfig.CodeMode = (CodeMode)EditorGUILayout.EnumPopup("CodeMode:", this.globalConfig.CodeMode);

            if (GUILayout.Button("BuildModelAndHotfix"))
            {
                if (Define.EnableHotfix)
                {
                    throw new Exception("now in UNITY_ET_CODE mode, do not need Build!");
                }

                BuildAssemblyHelper.BuildModel(this.codeOptimization, globalConfig.CodeMode);
                BuildAssemblyHelper.BuildHotfix(this.codeOptimization, globalConfig.CodeMode);

                AfterCompiling();

                ShowNotification("Build Model And Hotfix Success!");
            }

            if (GUILayout.Button("BuildModel"))
            {
                if (Define.EnableHotfix)
                {
                    throw new Exception("now in UNITY_ET_CODE mode, do not need Build!");
                }

                BuildAssemblyHelper.BuildModel(this.codeOptimization, globalConfig.CodeMode);

                AfterCompiling();

                ShowNotification("Build Model Success!");
            }

            if (GUILayout.Button("BuildHotfix"))
            {
                if (Define.EnableHotfix)
                {
                    throw new Exception("now in UNITY_ET_CODE mode, do not need Build!");
                }

                BuildAssemblyHelper.BuildHotfix(this.codeOptimization, globalConfig.CodeMode);

                AfterCompiling();

                ShowNotification("Build Hotfix Success!");
            }

            GUILayout.Label("Tool", titleGUIStyle);
            if (GUILayout.Button("ExcelExporter"))
            {
                ToolsEditor.ExcelExporter();
            }

            if (GUILayout.Button("Proto2CS"))
            {
                ToolsEditor.Proto2CS();
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
            EditorWindow game = EditorWindow.GetWindow(typeof (EditorWindow).Assembly.GetType("UnityEditor.GameView"));
            if (game != null) game.ShowNotification(new GUIContent($"{tips}"));
        }
    }
}