using System;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UXBuilderFormSettingStruct))]
public class UXBuilderFormSettingStructDrawer : PropertyDrawer
{
    private SerializedProperty componentTypeProperty;
    private SerializedProperty labelProperty;
    SerializedProperty floatValueProperty;
    SerializedProperty stringValueProperty;
    SerializedProperty boolValueProperty;
    SerializedProperty minValueProperty;
    SerializedProperty maxValueProperty;
    // SerializedProperty optionsProperty;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            EditorGUIUtility.labelWidth = position.width * 0.2f;

            position.height = EditorGUIUtility.singleLineHeight;

            Rect rect = new Rect(position) { };

            componentTypeProperty = property.FindPropertyRelative("componentType");
            labelProperty = property.FindPropertyRelative("label");
            floatValueProperty = property.FindPropertyRelative("floatValue");
            stringValueProperty = property.FindPropertyRelative("stringValue");
            boolValueProperty = property.FindPropertyRelative("boolValue");
            minValueProperty = property.FindPropertyRelative("minValue");
            maxValueProperty = property.FindPropertyRelative("maxValue");
            // optionsProperty = property.FindPropertyRelative("options");

            EditorGUI.PropertyField(rect, componentTypeProperty, new GUIContent("Component"));
            rect = new Rect(position) { y = rect.y + EditorGUIUtility.singleLineHeight + 2 };
            labelProperty.stringValue = EditorGUI.TextField(rect, labelProperty.displayName, labelProperty.stringValue);
            rect = new Rect(position) { y = rect.y + EditorGUIUtility.singleLineHeight + 2 };
            int number = componentTypeProperty.intValue;
            if (number == (int)ComponentType.Input || number == (int)ComponentType.Select || number == (int)ComponentType.PathUpload)
            {
                stringValueProperty.stringValue = EditorGUI.TextField(rect, stringValueProperty.displayName, stringValueProperty.stringValue);
                rect = new Rect(position) { y = rect.y + EditorGUIUtility.singleLineHeight + 2 };
            }
            else if (number == (int)ComponentType.Slider)
            {
                floatValueProperty.floatValue = EditorGUI.FloatField(rect, floatValueProperty.displayName, floatValueProperty.floatValue);
                rect = new Rect(position) { y = rect.y + EditorGUIUtility.singleLineHeight + 2 };
                minValueProperty.floatValue = EditorGUI.FloatField(rect, minValueProperty.displayName, minValueProperty.floatValue);
                rect = new Rect(position) { y = rect.y + EditorGUIUtility.singleLineHeight + 2 };
                if (maxValueProperty.floatValue < minValueProperty.floatValue) maxValueProperty.floatValue = minValueProperty.floatValue;
                double TOLERANCE = 0.00001f;
                if (Math.Abs(maxValueProperty.floatValue - minValueProperty.floatValue) < TOLERANCE)
                    maxValueProperty.floatValue = minValueProperty.floatValue + 100;
                maxValueProperty.floatValue = EditorGUI.FloatField(rect, maxValueProperty.displayName, maxValueProperty.floatValue);
                rect = new Rect(position) { y = rect.y + EditorGUIUtility.singleLineHeight + 2 };
            }
            else if (number == (int)ComponentType.CheckBox)
            {
                EditorGUI.PropertyField(rect, boolValueProperty, new GUIContent(boolValueProperty.displayName));
                rect = new Rect(position) { y = rect.y + EditorGUIUtility.singleLineHeight + 2 };
            }

            // if (number == (int)ComponentType.Select)
            // {
            //     EditorGUI.PropertyField(rect, optionsProperty, new GUIContent(optionsProperty.displayName));
            //     rect = new Rect(position) { y = rect.y + EditorGUIUtility.singleLineHeight + 2 };
            // }

        }
    }

    // public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    // {
    //     Debug.Log(optionsProperty.arraySize);
    //     int number = componentTypeProperty.intValue;
    //     if (number == (int)ComponentType.Select)
    //     {
    //         if (optionsProperty.isExpanded)
    //         {
    //             return 130 + optionsProperty.arraySize * EditorGUIUtility.singleLineHeight;
    //         }
    //         return 85;
    //     }
    //     return 65;
    // }

}
