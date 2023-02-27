using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class BuildToolEditor: EditorWindow
    {
        [MenuItem("Tools/Build Tool")]
        public static void ShowWindow()
        {
            GetWindow<BuildToolEditor>("Build Tool");
        }

        private void OnEnable()
        {
            
        }

        private void OnGUI()
        {
            GUIStyle titleGUIStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            
            GUILayout.Label("Code Compile", titleGUIStyle);
            
            if (GUILayout.Button("BuildModelAndHotfix"))
            {
                AfterCompiling();

                ShowNotification("Build Model And Hotfix Success!");
            }

            if (GUILayout.Button("BuildModel"))
            {
                AfterCompiling();

                ShowNotification("Build Model Success!");
            }

            if (GUILayout.Button("BuildHotfix"))
            {
                ShowNotification("Build Hotfix Success!");
            }

            GUILayout.Label("Tool", titleGUIStyle);
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