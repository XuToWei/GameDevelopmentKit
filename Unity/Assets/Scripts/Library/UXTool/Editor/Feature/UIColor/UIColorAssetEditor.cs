using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ThunderFireUITool;

[CustomEditor(typeof(UIColorAsset))]
public class UIColorAssetEditor : Editor
{
    // Start is called before the first frame update
    ReorderableList reorderableList;
    int activeIndex;
    Color activeColor;

    private readonly List<int> _invalidAssetIndex = new List<int>();

    void OnEnable()
    {
        if (target == null) return;
        activeIndex = -1;
        SerializedProperty prop = serializedObject.FindProperty("defList");
        reorderableList = new ReorderableList(serializedObject, prop, true, true, true, true);

        reorderableList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) =>
        {
            GUI.backgroundColor = _invalidAssetIndex.Contains(index) ? new Color(1f,0.2f,0.2f) : Color.white;
        };

        reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = prop.GetArrayElementAtIndex(index);
            if (index == activeIndex)
            {
                rect.height += 40;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, 20),
                    element.FindPropertyRelative("ColorDefName"), new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色名)));
                GUI.backgroundColor = Color.white; // 必须要加，否则会影响后续元素的背景颜色
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + 20, rect.width, 20),
                    element.FindPropertyRelative("colorValue"), new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色)));
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + 40, rect.width, 20),
                    element.FindPropertyRelative("ColorComment"), new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色备注)));
            }
            else
            {
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, 20),
                    element.FindPropertyRelative("ColorDefName"), new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色名)));
            }
            GUI.backgroundColor = Color.white; // 必须要加，否则会影响后续元素的背景颜色
        };
        reorderableList.elementHeightCallback = (index) =>
        {
            //Debug.Log("ele"+index+""+activeIndex);
            if (index == activeIndex)
            {
                return 60;
            }
            else return 20;
        };
        reorderableList.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, prop.displayName); };
        reorderableList.onSelectCallback = (l) =>
        {
            //Debug.Log("Select"+l.index);
            activeIndex = l.index == activeIndex ? -1 : l.index;
            activeColor = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("colorValue").colorValue;
        };
        reorderableList.onAddCallback = (l) =>
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("ColorDefName").stringValue = "";
            element.FindPropertyRelative("colorValue").colorValue = Color.white;
            element.FindPropertyRelative("ColorComment").stringValue = "";
        };
    }

    public override void OnInspectorGUI()
    {
        CheckNameValid();
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    public Color GetCurrentColor()
    {
        return activeColor;
    }

    private void CheckNameValid()
    {
        var recordNames = new Dictionary<string, int>();
        var uiColorAsset = (UIColorAsset)target;
        _invalidAssetIndex.Clear();

        for (var i = 0; i < uiColorAsset.defList.Count; i++)
        {
            var colorName = uiColorAsset.defList[i].ColorDefName;
            // 是否空值
            if (string.IsNullOrEmpty(colorName))
            {
                _invalidAssetIndex.Add(i);
                continue;
            }

            // 是否重复
            if (recordNames.TryGetValue(colorName, out var recordName))
            {
                _invalidAssetIndex.Add(i);
                _invalidAssetIndex.Add(recordName);
                continue;
            }

            // 是否合法
            if (!Regex.IsMatch(colorName, "^[a-zA-Z0-9_]+$"))
            {
                _invalidAssetIndex.Add(i);
                continue;
            }

            recordNames.Add(colorName, i);
        }
    }
}