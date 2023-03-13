using UnityEditor;
using UnityEngine;

namespace QFSW.QC.Editor
{
    [CustomEditor(typeof(QuantumLocalization), true)]
    public class QuantumLocalizationInspector : QCInspectorBase
    {
        private QuantumLocalization _localizationInstance;

        private SerializedProperty _loadingProperty;
        private SerializedProperty _executingAsyncCommandProperty;
        private SerializedProperty _enterCommandProperty;

        private SerializedProperty _commandErrorProperty;
        private SerializedProperty _consoleErrorProperty;
        private SerializedProperty _maxLogSizeExceededProperty;

        private SerializedProperty _initializationProgressProperty;
        private SerializedProperty _initializationCompleteProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _localizationInstance = (QuantumLocalization)target;

            _loadingProperty = serializedObject.FindProperty("Loading");
            _executingAsyncCommandProperty = serializedObject.FindProperty("ExecutingAsyncCommand");
            _enterCommandProperty = serializedObject.FindProperty("EnterCommand");

            _commandErrorProperty = serializedObject.FindProperty("CommandError");
            _consoleErrorProperty = serializedObject.FindProperty("ConsoleError");
            _maxLogSizeExceededProperty = serializedObject.FindProperty("MaxLogSizeExceeded");

            _initializationProgressProperty = serializedObject.FindProperty("InitializationProgress");
            _initializationCompleteProperty = serializedObject.FindProperty("InitializationComplete");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorHelpers.DrawHeader(Banner);

            EditorGUILayout.LabelField(new GUIContent("Prompts", "Prompts to display in the input field."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_loadingProperty, new GUIContent("Loading", "Prompt during table generation."));
            EditorGUILayout.PropertyField(_executingAsyncCommandProperty, new GUIContent("Executing Async Command", "Prompt during blocking async command execution."));
            EditorGUILayout.PropertyField(_enterCommandProperty, new GUIContent("Enter Command", "Prompt when the console is ready for a command to be entered."));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Errors", "Messages and labels around errors."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_commandErrorProperty, new GUIContent("Command Error", "The label to prefix command errors with."));
            EditorGUILayout.PropertyField(_consoleErrorProperty, new GUIContent("Console Error", "The label to prefix console errors with."));
            EditorGUILayout.PropertyField(_maxLogSizeExceededProperty, new GUIContent("Max Log Size Exceeded Color",
                "The error message for the max log size being exceeded." +
                "\n{0} = Log size" +
                "\n{0} = Max log size"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Initialization", "Localization around the initialization updates of Quantum Console."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_initializationProgressProperty, new GUIContent("Initialization Progress",
                "The initialization progress message." +
                "\n{0} = Loaded command count"));
            EditorGUILayout.PropertyField(_initializationCompleteProperty, new GUIContent("Initialization Complete",
                "The initialization completion message."));
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
