using Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities;
using System;
using UnityEditor;
using UnityEngine;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts.Inspector
{
    /// <summary>
    /// Custom property drawer class for PreprocessorSymbolData objects.
    /// </summary>
    [CustomPropertyDrawer(typeof(PreprocessorSymbolData))]
    public sealed class PreprocessorSymbolDataDrawer : PropertyDrawer
    {

        private static readonly GUIContent cachedContent = new GUIContent(GUIContent.none);

        private static GUIContent Content(string text = "", string tooltip = "")
        {
            cachedContent.text = text;
            cachedContent.tooltip = tooltip;
            return cachedContent;
        }

        private static readonly Color inactiveColor = new Color(0.53f, 0.53f, 0.53f);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var enabledRect = new Rect(position.x, position.y, 20, position.height);
            var appliedRect = new Rect(position.x + 18, position.y, 18, position.height);
            var textRect = new Rect(position.x + 30, position.y, 290, position.height);
            var targetLabelRect = new Rect(position.x + 330, position.y, 45, position.height);
            var targetRect = new Rect(position.x + 375, position.y, position.width - 375, position.height);

            var targetGroup = (FlagsBuildTargetGroup)property.FindPropertyRelative("targetGroup").intValue;
            var color = GUI.contentColor;

            var enabled = property.FindPropertyRelative("enabled").boolValue;
            var symbol = property.FindPropertyRelative("symbol").stringValue;
            var activeAndEnabled = enabled && targetGroup.HasFlag(PreprocessorDefineUtilities.FlagsBuildTargetCache);
            var isDefined = PreprocessorDefineUtilities.IsSymbolDefined(symbol);

            GUI.contentColor = activeAndEnabled ? color : inactiveColor;

            // Draw fields - pass GUIContent.none to each so they are drawn without labels.
            // Draw empty label fields to display tooltips.

            // Draw enabled field
            var enabledProperty = property.FindPropertyRelative("enabled");
            EditorGUI.PropertyField(enabledRect, enabledProperty, GUIContent.none);
            EditorGUI.LabelField(enabledRect, Content(tooltip: enabledProperty.tooltip));

            // Draw symbol * to indicate that changes need to be applied
            DrawChangesIndicationGUI(targetGroup, isDefined, activeAndEnabled, symbol, appliedRect, color);

            // Draw symbol field
            DrawSymbolGUI(property, textRect);

            // Draw build target field
            DrawBuildTargetGUI(property, color, targetLabelRect, targetRect);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private static void DrawSymbolGUI(SerializedProperty property, Rect textRect)
        {
            var symbolProperty = property.FindPropertyRelative("symbol");
            EditorGUI.PropertyField(textRect, symbolProperty, GUIContent.none);
            EditorGUI.LabelField(textRect, Content(tooltip: symbolProperty.tooltip));
        }

        private void DrawBuildTargetGUI(SerializedProperty property, Color color, Rect targetLabelRect, Rect targetRect)
        {
            GUI.contentColor = color;
            var targetGroupProperty = property.FindPropertyRelative("targetGroup");
            EditorGUI.LabelField(targetLabelRect, Content("Target", targetGroupProperty.tooltip));
            EditorGUI.PropertyField(targetRect, targetGroupProperty, GUIContent.none);
            EditorGUI.LabelField(targetRect, Content(tooltip: targetGroupProperty.tooltip));
        }

        /// <summary>
        /// Draws the GUI element that displays if unsaved changes are present.
        /// </summary>
        private void DrawChangesIndicationGUI(FlagsBuildTargetGroup targetGroup, bool isDefined, bool activeAndEnabled, string symbol, Rect appliedRect, Color color)
        {
            if (isDefined && !activeAndEnabled && targetGroup.HasFlag(PreprocessorDefineUtilities.FlagsBuildTargetCache))
            {
                GUI.contentColor = color;
                EditorGUI.LabelField(appliedRect, new GUIContent("*", $"Changes must be applied!"));
                GUI.contentColor = inactiveColor;
                return;
            }

            if (!isDefined && activeAndEnabled && targetGroup.HasFlag(PreprocessorDefineUtilities.FlagsBuildTargetCache))
            {
                EditorGUI.LabelField(appliedRect, new GUIContent("*", $"{symbol} is not Defined! Apply to define the symbol"));
                return;
            }

            EditorGUI.LabelField(appliedRect, GUIContent.none);
        }
    }
}