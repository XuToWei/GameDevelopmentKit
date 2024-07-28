using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using ThunderFireUITool;
using System;
using System.Text.RegularExpressions;

[CustomEditor(typeof(UIGradientAsset))]
public class UIGradientAssetEditor : Editor
{
    // Start is called before the first frame update
    ReorderableList reorderableList;
    int activeIndex;
    Gradient activeGradient;
    PropertyInfo propertyInfo;
    MethodInfo methodInfo;

    private readonly List<int> _invalidAssetIndex = new List<int>();

    void OnEnable()
    {
        if (target == null) return;

        propertyInfo = typeof(SerializedProperty).GetProperty("gradientValue",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        methodInfo = Utils.GetEditorMethod(Type.GetType("UnityEditor.SerializedProperty,UnityEditor"), "SetGradientValueInternal");
        activeIndex = -1;
        SerializedProperty prop = serializedObject.FindProperty("defList");
        reorderableList = new ReorderableList(serializedObject, prop, true, true, true, true);

        
        reorderableList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) =>
        {
            GUI.backgroundColor = _invalidAssetIndex.Contains(index) ? new Color(1f,0.2f,0.2f) : Color.white;
        };
        
        reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            //Debug.Log("Draw"+index+" "+activeIndex);
            if (index == activeIndex)
            {
                var element = prop.GetArrayElementAtIndex(index);
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
                var element = prop.GetArrayElementAtIndex(index);
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
        reorderableList.onSelectCallback = (ReorderableList l) =>
        {
            //Debug.Log("Select"+l.index);
            activeIndex = l.index == activeIndex ? -1 : l.index;
            SerializedProperty myprop = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("colorValue");
            activeGradient = GetGradient(myprop);
        };
        reorderableList.onAddCallback = (ReorderableList l) =>
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("ColorDefName").stringValue = "";
            //propertyInfo.GetValue(element.FindPropertyRelative("colorValue")) = new Gradient();
            Gradient gra = new Gradient()
            {
                colorKeys = new GradientColorKey[2]
                {
                    // Add your colour and specify the stop point
                    new GradientColorKey(new Color(0, 0, 0), 0),
                    new GradientColorKey(new Color(1, 1, 1), 1)
                },
                // This sets the alpha to 1 at both ends of the gradient
                alphaKeys = new GradientAlphaKey[2]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                }
            };

            SetGradient(element.FindPropertyRelative("colorValue"), gra);

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

    public Gradient GetGradient(SerializedProperty gradientProperty)
    {
        //System.Reflection.PropertyInfo propertyInfo = typeof(SerializedProperty).GetProperty("gradientValue", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (propertyInfo == null)
        {
            return null;
        }
        else
        {
            return propertyInfo.GetValue(gradientProperty, null) as Gradient;
        }
    }

    public void SetGradient(SerializedProperty prop, Gradient val)
    {
        if (methodInfo == null)
        {
            Debug.Log("111");
            return;
        }
        else methodInfo.Invoke(prop, new object[] { val });
    }

    public Gradient GetCurrentGradient()
    {
        return activeGradient;
    }

    private void CheckNameValid()
    {
        var recordNames = new Dictionary<string, int>();
        var uiGradientAsset = (UIGradientAsset)target;
        _invalidAssetIndex.Clear();

        for (var i = 0; i < uiGradientAsset.defList.Count; i++)
        {
            var colorName = uiGradientAsset.defList[i].ColorDefName;
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