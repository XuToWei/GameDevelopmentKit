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
                SceneHelper.StartScene("Assets/Launcher.unity");
            }
        }
    }

    internal static class SceneHelper
    {
        static string sceneToOpen;

        public static void StartScene(string sceneName)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            sceneToOpen = sceneName;
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
                // need to get scene via search because the path to the scene
                // file contains the package version so it'll change over time
                string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
                if (guids.Length == 0)
                {
                    Debug.LogWarning("Couldn't find scene file");
                }
                else
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    EditorSceneManager.OpenScene(scenePath);
                    EditorApplication.isPlaying = true;
                }
            }

            sceneToOpen = null;
        }
    }
}