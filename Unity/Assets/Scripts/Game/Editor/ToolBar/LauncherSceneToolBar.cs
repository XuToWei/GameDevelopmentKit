using ToolbarExtension;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    internal sealed class LauncherSceneToolBar
    {
        private static readonly GUIContent m_ButtonGUIContent = new GUIContent("Launcher", "Start Run Launcher Scene.");

        [Toolbar(OnGUISide.Left, 100)]
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
        static string s_SceneToOpen;

        public static void StartScene(string sceneAssetName)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            s_SceneToOpen = sceneAssetName;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (s_SceneToOpen == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(s_SceneToOpen);
                EditorApplication.isPlaying = true;
            }

            s_SceneToOpen = null;
        }
    }
}