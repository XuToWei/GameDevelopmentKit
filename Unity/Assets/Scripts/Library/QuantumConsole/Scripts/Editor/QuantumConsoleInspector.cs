using QFSW.QC.Editor.Tools;
using QFSW.QC.QGUI;
using UnityEditor;
using UnityEngine;

namespace QFSW.QC.Editor
{
    [CustomEditor(typeof(QuantumConsole), true)]
    public class QuantumConsoleInspector : QCInspectorBase
    {
        private QuantumConsole QCInstance;

        private SerializedProperty _themeProperty;
        private SerializedProperty _keyConfigProperty;
        private SerializedProperty _localizationProperty;

        private SerializedProperty _verboseLoggingProperty;
        private SerializedProperty _verboseErrorsProperty;
        private SerializedProperty _loggingLevelProperty;

        private SerializedProperty _openOnLogLevelProperty;
        private SerializedProperty _supportedStateProperty;
        private SerializedProperty _autoScrollProperty;
        private SerializedProperty _interceptDebugProperty;
        private SerializedProperty _interceptInactiveProperty;
        private SerializedProperty _prependTimestampsProperty;
        private SerializedProperty _activateOnStartupProperty;
        private SerializedProperty _initialiseOnStartupProperty;
        private SerializedProperty _closeOnSubmitProperty;
        private SerializedProperty _singletonModeProperty;
        private SerializedProperty _inputProperty;
        private SerializedProperty _inputPlaceholderProperty;
        private SerializedProperty _logProperty;
        private SerializedProperty _containerProperty;
        private SerializedProperty _scrollRectProperty;
        private SerializedProperty _suggestionProperty;
        private SerializedProperty _popupProperty;
        private SerializedProperty _popupTextProperty;
        private SerializedProperty _jobCounterTextProperty;
        private SerializedProperty _jobCounterRectProperty;
        private SerializedProperty _panelsProperty;

        private SerializedProperty _commandHistoryProperty;
        private SerializedProperty _commandHistorySizeProperty;
        private SerializedProperty _commandHistoryDuplicatesProperty;
        private SerializedProperty _commandHistoryAdjacentDuplicatesProperty;

        private SerializedProperty _enableAutocompleteProperty;
        private SerializedProperty _usePopupProperty;
        private SerializedProperty _maxSuggestionProperty;
        private SerializedProperty _popupOrderProperty;
        private SerializedProperty _fuzzyProperty;
        private SerializedProperty _caseSensitiveProperty;
        private SerializedProperty _collapseSuggestionOverloadsProperty;

        private SerializedProperty _showCurrentJobsProperty;
        private SerializedProperty _blockOnAsyncProperty;
        private SerializedProperty _maxStoredLogsProperty;
        private SerializedProperty _maxLogSizeProperty;
        private SerializedProperty _showInitLogsProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            QCInstance = (QuantumConsole)target;
            QCInstance.OnStateChange += Repaint;

            FetchSerializedProperties();
        }

