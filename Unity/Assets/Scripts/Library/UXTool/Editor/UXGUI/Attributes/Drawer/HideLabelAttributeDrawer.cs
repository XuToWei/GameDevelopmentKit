using UnityEditor;
using UnityEngine;
using System;

namespace NaughtyAttributes
{
    [CustomPropertyDrawer(typeof(HideLabelAttribute))]
    public class HideLabelAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.size = new Vector2(100, position.size.y);
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = EditorGUI.IntField(position, property.intValue);
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = EditorGUI.Toggle(position, property.boolValue);
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = EditorGUI.FloatField(position, property.floatValue);
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = EditorGUI.TextField(position, property.stringValue);
                    break;
            }
        }
    }
}
