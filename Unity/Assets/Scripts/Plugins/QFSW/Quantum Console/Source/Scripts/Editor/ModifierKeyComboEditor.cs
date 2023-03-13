using QFSW.QC.QGUI;
using UnityEditor;
using UnityEngine;

namespace QFSW.QC.Editor
{
    [CustomPropertyDrawer(typeof(ModifierKeyCombo), true)]
    public class ModifierKeyComboEditor : PropertyDrawer
    {
        private readonly GUIContent _shiftLabel = new GUIContent("shift");
        private readonly GUIContent _altLabel = new GUIContent("alt");
        private readonly GUIContent _ctrlLabel = new GUIContent("ctrl");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LayoutController layout = new LayoutController(position);
            EditorGUI.BeginProperty(layout.CurrentRect, label, property);

            const float boolWidth = 10;
            bool enableState = GUI.enabled;
            float boolLabelWidth = QGUILayout.GetMaxContentSize(EditorStyles.label, _shiftLabel, _altLabel, _ctrlLabel).x;

            SerializedProperty key = property.FindPropertyRelative("Key");
            SerializedProperty ctrl = property.FindPropertyRelative("Ctrl");
            SerializedProperty alt = property.FindPropertyRelative("Alt");
            SerializedProperty shift = property.FindPropertyRelative("Shift");

            GUI.enabled &= ((KeyCode)key.enumValueIndex) != KeyCode.None;
            EditorGUI.LabelField(layout.ReserveHorizontalReversed(boolLabelWidth), _shiftLabel);
            EditorGUI.PropertyField(layout.ReserveHorizontalReversed(boolWidth), shift, GUIContent.none);
            EditorGUI.LabelField(layout.ReserveHorizontalReversed(boolLabelWidth), _altLabel);
            EditorGUI.PropertyField(layout.ReserveHorizontalReversed(boolWidth), alt, GUIContent.none);
            EditorGUI.LabelField(layout.ReserveHorizontalReversed(boolLabelWidth), _ctrlLabel);
            EditorGUI.PropertyField(layout.ReserveHorizontalReversed(boolWidth), ctrl, GUIContent.none);

            GUI.enabled = enableState;
            EditorGUI.PropertyField(layout.CurrentRect, key, label);

            EditorGUI.EndProperty();
        }
    }
}