        private void FetchSerializedProperties()
        {
            _themeProperty = serializedObject.FindProperty("_theme");
            _keyConfigProperty = serializedObject.FindProperty("_keyConfig");
            _localizationProperty = serializedObject.FindProperty("_localization");

            _verboseLoggingProperty = serializedObject.FindProperty("_verboseLogging");
            _verboseErrorsProperty = serializedObject.FindProperty("_verboseErrors");
            _loggingLevelProperty = serializedObject.FindProperty("_loggingLevel");
            _openOnLogLevelProperty = serializedObject.FindProperty("_openOnLogLevel");
            _supportedStateProperty = serializedObject.FindProperty("_supportedState");
            _autoScrollProperty = serializedObject.FindProperty("_autoScroll");
            _interceptDebugProperty = serializedObject.FindProperty("_interceptDebugLogger");
            _interceptInactiveProperty = serializedObject.FindProperty("_interceptWhilstInactive");
            _prependTimestampsProperty = serializedObject.FindProperty("_prependTimestamps");
            _activateOnStartupProperty = serializedObject.FindProperty("_activateOnStartup");
            _initialiseOnStartupProperty = serializedObject.FindProperty("_initialiseOnStartup");
            _closeOnSubmitProperty = serializedObject.FindProperty("_closeOnSubmit");
            _singletonModeProperty = serializedObject.FindProperty("_singletonMode");
            _containerProperty = serializedObject.FindProperty("_containerRect");
            _scrollRectProperty = serializedObject.FindProperty("_scrollRect");
            _popupProperty = serializedObject.FindProperty("_suggestionPopupRect");
            _jobCounterRectProperty = serializedObject.FindProperty("_jobCounterRect");
            _panelsProperty = serializedObject.FindProperty("_panels");
            _commandHistoryProperty = serializedObject.FindProperty("_storeCommandHistory");
            _commandHistoryDuplicatesProperty = serializedObject.FindProperty("_storeDuplicateCommands");
            _commandHistoryAdjacentDuplicatesProperty = serializedObject.FindProperty("_storeAdjacentDuplicateCommands");
            _commandHistorySizeProperty = serializedObject.FindProperty("_commandHistorySize");
            _showCurrentJobsProperty = serializedObject.FindProperty("_showCurrentJobs");
            _blockOnAsyncProperty = serializedObject.FindProperty("_blockOnAsync");
            _enableAutocompleteProperty = serializedObject.FindProperty("_enableAutocomplete");
            _usePopupProperty = serializedObject.FindProperty("_showPopupDisplay");
            _maxSuggestionProperty = serializedObject.FindProperty("_maxSuggestionDisplaySize");
            _popupOrderProperty = serializedObject.FindProperty("_suggestionDisplayOrder");
            _fuzzyProperty = serializedObject.FindProperty("_useFuzzySearch");
            _caseSensitiveProperty = serializedObject.FindProperty("_caseSensitiveSearch");
            _collapseSuggestionOverloadsProperty = serializedObject.FindProperty("_collapseSuggestionOverloads");
            _maxStoredLogsProperty = serializedObject.FindProperty("_maxStoredLogs");
            _maxLogSizeProperty = serializedObject.FindProperty("_maxLogSize");
            _showInitLogsProperty = serializedObject.FindProperty("_showInitLogs");

            _inputProperty = serializedObject.FindProperty("_consoleInput");
            _inputPlaceholderProperty = serializedObject.FindProperty("_inputPlaceholderText");
            _logProperty = serializedObject.FindProperty("_consoleLogText");
            _suggestionProperty = serializedObject.FindProperty("_consoleSuggestionText");
            _popupTextProperty = serializedObject.FindProperty("_suggestionPopupText");
            _jobCounterTextProperty = serializedObject.FindProperty("_jobCounterText");
        }

