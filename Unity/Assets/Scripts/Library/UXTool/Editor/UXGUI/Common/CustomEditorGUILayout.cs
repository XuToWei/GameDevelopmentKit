using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ThunderFireUITool;


namespace UnityEditor
{


    public sealed class CustomEditorGUILayout
    {
        private static bool IsChildrenIncluded(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Vector4:
                    return true;
                default:
                    return false;
            }
        }
        public static bool PropertyField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            return PropertyField(property, label, IsChildrenIncluded(property), options);
        }
        public static bool PropertyField(SerializedProperty property, params GUILayoutOption[] options)
        {
            return PropertyField(property, null, IsChildrenIncluded(property), options);
        }
        
        public static bool PropertyField(SerializedProperty property, bool includeChildren, params GUILayoutOption[] options)
        {
            return PropertyField(property, null, includeChildren, options);
        }

        public static bool PropertyField(SerializedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options)
        {
            GUIContent L18NLabel = GetGUIContent(property);
            return EditorGUILayout.PropertyField(property, L18NLabel, includeChildren, options);
        }

        private static GUIContent GetGUIContent(SerializedProperty property)
        {
            string propertyType = property.serializedObject.targetObject.GetType().Name;
            string propertyName = property.name;

            string name = GetL18NTextByTypeAndFieldName(propertyType, propertyName);
            return new GUIContent(name);
        }

        public static string GetL18NTextByTypeAndFieldName(string type, string fieldName)
        {
            return EditorLocalization.GetLocalization(type, fieldName);
            
        }


    }
}

