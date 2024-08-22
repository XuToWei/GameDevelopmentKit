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
        public static readonly string EntryScenePath = "Assets/Launcher.unity";
        
        public static Scene GetEntryScene()
        {
            EditorSceneManager.OpenScene(EntryScenePath, OpenSceneMode.Additive);
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
            ResourceComponent resourceComponent = GetEntrySceneComponent<ResourceComponent>();
            FieldInfo resourceModeFieldInfo = typeof(ResourceComponent).GetField("m_ResourceMode", BindingFlags.Instance | BindingFlags.NonPublic);
            ResourceMode resourceMode = (ResourceMode)resourceModeFieldInfo.GetValue(resourceComponent);
            return resourceMode;
        }
    }
}
