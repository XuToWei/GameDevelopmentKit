using System;
using System.Collections.Generic;
using System.Reflection;
using GameFramework.Resource;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension.Editor
{
    public static class EntryUtility
    {
        public static readonly string EntrySceneName = "Assets/Launcher.unity";
        
        public static Scene GetEntryScene()
        {
            EditorSceneManager.OpenScene(EntrySceneName);
            return SceneManager.GetSceneByPath(EntrySceneName);
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
            ResourceComponent resourceComponent = GetEntrySceneComponent<ResourceComponent>();
            Type type = typeof(ResourceComponent);
            FieldInfo fieldInfo = type.GetField("m_ResourceMode", BindingFlags.NonPublic | BindingFlags.Instance);
            ResourceMode resourceMode = (ResourceMode)fieldInfo.GetValue(resourceComponent);
            return resourceMode;
        }
    }
}
