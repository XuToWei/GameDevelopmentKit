using ToolbarExtension;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Extension.Editor;

namespace Game.Editor
{
    internal sealed class LauncherSceneToolBar
    {
        private static readonly GUIContent m_ButtonGUIContent = new GUIContent("Launcher", "Start Run Launcher Scene.");

        [Toolbar(OnGUISide.Left, 100)]
        static void OnToolbarGUI()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            {
                if (GUILayout.Button(m_ButtonGUIContent))
                {
                    BuildSceneSetting.AllScenes();
                    SceneHelper.StartScene(EntryUtility.EntryScenePath);
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    internal static class SceneHelper
    {
        private const string UnityEditorSceneToOpenKey = "UnityEditorSceneToOpen";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            if (EditorPrefs.HasKey(UnityEditorSceneToOpenKey))
            {
                string scenePath = EditorPrefs.GetString(UnityEditorSceneToOpenKey);
                if (!SceneManager.GetActiveScene().path.Equals(scenePath))
                {
                    SceneManager.LoadScene(scenePath);
                }
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            if (EditorPrefs.HasKey(UnityEditorSceneToOpenKey))
            {
                EditorPrefs.DeleteKey(UnityEditorSceneToOpenKey);
            }
        }

        public static void StartScene(string scenePathName)
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }
            EditorPrefs.SetString(UnityEditorSceneToOpenKey, scenePathName);
            EditorApplication.isPlaying = true;
        }
    }
}