using QFSW.QC.Utilities;
using System;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace QFSW.QC.Editor
{
    [CustomEditor(typeof(QuantumTheme), true)]
    public class QuantumThemeInspector : QCInspectorBase
    {
        private QuantumTheme _themeInstance;

        private SerializedProperty _fontProperty;
        private SerializedProperty _panelMaterialProperty;
        private SerializedProperty _panelColorProperty;

        private SerializedProperty _errorColorProperty;
        private SerializedProperty _warningColorProperty;
        private SerializedProperty _successColorProperty;
        private SerializedProperty _selectedSuggestionColorProperty;
        private SerializedProperty _suggestionColorProperty;
        private SerializedProperty _commandLogColorProperty;
        private SerializedProperty _defaultValueColorProperty;

        private SerializedProperty _timestampFormatProperty;
        private SerializedProperty _commandLogFormatProperty;

        private ReorderableList _typeFormattersListDisplay;
        private SerializedProperty _typeFormattersProperty;

        private ReorderableList _collectionFormattersListDisplay;
        private SerializedProperty _collectionFormattersProperty;

        private GUIStyle _centeredMiniLabel;
        private GUIStyle _centeredLabel;
        private GUIStyle _centeredTextField;
        private bool _initialisedStyles;

        protected override void OnEnable()
        {
            base.OnEnable();

            _themeInstance = (QuantumTheme)target;

            _fontProperty = serializedObject.FindProperty("Font");
            _panelMaterialProperty = serializedObject.FindProperty("PanelMaterial");
            _panelColorProperty = serializedObject.FindProperty("PanelColor");

            _defaultValueColorProperty = serializedObject.FindProperty("DefaultReturnValueColor");
            _errorColorProperty = serializedObject.FindProperty("ErrorColor");
            _warningColorProperty = serializedObject.FindProperty("WarningColor");
            _successColorProperty = serializedObject.FindProperty("SuccessColor");
            _selectedSuggestionColorProperty = serializedObject.FindProperty("SelectedSuggestionColor");
            _suggestionColorProperty = serializedObject.FindProperty("SuggestionColor");
            _commandLogColorProperty = serializedObject.FindProperty("CommandLogColor");

            _timestampFormatProperty = serializedObject.FindProperty("TimestampFormat");
            _commandLogFormatProperty = serializedObject.FindProperty("CommandLogFormat");

            _typeFormattersProperty = serializedObject.FindProperty("TypeFormatters");
            _typeFormattersListDisplay = new ReorderableList(serializedObject, _typeFormattersProperty, true, true, true, true);
            _typeFormattersListDisplay.onAddCallback = AppendNewTypeFormatter;
            _typeFormattersListDisplay.drawElementCallback = DrawTypeFormatterInspector;
            _typeFormattersListDisplay.drawHeaderCallback = DrawTypeFormatterListHeader;

            _collectionFormattersProperty = serializedObject.FindProperty("CollectionFormatters");
            _collectionFormattersListDisplay = new ReorderableList(serializedObject, _collectionFormattersProperty, true, true, true, true);
            _collectionFormattersListDisplay.onAddCallback = AppendNewCollectionFormatter;
            _collectionFormattersListDisplay.drawElementCallback = DrawCollectionFormatterInspector;
            _collectionFormattersListDisplay.drawHeaderCallback = DrawCollectionFormatterListHeader;
        }

        private void CreateStyles()
        {
            if (!_initialisedStyles)
            {
                _initialisedStyles = true;

                _centeredMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                _centeredMiniLabel.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.7f, 0.7f, 0.7f, 1) : new Color(0.3f, 0.3f, 0.3f, 1);

                _centeredLabel = new GUIStyle(EditorStyles.label);
                _centeredLabel.alignment = TextAnchor.MiddleCenter;
                _centeredLabel.richText = true;

                _centeredTextField = new GUIStyle(EditorStyles.textField);
                _centeredTextField.alignment = TextAnchor.MiddleCenter;
            }
        }

        public override void OnInspectorGUI()
        {
            CreateStyles();
            serializedObject.Update();
            EditorHelpers.DrawHeader(Banner);

            EditorGUILayout.LabelField(new GUIContent("UI", "Theme customisations for the Quantum Console UI."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_fontProperty, new GUIContent("Font", "The font to be used throughout the Quantum Console."));

            EditorGUILayout.PropertyField(_panelMaterialProperty, new GUIContent("Panel Material", "The material to use in the UI panels. Leave null for default."));
            EditorGUILayout.PropertyField(_panelColorProperty, new GUIContent("Panel Color", "The color to use in the UI panels."));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Colors", "Color customisation for various aspects of the Quantum Console."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_errorColorProperty, new GUIContent("Error Color", "The color to use when formatting errors in the console log."));
            EditorGUILayout.PropertyField(_warningColorProperty, new GUIContent("Warning Color", "The color to use when formatting warnings in the console log."));
            EditorGUILayout.PropertyField(_successColorProperty, new GUIContent("Success Color", "The color to use when formatting successful void commands."));
            EditorGUILayout.PropertyField(_selectedSuggestionColorProperty, new GUIContent("Selected Suggestion Color", "The color to use for the selected suggestion from the suggestion popup display."));
            EditorGUILayout.PropertyField(_suggestionColorProperty, new GUIContent("Suggestion Signature Color", "The color to use when displaying the paramater signature for suggested commands."));
            EditorGUILayout.PropertyField(_commandLogColorProperty, new GUIContent("Command Log Color", "The color to use when displaying logged commands in the console log."));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Formatting", "Control various formatting within Quantum Console."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_timestampFormatProperty, new GUIContent("Timestamp Format",
                "The format to use when generating timestamps." +
                "\n{0} = Hour" +
                "\n{1} = Minute" +
                "\n{2} = Second"));
            EditorGUILayout.PropertyField(_commandLogFormatProperty, new GUIContent("Command Log Format",
                "The format to use when generating command logs." +
                "\n{0} = The invoked command"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Return Value Formatting", "Formatting options for the return serialization"), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_defaultValueColorProperty, new GUIContent("Default Color", "The default color for return values"));
            _typeFormattersListDisplay.DoLayoutList();
            _collectionFormattersListDisplay.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTypeFormatterInspector(Rect drawRect, int index, bool isActive, bool isFocused)
        {
            float nameWidth = 150f;
            drawRect.y += 2;

            SerializedProperty currentTypeFormatter = _typeFormattersListDisplay.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty colorProperty = currentTypeFormatter.FindPropertyRelative("Color");

            string typeName = _themeInstance.TypeFormatters[index].Type.GetDisplayName();
            Rect labelRect = new Rect(drawRect.x, drawRect.y, nameWidth, EditorGUIUtility.singleLineHeight);
            Rect colorRect = new Rect(drawRect.x + nameWidth, drawRect.y, drawRect.width - nameWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, typeName);
            EditorGUI.PropertyField(colorRect, colorProperty, new GUIContent());
        }

        private void DrawTypeFormatterListHeader(Rect drawRect)
        {
            EditorGUI.LabelField(new Rect(drawRect.x, drawRect.y, 150, drawRect.height), new GUIContent("Type Formatters", "The different colors that should be used for formatting different type returns."));
            EditorGUI.LabelField(new Rect((drawRect.x + drawRect.width) / 2 - 40, drawRect.y, 80, drawRect.height), _typeFormattersProperty.arraySize.ToString() + (_typeFormattersProperty.arraySize == 1 ? " Formatter" : " Formatters"), _centeredMiniLabel);
        }

        private void AppendNewTypeFormatter(ReorderableList listTarget)
        {
            Action<string> SubmitCallback = (string data) =>
            {
                Type type = QuantumParser.ParseType(data);
                if (type == null) { throw new ArgumentException($"No type of name '{data}' could be found. Are you missing a namespace?"); }

                Undo.RecordObject(_themeInstance, "Added a new Type Formatter");
                _themeInstance.TypeFormatters.Add(new TypeColorFormatter(type));
                EditorUtility.SetDirty(_themeInstance);
            };

            PopupWindow.Show(new Rect(5, 5, 0, 0), new DataEntryPopup("Type Name", "Create Type Formatter", SubmitCallback));
        }

        private void DrawCollectionFormatterInspector(Rect drawRect, int index, bool isActive, bool isFocused)
        {
            float itemWidth = 40f;
            float dataWidth = 35f;
            float padding = 5f;
            float endPadding = 10f;
            float nameWidth = drawRect.width - (6 * itemWidth + 3 * dataWidth + 5 * padding + endPadding);
            drawRect.y += 2;

            SerializedProperty currentCollectionFormatter = _collectionFormattersListDisplay.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty seperatorProperty = currentCollectionFormatter.FindPropertyRelative("SeperatorString");
            SerializedProperty leftScoperProperty = currentCollectionFormatter.FindPropertyRelative("LeftScoper");
            SerializedProperty rightScoperProperty = currentCollectionFormatter.FindPropertyRelative("RightScoper");

            string typeName = _themeInstance.CollectionFormatters[index].Type.GetDisplayName();
            Rect rect = new Rect(drawRect.x, drawRect.y, nameWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, typeName);
            rect.x += nameWidth + padding;

            Action<SerializedProperty, float> DrawTextField = (SerializedProperty prop, float width) =>
            {
                rect.width = width;
                prop.stringValue = EditorGUI.TextField(rect, new GUIContent(), prop.stringValue, _centeredTextField);
                rect.x += width + padding;
            };

            Action<string, float> DrawLabelField = (string text, float width) =>
            {
                rect.width = width;
                EditorGUI.LabelField(rect, text, _centeredLabel);
                rect.x += width + padding;
            };

            string highlightCol = EditorGUIUtility.isProSkin ? "#1fe035" : "#005209";
            string itemCol = EditorGUIUtility.isProSkin ? "#ff8280" : "#6A0301";
            string example = $"<b><color={highlightCol}>{leftScoperProperty.stringValue}</color></b>" +
                             $"<color={itemCol}>item1</color><b><color={highlightCol}>{seperatorProperty.stringValue}</color></b>" +
                             $"<color={itemCol}>item2</color><b><color={highlightCol}>{rightScoperProperty.stringValue}</color></b>";

            DrawTextField(leftScoperProperty, dataWidth);
            DrawLabelField("item1", itemWidth);
            DrawTextField(seperatorProperty, dataWidth);
            DrawLabelField("item2", itemWidth);
            DrawTextField(rightScoperProperty, dataWidth);
            DrawLabelField("<b>=></b>", itemWidth * 1.5f);
            DrawLabelField(example, itemWidth * 2.5f);
        }

        private void DrawCollectionFormatterListHeader(Rect drawRect)
        {
            EditorGUI.LabelField(new Rect(drawRect.x, drawRect.y, 150, drawRect.height), new GUIContent("Collection Formatters", "The different strings that should be used for seperating and enclosing collections when serialized."));
            EditorGUI.LabelField(new Rect((drawRect.x + drawRect.width) / 2 - 40, drawRect.y, 80, drawRect.height), _collectionFormattersProperty.arraySize.ToString() + (_collectionFormattersProperty.arraySize == 1 ? " Formatter" : " Formatters"), _centeredMiniLabel);
        }

        private void AppendNewCollectionFormatter(ReorderableList listTarget)
        {
            Action<string> SubmitCallback = (string data) =>
            {
                Type type = QuantumParser.ParseType(data);
                if (type == null) { throw new ArgumentException($"No type of name '{data}' could be found. Are you missing a namespace?"); }
                if (!typeof(IEnumerable).IsAssignableFrom(type))
                {
                    throw new ArgumentException("Collection type must implement IEnumerator");
                }

                Undo.RecordObject(_themeInstance, "Added a new Collection Formatter");
                _themeInstance.CollectionFormatters.Add(new CollectionFormatter(type));
                EditorUtility.SetDirty(_themeInstance);
            };

            PopupWindow.Show(new Rect(5, 5, 0, 0), new DataEntryPopup("Collection Type Name", "Create Collection Formatter", SubmitCallback));
        }
    }
}
