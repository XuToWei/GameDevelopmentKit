using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Extension.Editor;

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
            scenes.Add(new EditorBuildSettingsScene(EntryUtility.EntryScenePath, true));

            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", s_SearchScenePaths);
            foreach (string sceneGuid in sceneGuids)
            {
                string sceneName = AssetDatabase.GUIDToAssetPath(sceneGuid);
                scenes.Add(new EditorBuildSettingsScene(sceneName, true));
            }
            
            EditorBuildSettings.scenes = scenes.ToArray();

            Debug.Log("Set scenes of build settings to all scenes.");
        }
    }
}
