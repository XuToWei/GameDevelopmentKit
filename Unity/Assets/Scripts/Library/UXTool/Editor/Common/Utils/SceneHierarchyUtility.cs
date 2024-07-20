using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Editor functionalities from internal SceneHierarchyWindow and SceneHierarchy classes. 
/// For that we are using reflection.
/// </summary>
public static class SceneHierarchyUtility
{
    private static PropertyInfo sceneHierarchyInfo;
    private static MethodInfo getExpandedGameObjects;
    private static MethodInfo expandTreeViewItem;
    private static MethodInfo setExpandedRecursive;

    /// <summary>
    /// Check if the target GameObject is expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static bool IsExpanded(GameObject go)
    {
        return GetExpandedGameObjects().Contains(go);
    }

    /// <summary>
    /// Get a list of all GameObjects which are expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static List<GameObject> GetExpandedGameObjects()
    {
        object sceneHierarchy = GetSceneHierarchy();

        if (getExpandedGameObjects == null)
        {
            getExpandedGameObjects = sceneHierarchy
                .GetType()
                .GetMethod("GetExpandedGameObjects");
        }
        object result = getExpandedGameObjects.Invoke(sceneHierarchy, new object[0]);

        return (List<GameObject>)result;
    }

    /// <summary>
    /// Set the target GameObject as expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static void SetExpanded(GameObject go, bool expand)
    {
        object sceneHierarchy = GetSceneHierarchy();

        if (expandTreeViewItem == null)
        {

            expandTreeViewItem = sceneHierarchy
                .GetType()
                .GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        expandTreeViewItem.Invoke(sceneHierarchy, new object[] { go.GetInstanceID(), expand });
    }

    /// <summary>
    /// Set the target GameObject and all children as expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static void SetExpandedRecursive(GameObject go, bool expand)
    {
        object sceneHierarchy = GetSceneHierarchy();
        if (setExpandedRecursive == null)
        {
            setExpandedRecursive = sceneHierarchy
                .GetType()
                .GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);
        }
        setExpandedRecursive.Invoke(sceneHierarchy, new object[] { go.GetInstanceID(), expand });
    }

    private static object GetSceneHierarchy()
    {
        EditorWindow window = GetHierarchyWindow();

        if (sceneHierarchyInfo == null)
        {
            sceneHierarchyInfo = typeof(EditorWindow).Assembly
                .GetType("UnityEditor.SceneHierarchyWindow")
                .GetProperty("sceneHierarchy");
            //.GetValue(window);
        }
        return sceneHierarchyInfo.GetValue(window);
    }

    private static EditorWindow GetHierarchyWindow()
    {
        // For it to open, so that it the current focused window.
        EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        return EditorWindow.focusedWindow;
    }
}
