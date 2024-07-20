using UnityEditor;
using UnityEngine;
using System;

namespace NaughtyAttributes
{
    [CustomPropertyDrawer(typeof(LabelTextAttribute))]
    public class LabelTextAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelTextAttribute LableAttribute = (LabelTextAttribute)attribute;
            EditorGUILayout.PropertyField(property, new GUIContent(LableAttribute.LabelName, LableAttribute.ToolTip));
        }
    }
}
