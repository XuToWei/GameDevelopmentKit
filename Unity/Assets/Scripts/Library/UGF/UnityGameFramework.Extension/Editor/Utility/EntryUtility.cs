using System;
using System.Collections.Generic;
using GameFramework.Resource;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityGameFramework.Extension.Editor
{
    public static class EntryUtility
    {
        public static readonly string EntryScenePath = "Assets/Launcher.unity";
        
        public static Scene GetEntryScene()
        {
            EditorSceneManager.OpenScene(EntryScenePath);
            return SceneManager.GetSceneByPath(EntryScenePath);
        }

        public static T GetEntrySceneComponent<T>() where T : Component
        {
            Scene entryScene = GetEntryScene();
            foreach (var rootGameObject in entryScene.GetRootGameObjects())
            {
                T component = rootGameObject.GetComponentInChildren<T>();
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }
        
        public static T[] GetEntrySceneComponents<T>() where T : Component
        {
            Scene entryScene = GetEntryScene();
            List<T> components = new List<T>();
            foreach (var rootGameObject in entryScene.GetRootGameObjects())
            {
                components.AddRange(rootGameObject.GetComponentsInChildren<T>(true));
            }
            return components.ToArray();
        }

        public static ResourceMode GetEntryResourceMode()
        {
            var content = System.IO.File.ReadAllText(EntryScenePath);
            string targetString = "      propertyPath: m_ResourceMode\r\n      value: ";
            int index = content.IndexOf(targetString, StringComparison.Ordinal);
            Debug.Assert(index >= 0);
            index += targetString.Length;
            ResourceMode resourceMode = Enum.Parse<ResourceMode>(content.Substring(index, 1));
            return resourceMode;
        }
    }
}