        private void OnDisable()
        {
            QCInstance.OnStateChange -= Repaint;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorHelpers.DrawHeader(Banner);

            if (QuantumConsoleProcessor.TableGenerated || QuantumConsoleProcessor.TableIsGenerating)
            {
                EditorGUILayout.LabelField("Quantum Console Processor Information", EditorStyles.miniBoldLabel);
                if (QuantumConsoleProcessor.TableIsGenerating) { EditorGUILayout.LabelField("Command Table Generating...", EditorStyles.miniLabel); }
                EditorGUILayout.LabelField($"Commands Loaded: {QuantumConsoleProcessor.LoadedCommandCount}", EditorStyles.miniLabel);
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField(new GUIContent("General Settings", "All general and basic settings for the Quantum Console."), EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_themeProperty, new GUIContent("Theme", "QuantumTheme to use for this Quantum Console."));
            if (_themeProperty.objectReferenceValue)
            {
                GUIContent applyBtnContent = new GUIContent("Apply", "Forces an application of the theme now allowing you to see any GUI changes it would make");
                if (QGUILayout.ButtonAuto(applyBtnContent, EditorStyles.miniButton))
                {
                    Undo.RecordObject(QCInstance, "Applied a theme to the Quantum Console");
                    QCInstance.ApplyTheme((QuantumTheme)_themeProperty.objectReferenceValue, true);
                    PrefabUtil.RecordPrefabInstancePropertyModificationsFullyRecursive(QCInstance.gameObject);
                    EditorUtility.SetDirty(QCInstance);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_keyConfigProperty, new GUIContent("Key Configuration", "Key configuration for the various keyboard shortcuts used by Quantum Console."));
            EditorGUILayout.PropertyField(_localizationProperty, new GUIContent("Localization", "Localization configuration for the various messages displayed by Quantum Console."));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_supportedStateProperty, new GUIContent("Enabled", "On which build/editor states should the console be enabled on"));
            ShowSceneViewToggle();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_activateOnStartupProperty, new GUIContent("Activate on Startup", "If the Quantum Console should be shown and activated on startup."));
            if (!_activateOnStartupProperty.boolValue)
            {
                EditorGUILayout.PropertyField(_initialiseOnStartupProperty, new GUIContent("Initialise on Startup", "If the Quantum Console should be initialised on startup in the background."));
            }

            EditorGUILayout.PropertyField(_closeOnSubmitProperty, new GUIContent("Close on Submit", "If the Quantum Console should be hidden and closed when a command is submitted and invoked."));
            EditorGUILayout.PropertyField(_singletonModeProperty, new GUIContent("Singleton", "Forces the console into singleton mode. " +
                "This means the console will be made scene persistent and will not be destroyed when new scenes are loaded. " +
                "Additionally, only one instance of the console will be allowed to exist, and it will be accessible via QuantumConsole.Instance"));
            EditorGUILayout.PropertyField(_verboseErrorsProperty, new GUIContent("Verbose Errors", "If errors caused by the Quantum Console Processor or commands should be logged in verbose mode."));
            EditorGUILayout.PropertyField(_autoScrollProperty, new GUIContent("Autoscroll", "Determine if and when the console should autoscroll."));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Debug Interception", "All settings relating to the interception of Unity's Debug class."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_interceptDebugProperty, new GUIContent("Intercept Debug Messages", "If the Quantum Console should intercept and display messages from the Unity Debug logging."));
            if (_interceptDebugProperty.boolValue)
            {
                EditorGUILayout.PropertyField(_interceptInactiveProperty, new GUIContent("Intercept Whilst Inactive", "If the Quantum Console should continue to intercept messages whilst inactive."));
                EditorGUILayout.PropertyField(_prependTimestampsProperty, new GUIContent("Enable Timestamps", "If the timestamp of the log message should be prepended."));
                EditorGUILayout.PropertyField(_loggingLevelProperty, new GUIContent("Logging Level", "The minimum log severity required to intercept and display the log."));
                EditorGUILayout.PropertyField(_verboseLoggingProperty, new GUIContent("Verbose Logging", "The minimum log severity required to use verbose logging."));
                EditorGUILayout.PropertyField(_openOnLogLevelProperty, new GUIContent("Open Console", "The minimum log severity required to open the console."));
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Async Settings", "All settings related to async commands."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_showCurrentJobsProperty, new GUIContent("Show Current Jobs", "Shows a popup counter with the currently executing async commands."));
            EditorGUILayout.PropertyField(_blockOnAsyncProperty, new GUIContent("Block on Execute", "Blocks the Quantum Console from being used until the current async command has finished."));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Autocomplete Settings", "Settings relating to autocomplete and suggestions in the console using tab."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_enableAutocompleteProperty, new GUIContent("Enable Autocomplete", "If the suggestion and autocomplete system should be enabled."));
            if (_enableAutocompleteProperty.boolValue)
            {
                EditorGUILayout.PropertyField(_fuzzyProperty, new GUIContent("Use Fuzzy Search", "If fuzzy search is disabled, then your current search must match the beginning of the suggestion to be suggested (foo*). If fuzzy search is enabled, it can be anywhere within the suggestion to be suggested (*foo*)."));
                EditorGUILayout.PropertyField(_caseSensitiveProperty, new GUIContent("Case Sensitive", "If the search should be case sensitive or not."));
                EditorGUILayout.PropertyField(_collapseSuggestionOverloadsProperty, new GUIContent("Collapse Overloads", 
                    "If multiple overloads of the same suggestion should be collapsed into a single suggestion with optional elements where possible." +
                    "\nFor example, the following" +
                    "\ncommand arg0" +
                    "\ncommand arg0 arg1" +
                    "\nWill become" +
                    "\ncommand arg0 [arg1]"));

                EditorGUILayout.PropertyField(_usePopupProperty, new GUIContent("Show Popup Display", "If enabled, a popup display will be shown containing potential auto completions as you type."));
                if (_usePopupProperty.boolValue)
                {
                    EditorGUILayout.PropertyField(_maxSuggestionProperty, new GUIContent("Max Suggestion Count", "The maximum number of suggestions to display in the popup. Set to -1 for unlimited."));
                    EditorGUILayout.PropertyField(_popupOrderProperty, new GUIContent("Suggestion Popup Order", "The sort direction used when displaying suggestions to the popup display."));
                }
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Command History", "Settings relating to storing previous commands so that they can be easily accessed with the arrow keys."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_commandHistoryProperty, new GUIContent("Store Previous Commands", "If previous commands should be stored, allowing them to be accessed with the arrow keys."));
            if (_commandHistoryProperty.boolValue)
            {
                EditorGUILayout.PropertyField(_commandHistoryDuplicatesProperty, new GUIContent("Allow Duplicates", "Store commands into the history even if they have already appeared."));
                if (_commandHistoryDuplicatesProperty.boolValue) { EditorGUILayout.PropertyField(_commandHistoryAdjacentDuplicatesProperty, new GUIContent("Allow Adjacent Duplicates", "Store commands in the history even if they are adjacent duplicates (i.e same command multiple times in a row).")); }
                _commandHistorySizeProperty.intValue = Mathf.Max(-1, EditorGUILayout.IntField(new GUIContent("Max Size", "The maximum size of the command history buffer; exceeding this size will cause the oldest commands to be removed to make space. Set to -1 for unlimited."), _commandHistorySizeProperty.intValue));
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Advanced Settings", "Advanced settings such as buffer sizes."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_maxStoredLogsProperty, new GUIContent("Maximum Stored Logs", "Maximum number of logtraces to store before discarding old logs. Set to -1 for unlimited."));
            EditorGUILayout.PropertyField(_maxLogSizeProperty, new GUIContent("Maximum Log Size", "Logs exceeding this size will be discarded and an error will be shown. Set to -1 for no maximum size on a single log."));
            EditorGUILayout.PropertyField(_showInitLogsProperty, new GUIContent("Show Initialization Logs", "Whether the initialization logs should be shown or not."));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("References", "All the references needed by the Quantum Console"), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_containerProperty, new GUIContent("Container Rect", "The top level container rect-transform containing all of the Quantum Console UI elements."));
            EditorGUILayout.PropertyField(_scrollRectProperty, new GUIContent("Scroll Rect", "(Optional) The scroll rect of the console text, required for auto scrolling."));
            EditorGUILayout.PropertyField(_popupProperty, new GUIContent("Suggestion Popup Display", "Top level transform for the suggestion popup display."));
            EditorGUILayout.PropertyField(_jobCounterRectProperty, new GUIContent("Job Counter Display", "Top level transform for the job counter display."));

            EditorGUILayout.PropertyField(_inputProperty, new GUIContent("Console Input Field", "The input field used for interfacing with the Quantum Console."));
            EditorGUILayout.PropertyField(_inputPlaceholderProperty, new GUIContent("Console Input Placeholder", "The placeholder text component for when the input field is not in use."));
            EditorGUILayout.PropertyField(_popupTextProperty, new GUIContent("Suggestion Popup Text", "Text display for the suggestion popup display."));
            EditorGUILayout.PropertyField(_logProperty, new GUIContent("Console Log Display", "The text display used as the log output by the Quantum Console."));
            EditorGUILayout.PropertyField(_suggestionProperty, new GUIContent("Command Suggestion Display", "(optional) If assigned, the Quantum Console will show the paramater signature for suggested commands here."));
            EditorGUILayout.PropertyField(_jobCounterTextProperty, new GUIContent("Job Counter Text", "Text display for the job counter display."));

            EditorGUILayout.PropertyField(_panelsProperty, new GUIContent("UI Panels", "All panels in the UI to control with the Quantum Theme."), true);

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowSceneViewToggle()
        {
            RectTransform consoleContainer = (RectTransform)_containerProperty.objectReferenceValue;
            bool containerFound = consoleContainer;
            bool containerHidden = containerFound ? consoleContainer.gameObject.activeSelf : false;

            GUI.enabled = containerFound;
            GUIContent message = new GUIContent(containerFound ? containerHidden ? "Hide Console" : "Show Console" : "Console Missing");
            if (QGUILayout.ButtonAuto(message, EditorStyles.miniButton)) { consoleContainer.gameObject.SetActive(!consoleContainer.gameObject.activeSelf); }
            GUI.enabled = true;
        }
    }
}
