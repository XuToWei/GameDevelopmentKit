using System.IO;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;
using UnityGameFramework.Editor.ResourceTools;

namespace Game.Editor
{
    public class BuildToolEditor: EditorWindow
    {
        private Platform m_Platform;
        
        [MenuItem("Tools/Build Tool")]
        public static void ShowWindow()
        {
            GetWindow<BuildToolEditor>("Build Tool");
        }

        public Platform GetCurPlatform()
        {
#if UNITY_IOS
            return Platform.IOS;
#elif UNITY_ANDROID
            return Platform.Android;
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return Platform.Windows64;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return Platform.MacOS;
#endif
        }

        private void OnEnable()
        {
            m_Platform = GetCurPlatform();
        }

        private void OnGUI()
        {
            GUIStyle titleGUIStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            {
                m_Platform = (Platform)EditorGUILayout.EnumPopup("Platform: ", m_Platform);

                if (GUILayout.Button("Build Pkg"))
                {
                    if (m_Platform != GetCurPlatform())
                    {
                        switch (EditorUtility.DisplayDialogComplex("Warning!",
                                    $"current platform is {GetCurPlatform()}, if change to {m_Platform}, may be take a long time",
                                    "change", "cancel", "no change"))
                        {
                            case 0:
                                break;
                            case 1:
                                return;
                            case 2:
                                m_Platform = GetCurPlatform();
                                break;
                        }
                    }

                    BuildHelper.BuildPkg(m_Platform);
                    ShowNotification($"Build {m_Platform} Pkg Success!");
                }
                GUILayout.Space(20);
                if (GUILayout.Button("Refresh Windows Pkg Resource"))
                {
                    if (Platform.Windows != GetCurPlatform())
                    {
                        switch (EditorUtility.DisplayDialogComplex("Warning!",
                                    $"current platform is {GetCurPlatform()}, if change to {Platform.Windows}, may be take a long time",
                                    "change", "cancel", null))
                        {
                            case 0:
                                break;
                            case 1:
                                return;
                            case 2:
                                return;
                        }
                    }

                    BuildHelper.RefreshWindows64PkgResource();
                    ShowNotification("Build Model Success!");
                }

                if (GUILayout.Button("Open Pkg Folder"))
                {
                    string folderPath = $"{BuildHelper.BuildPkgFolder}/{m_Platform}";
                    if (Directory.Exists(folderPath))
                    {
                        OpenFolder.Execute(folderPath);
                    }
                    else
                    {
                        Debug.LogError($"Open folder fail! {folderPath} not exist!");
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        
        private static void ShowNotification(string tips)
        {
            Debug.Log(tips);
            EditorWindow game = GetWindow(typeof (EditorWindow).Assembly.GetType("UnityEditor.GameView"));
            if (game != null) game.ShowNotification(new GUIContent($"{tips}"));
        }
    }
}