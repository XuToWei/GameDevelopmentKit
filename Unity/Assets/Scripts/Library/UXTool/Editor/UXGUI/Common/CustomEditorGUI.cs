using System.ComponentModel;
using UnityEngine;

namespace UnityEditor
{
    public sealed class CustomEditorGUI
    {
        public static int propertyInterval = 5;
        public static int listInterval = 10;


        public static bool PropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            return PropertyField(position, property, label, false);
        }

        public static bool PropertyField(Rect position, SerializedProperty property, [DefaultValue("false")] bool includeChildren)
        {
            return PropertyField(position, property, null, includeChildren);
        }

        public static bool PropertyField(Rect position, SerializedProperty property)
        {
            return PropertyField(position, property, false);
        }

        public static bool PropertyField(Rect position, SerializedProperty property, GUIContent label, [DefaultValue("false")] bool includeChildren)
        {
            GUIContent t = GetGUIContent(property);
            return EditorGUI.PropertyField(position, property, t, includeChildren);
        }

        private static GUIContent GetGUIContent(SerializedProperty property)
        {
            string propertyType = property.serializedObject.targetObject.GetType().Name;
            string propertyName = property.name;

            string name = CustomEditorGUILayout.GetL18NTextByTypeAndFieldName(propertyType, propertyName);
            return new GUIContent(name);
        }
    }
}

