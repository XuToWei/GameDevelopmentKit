using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace NaughtyAttributes.Editor
{
	[CustomPropertyDrawer(typeof(PropertySpaceAttribute))]
	public class PropertySpacePropertyDrawer : PropertyDrawerBase
	{
		protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(rect, label, property);
            PropertySpaceAttribute attr = (PropertySpaceAttribute)attribute;
            if (attr.SpaceBefore != 0f)
            {
                GUILayout.Space(attr.SpaceBefore);
            }

            EditorGUILayout.PropertyField(property);

            if (attr.SpaceAfter != 0f)
            {
                GUILayout.Space(attr.SpaceAfter);
            }

            EditorGUI.EndProperty();
		}
	}
}