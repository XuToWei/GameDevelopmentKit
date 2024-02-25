using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

public class ReorderableListDrawer
{
    private static Dictionary<string, ReorderableList> _listDict = new Dictionary<string, ReorderableList>();

    public static float GetRecordListHeaderHeight()
    {
        return EditorGUIUtility.singleLineHeight ;
    }
    public static float GetRecordListFooterHeight()
    {
        return EditorGUIUtility.singleLineHeight;
    }
    public static float GetRecordListEmptyHeight()
    {
        return GetRecordListHeaderHeight() + GetRecordListFooterHeight() + 30;
    }

    private static string GetPropertyKeyName(SerializedProperty property)
    {
        return property.serializedObject.targetObject.GetInstanceID() + "." + property.name+ "." + property.propertyPath;
    }

    private static ReorderableList TryGetPropertyList(SerializedProperty property, string headerName, Func<SerializedProperty, float> getHeight)
    {
        string key = GetPropertyKeyName(property);
        ReorderableList list;
        if (_listDict.ContainsKey(key))
        {
            list = _listDict[key];
        }
        else
        {
            list = new ReorderableList(property.serializedObject, property, true, true, true, true);
            InitRecordListCallback(list, headerName, getHeight);
            _listDict[key] = list;
        }
        return list;
    }
    
    private static void InitRecordListCallback(ReorderableList list, string headerName, Func<SerializedProperty, float> getHeight)
    {
        list.drawHeaderCallback = (Rect rect) =>
        {
            GUI.Label(rect, headerName);
        };
        list.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
        {
            SerializedProperty item = list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, item, new GUIContent(index.ToString()));
        };

        list.elementHeightCallback = index =>
        {
            SerializedProperty item = list.serializedProperty.GetArrayElementAtIndex(index);
            return getHeight(item);
        };
    }
    
    public static void ClearCache()
    {
        _listDict.Clear();
    }
    
    public static void DrawList(SerializedProperty property, Rect listRect)
    {
        string disPlayName = property.displayName;

        Func<SerializedProperty, float> getHeight = (itemProperty) =>
        {
            return EditorGUI.GetPropertyHeight(itemProperty);
        };

        DrawList(property, listRect, disPlayName, getHeight);
    }

    public static void DrawList(SerializedProperty property, Rect listRect, string displayName)
    {
        Func<SerializedProperty, float> getHeight = (itemProperty) =>
        {
            return EditorGUI.GetPropertyHeight(itemProperty);
        };

        DrawList(property, listRect, displayName, getHeight);
    }

    public static void DrawList(SerializedProperty property, Rect listRect, Func<SerializedProperty, float> getHeight)
    {
        string displayName = property.displayName;
        DrawList(property, listRect, displayName, getHeight);
    }
    
    private static void DrawList(SerializedProperty property, Rect listRect, string headerName, Func<SerializedProperty, float> getHeight)
    {
        ReorderableList list = TryGetPropertyList(property, headerName, getHeight);

        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(listRect, property.isExpanded, new GUIContent(headerName));
        EditorGUI.EndFoldoutHeaderGroup();

        if (property.isExpanded)
            list.DoList(listRect);
    }

    public static void DrawListLayout(SerializedProperty property)
    {
        string displayName = property.displayName;
        Func<SerializedProperty, float> getHeight = (itemProperty) =>
        {
            return EditorGUI.GetPropertyHeight(itemProperty);
        };
        DrawListLayout(property, displayName, getHeight);
    }
    
    public static void DrawListLayout(SerializedProperty property, Func<SerializedProperty, float> getHeight)
    {
        string displayName = property.displayName;
        DrawListLayout(property, displayName, getHeight);
    }

    public static void DrawListLayout(SerializedProperty property, string displayName)
    {
        Func<SerializedProperty, float> getHeight = (itemProperty) =>
        {
            return EditorGUI.GetPropertyHeight(itemProperty);
        };
        DrawListLayout(property, displayName, getHeight);
    }

    public static void DrawListLayout(SerializedProperty property, string headerName, Func<SerializedProperty, float> getHeight)
    {
        ReorderableList list = TryGetPropertyList(property, headerName, getHeight);

        Rect listRect = EditorGUILayout.GetControlRect();
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(listRect, property.isExpanded, new GUIContent(property.displayName));
        EditorGUI.EndFoldoutHeaderGroup();

        if (property.isExpanded)
            list.DoLayoutList();
    }
}