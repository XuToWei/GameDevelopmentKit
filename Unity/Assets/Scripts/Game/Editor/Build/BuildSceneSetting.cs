using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

namespace Game.Editor
{
    public static class BuildSceneSetting
    {
        private static readonly string[] s_SearchScenePaths = new string[]
        {
            "Assets/Res/Scene"
        };
        
        public static void AllScenes()
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
            scenes.Add(new EditorBuildSettingsScene("Assets/Launcher.unity", true));

            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", s_SearchScenePaths);
            foreach (string sceneGuid in sceneGuids)
            {
                string sceneName = AssetDatabase.GUIDToAssetPath(sceneGuid);
                scenes.Add(new EditorBuildSettingsScene(sceneName, false));
            }
            
            EditorBuildSettings.scenes = scenes.ToArray();

            Debug.Log("Set scenes of build settings to all scenes.");
        }
    }
}
