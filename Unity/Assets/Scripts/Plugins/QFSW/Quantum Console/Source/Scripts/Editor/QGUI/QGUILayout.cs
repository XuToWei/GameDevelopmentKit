using System;
using UnityEditor;
using UnityEngine;

namespace QFSW.QC.QGUI
{
    public static class QGUILayout
    {
        public static T EnumPopup<T>(T selected, params GUILayoutOption[] options) where T : Enum
        {
            return (T)EditorGUILayout.EnumPopup(selected, options);
        }

        public static T EnumPopup<T>(GUIContent content, T selected, params GUILayoutOption[] options) where T : Enum
        {
            return (T)EditorGUILayout.EnumPopup(content, selected, options);
        }

        public static T EnumFlagsField<T>(GUIContent content, T enumValue, params GUILayoutOption[] options) where T : Enum
        {
            return (T)EditorGUILayout.EnumFlagsField(content, enumValue, options);
        }

        public static bool ButtonAuto(GUIContent content, GUIStyle style)
        {
            Vector2 size = style.CalcSize(content);
            return GUILayout.Button(content, style, GUILayout.Width(size.x));
        }

        public static bool ButtonAuto(LayoutController layout, GUIContent content, GUIStyle style)
        {
            Rect rect = layout.ReserveAuto(content, style);
            return GUI.Button(rect, content, style);
        }

        public static Vector2 GetMaxContentSize(GUIStyle style, params GUIContent[] contents)
        {
            Vector2 maxSize = new Vector2();
            foreach (GUIContent content in contents)
            {
                Vector2 size = style.CalcSize(content);
                maxSize.x = Mathf.Max(maxSize.x, size.x);
                maxSize.y = Mathf.Max(maxSize.y, size.y);
            }

            return maxSize;
        }
    }
}
