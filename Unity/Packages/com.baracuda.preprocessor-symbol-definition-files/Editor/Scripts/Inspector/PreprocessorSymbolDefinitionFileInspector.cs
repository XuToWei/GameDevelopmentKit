using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts.Inspector
{
    /// <summary>
    /// Editor class drawing a custom inspector for Preprocessor-Symbol-Definition-Files
    /// </summary>
    [CustomEditor(typeof(PreprocessorSymbolDefinitionFile))]
    internal class PreprocessorSymbolDefinitionFileInspector : Editor
    {
        /*
         *  Private Fields
         */

        private PreprocessorSymbolDefinitionFile _targetObject = null;
        private SerializedProperty _serializedProperty;
        private ReorderableList _localCustomList;
        private readonly Stack<string> _checkedSymbols = new Stack<string>();

        /*
         *  Initialization
         */

        private void OnEnable()
        {
            // Cache the target as an PreprocessorSymbolDefinitionFile instance.
            _targetObject = target as PreprocessorSymbolDefinitionFile;

            // Initialize the local reorderable preprocessor define list
            _serializedProperty = serializedObject.FindProperty("scriptSymbolDefinitions");
            _localCustomList = new ReorderableList(serializedObject, _serializedProperty, true, true, true, true);
            _localCustomList.drawElementCallback += DrawListItem;
            _localCustomList.drawHeaderCallback += DrawLocalList;
        }

        private void OnDisable()
        {
            _localCustomList.drawElementCallback -= DrawListItem;
            _localCustomList.drawHeaderCallback -= DrawLocalList;
        }

        /*
         *  GUI
         */

        public override void OnInspectorGUI()
        {
            DrawTitle();
            GUIExtensions.DrawGUILine();
            GUIExtensions.DrawGUISpace();
            DrawToggleButtons();
            GUIExtensions.DrawGUISpace();
            DrawSymbolList();
            DrawWarningMessages();
            GUIExtensions.DrawGUISpace();

            if(PreprocessorSymbolDefinitionSettings.ShowAllDefinedSymbols)
            {
                GUIExtensions.DrawGlobalSymbols();
            }
        }

        /*
         *  Draw List
         */

        private void DrawLocalList(Rect rect)
        {
            DrawListHeader(rect, "Locally Defined Symbols");
        }

        private void DrawListHeader(Rect rect, string header)
        {
            EditorGUI.LabelField(rect, header);
        }

        private void DrawListItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _localCustomList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element);
        }

        /*
         *  Draw Inspector
         */

        /// <summary>
        /// Draw the title and the settings button to enable easy access to the settings file from every definition file.
        /// </summary>
        private void DrawTitle()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preprocessor Symbol Definition File",
                new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16, fontStyle = FontStyle.Bold
                },
                GUILayout.Height(25), GUILayout.MinWidth(300));

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Settings"))
            {
                PreprocessorSymbolDefinitionSettings.Select();
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw the three control buttons that will enable / disable or toggle all symbols.
        /// </summary>
        private void DrawToggleButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Toggle", GUILayout.Width(60)))
            {
                foreach (var localSymbol in _targetObject.LocalSymbols)
                {
                    localSymbol.SetEnabled(!localSymbol.Enabled);
                }
            }

            if (GUILayout.Button("Enable", GUILayout.Width(60)))
            {
                foreach (var localSymbol in _targetObject.LocalSymbols)
                {
                    localSymbol.SetEnabled(true);
                }
            }

            if (GUILayout.Button("Disable", GUILayout.Width(60)))
            {
                foreach (var localSymbol in _targetObject.LocalSymbols)
                {
                    localSymbol.SetEnabled(false);
                }
            }


            GUILayout.FlexibleSpace();
            GUILayout.Label("Current Build Target:");
            GUILayout.Label($"{PreprocessorDefineUtilities.BuildTarget}",
                new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold, fontSize = 13});
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw the list containing the local (file based) preprocessor symbols.
        /// </summary>
        private void DrawSymbolList()
        {
            serializedObject.Update();
            _localCustomList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Apply", GUILayout.Width(100)))
            {
                _targetObject.ApplyPreprocessorDefines();
            }

            EditorGUILayout.EndHorizontal();
        }

        /*
         *  Draw Warning Message
         */

        /// <summary>
        /// Draw the cached warning messages if necessary.
        /// </summary>
        private void DrawWarningMessages()
        {
            // Set every symbol as valid before checking which are not.
            foreach (var symbol in _targetObject.LocalSymbols)
            {
                symbol.IsValid = true;
            }

            // Check for multiple entries in this file and draw warning message if necessary.
            CheckMultipleEntriesLocal();

            // Check for multiple entries in other files and draw warning message if necessary.
            CheckMultipleEntriesGlobal();

            // Check for usage of elevated symbols and draw warning message if necessary.
            CheckForElevatedSymbols();
        }

        /// <summary>
        /// Check for multiple entries in this file and draw warning message if necessary.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckMultipleEntriesLocal()
        {
            foreach (var defineData in _targetObject.LocalSymbols)
            {
                if (_checkedSymbols.Contains(defineData.Symbol))
                {
                    continue;
                }

                if (_targetObject.LocalSymbols.Count(other => defineData.Equals(other)) > 1)
                {
                    defineData.IsValid = false;
                    var messageMultipleInFile =
                        $"<b><color=#FF3333>WARNING!</color> [{defineData.Symbol}]</b> is defined multiple times! " +
                        $"Adding this symbol can lead to accidental loss or unexpected behaviour! " +
                        $"Please ensure that preprocessor symbols are not defined more than once.";
                    GUIExtensions.DrawGUIMessage(messageMultipleInFile);
                }

                _checkedSymbols.Push(defineData.Symbol);
            }

            _checkedSymbols.Clear();
        }

        /// <summary>
        /// Check for multiple entries in other files and draw warning message if necessary.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckMultipleEntriesGlobal()
        {
            foreach (var defineData in _targetObject.LocalSymbols)
            {
                if (_checkedSymbols.Contains(defineData.Symbol))
                {
                    continue;
                }

                if (PreprocessorDefineUtilities.IsSymbolDefinedElsewhere(defineData.Symbol, _targetObject))
                {
                    defineData.IsValid = false;
                    var messageMultipleInFile =
                        $"<b><color=#FF3333>WARNING!</color> [{defineData.Symbol}]</b> is already defined in another file! " +
                        $"Adding this symbol can lead to accidental loss or unexpected behaviour! " +
                        $"Please ensure that preprocessor symbols are not defined more than once.";
                    GUIExtensions.DrawGUIMessage(messageMultipleInFile);
                }

                _checkedSymbols.Push(defineData.Symbol);
            }

            _checkedSymbols.Clear();
        }

        /// <summary>
        /// Check for usage of elevated symbols and draw warning message if necessary.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckForElevatedSymbols()
        {
            foreach (var defineData in _targetObject.LocalSymbols)
            {
                if (_checkedSymbols.Contains(defineData.Symbol))
                {
                    continue;
                }

                if (PreprocessorDefineUtilities.IsSymbolElevated(defineData.Symbol))
                {
                    defineData.IsValid = false;
                    var messageMultipleInFile =
                        $"<b><color=#FF3333>WARNING!</color> [{defineData.Symbol}]</b> has an elevated status! " +
                        "Adding this symbol can lead to accidental loss or unexpected behaviour, if it is used by a third-party plugin! " +
                        "if, however, this symbol was defined by you before this plugin was installed, " +
                        "feel free to remove it from the list of elevated symbols and manage it from this file!";
                    GUIExtensions.DrawGUIMessage(messageMultipleInFile);

                    GUILayout.BeginHorizontal();
                    if(GUILayout.Button(new GUIContent(
                        $"Remove from this file",
                        $"Remove [{defineData.Symbol}] from this file. " +
                        $"Chose this option, if this symbol is handled by a third party plugin."),
                        GUILayout.Height(30), GUILayout.MinWidth(200)))
                    {
                        _targetObject.RemovePreprocessorSymbol(defineData);
                        break; // break because the collection was modified
                    }
                    if(GUILayout.Button(new GUIContent(
                        $"Manage from this file (Caution)",
                        $"Remove [{defineData.Symbol}] from the list of elevated symbols, located in the settings file. " +
                        $"Chose this option, if you want to manage the symbol from this file."),
                        GUILayout.Height(30), GUILayout.MinWidth(200)))
                    {
                        PreprocessorSymbolDefinitionSettings.RemoveElevatedSymbol(defineData.Symbol);
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                _checkedSymbols.Push(defineData.Symbol);
            }

            _checkedSymbols.Clear();
        }
    }
}