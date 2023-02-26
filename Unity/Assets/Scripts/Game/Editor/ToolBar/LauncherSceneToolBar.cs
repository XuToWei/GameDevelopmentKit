using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

namespace Game.Editor
{
    [InitializeOnLoad]
    internal sealed class LauncherSceneToolBar
    {
        private static readonly GUIContent m_ButtonGUIContent;
        
        static LauncherSceneToolBar()
        {
            m_ButtonGUIContent = new GUIContent("Launcher", "Start Run Launcher Scene.");
            ToolbarExtender.AddLeftToolbarGUI(100, OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            if (GUILayout.Button(m_ButtonGUIContent))
            {
                BuildSceneSetting.AllScenes();
                SceneHelper.StartScene("Assets/Launcher.unity");
            }
        }
    }

    internal static class SceneHelper
    {
        static string sceneToOpen;

        public static void StartScene(string sceneAssetName)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            sceneToOpen = sceneAssetName;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (sceneToOpen == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(sceneToOpen);
                EditorApplication.isPlaying = true;
            }

            sceneToOpen = null;
        }
    }
}