using Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts.Inspector
{
    [CustomEditor(typeof(PreprocessorSymbolDefinitionSettings))]
    public class PreprocessorSymbolDefinitionSettingsInspector : Editor
    {

        private SerializedProperty _securedSymbolProperty;
        private ReorderableList _securedSymbolList;
        private ReorderableList _sdsFileList;

        private static readonly GUIContent checkForSymbolsContent =
            new GUIContent(
                "Check For Elevated Symbols",
                "Scan the project for symbols that should be elevated.");

        private static readonly GUIContent applyUnsavedGUI =
            new GUIContent(
                "Apply Unsaved Changes",
                "Check if unsaved changes are present in any of the listed definition files and apply them.");

        private static readonly GUIContent updateListGUI =
            new GUIContent(
                "Update List",
                "Manually check if any definition file is located in the project that is not listed above.");

        private static readonly GUIContent removeSymbolsOnDeleteContent = new GUIContent(
            "Remove Symbols On Delete",
            "Removes the symbols of a Preprocessor Symbol Definition File when it is deleted. " +
            "If this option is not enabled the symbols of a deleted file will be elevated and must be removed manually");

        private static readonly GUIContent logMessagesContent = new GUIContent(
            "Log Messages",
            "When enabled messages will be logged when symbols are removed, added or elevated.");

        private static readonly GUIContent saveOnCompileContent = new GUIContent(
            "Save And Apply On Load",
            "When enabled unsaved changes will be applied when scripts are recompiling.");

        private static readonly GUIContent showAllDefinedSymbols = new GUIContent(
            "Show All Defined Symbols",
            "When enabled, lists of all defined symbols will be displayed in the inspector of the settings file " +
            "as well as the inspector of Preprocessor Symbol Definition Files.");

        /*
         *  Initialization
         */

        private void OnEnable()
        {
            _securedSymbolProperty = serializedObject.FindProperty("elevatedSymbols");

            _securedSymbolList = new ReorderableList(serializedObject, _securedSymbolProperty, true, true, true, true);
            _securedSymbolList.drawHeaderCallback += rect => DrawHeader(rect, "Elevated Symbols");
            _securedSymbolList.drawElementCallback += DrawSecuredSymbols;

            var list = PreprocessorSymbolDefinitionSettings.ScriptDefineSymbolFiles as IList;
            _sdsFileList = new ReorderableList(list, typeof(PreprocessorSymbolDefinitionFile), false, true, false, false);
            _sdsFileList.drawHeaderCallback += rect => DrawHeader(rect, "Preprocessor Symbol Definition Files");
            _sdsFileList.drawElementCallback += DrawFiles;
        }

        /*
         *  GUI
         */

        private const string InfoMessage =
            "<b>Symbols in this list are considered elevated and will not be handled by definition files. " +
            "If you would like to manage elevated symbols from a definition file, you can do so by removing them from this list " +
            "and adding them to a definition file.</b> Note that active symbols that are not listed in any definition file will " +
            "automatically be elevated. This means, that if you've just installed this asset, any previously active symbol will " +
            "be elevated and must first be removed from this list before it can be handled by a definition file. " +
            "This is especially important if you are working with third party plugins that handle their preprocessor symbols " +
            "independently because it will prevent definition files from accidentally interfering with those symbols.";

        public override void OnInspectorGUI()
        {
            DrawTitle();
            GUIExtensions.DrawGUILine();
            GUIExtensions.DrawGUISpace();

            DrawToggleControls();

            GUIExtensions.DrawGUISpace();

            GUIExtensions.DrawGUIMessage(InfoMessage);
            GUIExtensions.DrawGUISpace(3);
            serializedObject.Update();
            _securedSymbolList.DoLayoutList();

            if (GUILayout.Button(checkForSymbolsContent, GUILayout.Width(200)))
            {
                PreprocessorDefineUtilities.ElevateIndependentSymbols();
            }

            GUIExtensions.DrawGUISpace();

            _sdsFileList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            DrawSaveAll();
            if (PreprocessorSymbolDefinitionSettings.ShowAllDefinedSymbols)
            {
                GUIExtensions.DrawGlobalSymbols();
            }

            EditorUtility.SetDirty(target);
        }

        /*
         *  Individual GUI Elements
         */

        private void DrawFiles(Rect rect, int index, bool isActive, bool isFocused)
        {
            var file = (PreprocessorSymbolDefinitionFile) _sdsFileList.list[index];

            if (file)
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold
                };

                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), new GUIContent($"{file.name}"), style);
                var path = AssetDatabase.GetAssetPath(file).Replace($"{file.name}.asset", "");

                EditorGUI.LabelField(new Rect(rect.x + 300, rect.y, rect.width - 370, rect.height), $"Path: {path}");
                if (GUI.Button(new Rect(rect.x + rect.width -60, rect.y, 60, rect.height), "Select"))
                {
                    Selection.activeObject = file;
                }
            }
        }

        private void DrawSaveAll()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(applyUnsavedGUI))
            {
                PreprocessorDefineUtilities.ApplyAndUpdateAllDefinitionFiles();
            }
            if (GUILayout.Button(updateListGUI))
            {
                PreprocessorSymbolDefinitionSettings.FindAllPreprocessorSymbolDefinitionFiles();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void DrawSecuredSymbols(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _securedSymbolList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        private void DrawHeader(Rect rect, string header)
        {
            EditorGUI.LabelField(rect, header);
        }

        private static void DrawToggleControls()
        {

            EditorGUILayout.BeginHorizontal();
            // --- Draw RemoveSymbolsOnDelete Toggle
            PreprocessorSymbolDefinitionSettings.RemoveSymbolsOnDelete
                = EditorGUILayout.ToggleLeft(removeSymbolsOnDeleteContent, PreprocessorSymbolDefinitionSettings.RemoveSymbolsOnDelete);
            GUILayout.FlexibleSpace();

            // --- Draw Build Target
            GUILayout.Label("Current Build Target:");
            GUILayout.Label($"{PreprocessorDefineUtilities.BuildTarget}",
                new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold, fontSize = 13});
            EditorGUILayout.EndHorizontal();

            // --- Draw LogMessages Toggle
            PreprocessorSymbolDefinitionSettings.LogMessages = EditorGUILayout.ToggleLeft(
                logMessagesContent,
                PreprocessorSymbolDefinitionSettings.LogMessages);

            // --- Draw ShowAllDefinedSymbols Toggle
            PreprocessorSymbolDefinitionSettings.ShowAllDefinedSymbols = EditorGUILayout.ToggleLeft(
                showAllDefinedSymbols,
                PreprocessorSymbolDefinitionSettings.ShowAllDefinedSymbols);
        }

        private void DrawTitle()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preprocessor Symbol Settings", new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            }, GUILayout.Height(25),
                 GUILayout.MinWidth(250));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Player Settings"))
            {
                SettingsService.OpenProjectSettings("Project/Player");
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
