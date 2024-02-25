using System.Collections.Generic;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;
// using UnityEngine.TextCore.Text;

[CustomPropertyDrawer(typeof(TimeAndKeys))]
public class UIBeginnerTimeAndKeysDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            //设置属性名宽度 Name HP
            EditorGUIUtility.labelWidth = position.width * 0.1f;

            position.height = EditorGUIUtility.singleLineHeight;

            Rect rect = new Rect(position)
            {
                width = position.width * 0.3f,
            };


            //找到每个属性的序列化值
            SerializedProperty timeProperty = property.FindPropertyRelative("time");
            SerializedProperty keysProperty = property.FindPropertyRelative("keys");

            //绘制time
            //timeProperty.intValue = EditorGUI.IntField(timeRect, timeProperty.displayName, timeProperty.intValue);
            string timeString = EditorLocalization.GetLocalization("UIBeginnerTimeAndKeys", "Time");
            string actionString = EditorLocalization.GetLocalization("UIBeginnerTimeAndKeys", "Actions");
            timeProperty.floatValue = EditorGUI.FloatField(rect, timeString, timeProperty.floatValue);
            rect = new Rect(position)
            {
                x = position.x + rect.width + position.width * 0.05f,
                // y = rect.y + EditorGUIUtility.singleLineHeight + 2, 
                width = position.width * 0.65f,
            };

            //绘制keys
            // keysProperty.stringValue = EditorGUI.TextField(keysRect, keysProperty.displayName, keysProperty.stringValue);
            // EditorGUI.PropertyField(rect, keysProperty, new GUIContent(actionString));
            ReorderableListDrawer.DrawList(keysProperty, rect, actionString);
            

        }
    }
}