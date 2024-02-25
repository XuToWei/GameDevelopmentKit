#if UNITY_EDITOR
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;

public class HierarchyManagementSettingWindow : EditorWindow
{
    private Editor settingEditor;

    public static void ShowWindow()
    {
        var window = GetWindow<HierarchyManagementSettingWindow>();
        window.minSize = new Vector2(550, 450);
    }

    private void OnEnable()
    {
        HierarchyManagementSetting setting = HierarchyManagementEvent.hierarchyManagementSetting;
        if (setting == null)
        {
            Debug.Log("Cant find HierarchyManagementEvent");
            return;
        }
        settingEditor = Editor.CreateEditor(setting);
    }

    private void OnDisable()
    {
        DestroyImmediate(settingEditor);
    }

    private void OnGUI()
    {
        if (settingEditor)
        {
            settingEditor.OnInspectorGUI();
        }
    }
}
#endif