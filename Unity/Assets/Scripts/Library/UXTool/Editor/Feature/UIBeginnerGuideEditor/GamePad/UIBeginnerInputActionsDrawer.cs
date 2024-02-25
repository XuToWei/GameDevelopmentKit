using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if USE_InputSystem
using UnityEngine.InputSystem;
#endif
[CustomPropertyDrawer(typeof(InputActions))]
public class UIBeginnerInputActionsDrawer : PropertyDrawer
{
#if USE_InputSystem
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            //设置属性名宽度 Name HP
            EditorGUIUtility.labelWidth = 0f;
            Rect strRect = new Rect(position)
            {
                width = position.width,
                // height = 100,
            };
            //找到每个属性的序列化值
            SerializedProperty strProperty = property.FindPropertyRelative("actions");
            strProperty.objectReferenceValue = EditorGUI.ObjectField(strRect, strProperty.objectReferenceValue, typeof(InputActionReference), false);

        }
    }
#endif
}