using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    [CustomPropertyDrawer(typeof(TagColor))]
    public class TagColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                position.height = EditorGUIUtility.singleLineHeight;

                Rect rect = new Rect(position) { };

                //找到每个属性的序列化值
                SerializedProperty colorProperty = property.FindPropertyRelative("Color");
                SerializedProperty nameProperty = property.FindPropertyRelative("Name");
                colorProperty.colorValue = EditorGUI.ColorField(rect, nameProperty.stringValue, colorProperty.colorValue);

            }
        }
    }
}