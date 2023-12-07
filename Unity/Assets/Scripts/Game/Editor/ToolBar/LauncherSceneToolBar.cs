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
            if (GUILayout.Button(m_ButtonGUIContent))
            {
                BuildSceneSetting.AllScenes();
                SceneHelper.StartScene(EntryUtility.EntryScenePath);
            }
        }
    }

    internal static class SceneHelper
    {
        private const string UnityEditorSceneToOpenKey = "UnityEditorSceneToOpen";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            if (PlayerPrefs.HasKey(UnityEditorSceneToOpenKey))
            {
                string scenePath = PlayerPrefs.GetString(UnityEditorSceneToOpenKey);
                if (!SceneManager.GetActiveScene().path.Equals(scenePath))
                {
                    SceneManager.LoadScene(scenePath);
                }
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            if (PlayerPrefs.HasKey(UnityEditorSceneToOpenKey))
            {
                PlayerPrefs.DeleteKey(UnityEditorSceneToOpenKey);
            }
        }

        public static void StartScene(string scenePathName)
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }
            PlayerPrefs.SetString(UnityEditorSceneToOpenKey, scenePathName);
            EditorApplication.isPlaying = true;
        }
    }
}