using System;
using ReferenceBindTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    [CustomPropertyDrawer(typeof(SearchableData))]
    public class SearchableDataDrawer : PropertyDrawer
    {
        private int m_IdHash = "SearchableData".GetHashCode();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty selectProperty = property.FindPropertyRelative("m_Select");
            SerializedProperty namesProperty = property.FindPropertyRelative("m_Names");

            int id = GUIUtility.GetControlID(m_IdHash, FocusType.Keyboard, position);

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            GUIContent buttonText;
            // If the enum has changed, a blank entry
            if (selectProperty.intValue < 0 || selectProperty.intValue >= namesProperty.arraySize)
            {
                buttonText = new GUIContent();
            }
            else
            {
                buttonText = new GUIContent(namesProperty.GetArrayElementAtIndex(selectProperty.intValue).stringValue);
            }

            if (DropdownButton(id, position, buttonText))
            {
                Action<int> onSelect = i =>
                {
                    selectProperty.intValue = i;
                    property.serializedObject.ApplyModifiedProperties();
                };
                string[] names = new string[namesProperty.arraySize];

                names = new string[namesProperty.arraySize];
                for (int i = 0; i < namesProperty.arraySize; i++)
                {
                    names[i] = namesProperty.GetArrayElementAtIndex(i).stringValue;
                }

                SearchablePopup.Show(position, names, selectProperty.intValue, onSelect);
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// A custom button drawer that allows for a controlID so that we can
        /// sync the button ID and the label ID to allow for keyboard
        /// navigation like the built-in enum drawers.
        /// </summary>
        private static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();
                        return true;
                    }

                    break;
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id && current.character == '\n')
                    {
                        Event.current.Use();
                        return true;
                    }

                    break;
                case EventType.Repaint:
                    EditorStyles.popup.Draw(position, content, id, false);
                    break;
            }

            return false;
        }
    }
}