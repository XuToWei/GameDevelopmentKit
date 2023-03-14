using QFSW.QC.Pooling;
using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QFSW.QC
{
    /// <summary>
    /// Provides the UI and I/O interface for the QuantumConsoleProcessor. Invokes commands on the processor and displays the output.
    /// </summary>
    [DisallowMultipleComponent]
    public class QuantumConsole : MonoBehaviour
    {
        /// <summary>
        /// Singleton reference to the console. Only valid and set if the singleton option is enabled for the console.
        /// </summary>
        public static QuantumConsole Instance { get; private set; }

#pragma warning disable 0414, 0067, 0649
        [SerializeField] private RectTransform _containerRect;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _suggestionPopupRect;
        [SerializeField] private RectTransform _jobCounterRect;
        [SerializeField] private Image[] _panels;

        [SerializeField] private QuantumTheme _theme;
        [SerializeField] private QuantumKeyConfig _keyConfig;
        [SerializeField] private QuantumLocalization _localization;

        public QuantumTheme Theme => _theme;

        public QuantumKeyConfig KeyConfig
        {
            get => _keyConfig;
            set => _keyConfig = value;
        }

        public QuantumLocalization Localization
        {
            get => _localization;
            set => _localization = value;
        }

        [Command("verbose-errors", "If errors caused by the Quantum Console Processor or commands should be logged in verbose mode.", MonoTargetType.Registry)]
        [SerializeField] private bool _verboseErrors = false;

        [Command("verbose-logging", "The minimum log severity required to use verbose logging.", MonoTargetType.Registry)]
        [SerializeField] private LoggingThreshold _verboseLogging = LoggingThreshold.Never;

        [Command("logging-level", "The minimum log severity required to intercept and display the log.", MonoTargetType.Registry)]
        [SerializeField] private LoggingThreshold _loggingLevel = LoggingThreshold.Always;

        [SerializeField] private LoggingThreshold _openOnLogLevel = LoggingThreshold.Never;
        [SerializeField] private bool _interceptDebugLogger = true;
        [SerializeField] private bool _interceptWhilstInactive = true;
        [SerializeField] private bool _prependTimestamps = false;

        [SerializeField] private SupportedState _supportedState = SupportedState.Always;
        [SerializeField] private bool _activateOnStartup = true;
        [SerializeField] private bool _initialiseOnStartup = false;
        [SerializeField] private bool _closeOnSubmit = false;
        [SerializeField] private bool _singletonMode = false;
        [SerializeField] private AutoScrollOptions _autoScroll = AutoScrollOptions.OnInvoke;

        [SerializeField] private bool _enableAutocomplete = true;
        [SerializeField] private bool _showPopupDisplay = true;
        [SerializeField] private SortOrder _suggestionDisplayOrder = SortOrder.Descending;
        [SerializeField] private int _maxSuggestionDisplaySize = -1;
        [SerializeField] private bool _useFuzzySearch = false;
        [SerializeField] private bool _caseSensitiveSearch = true;
        [SerializeField] private bool _collapseSuggestionOverloads = true;

        [SerializeField] private bool _showCurrentJobs = true;
        [SerializeField] private bool _blockOnAsync = false;

        [SerializeField] private bool _storeCommandHistory = true;
        [SerializeField] private bool _storeDuplicateCommands = true;
        [SerializeField] private bool _storeAdjacentDuplicateCommands = false;
        [SerializeField] private int _commandHistorySize = -1;

        [SerializeField] private int _maxStoredLogs = 1024;
        [SerializeField] private int _maxLogSize = 8192;
        [SerializeField] private bool _showInitLogs = true;


        [SerializeField] private TMP_InputField _consoleInput;
        [SerializeField] private TextMeshProUGUI _inputPlaceholderText;
        [SerializeField] private TextMeshProUGUI _consoleLogText;
        [SerializeField] private TextMeshProUGUI _consoleSuggestionText;
        [SerializeField] private TextMeshProUGUI _suggestionPopupText;
        [SerializeField] private TextMeshProUGUI _jobCounterText;
        
        /// <summary>
        /// The maximum number of logs that may be stored in the log storage before old logs are removed.
        /// </summary>
        [Command("max-logs", MonoTargetType.Registry)]
        [CommandDescription("The maximum number of logs that may be stored in the log storage before old logs are removed.")]
        public int MaxStoredLogs
        {
            get => _maxStoredLogs;
            set
            {
                _maxStoredLogs = value;
                if (_logStorage != null) { _logStorage.MaxStoredLogs = value; }
                if (_logQueue != null) { _logQueue.MaxStoredLogs = value; }
            }
        }
#pragma warning restore 0414, 0067, 0649

        #region Callbacks
        /// <summary>Callback executed when the QC state changes.</summary>
        public event Action OnStateChange;

        /// <summary>Callback executed when the QC invokes a command.</summary>
        public event Action<string> OnInvoke;

        /// <summary>Callback executed when the QC is cleared.</summary>
        public event Action OnClear;

        /// <summary>Callback executed when text has been logged to the QC.</summary>
        public event Action<ILog> OnLog;

        /// <summary>Callback executed when the QC is activated.</summary>
        public event Action OnActivate;

        /// <summary>Callback executed when the QC is deactivated.</summary>
        public event Action OnDeactivate;

        /// <summary>Callback executed when a new suggestion set is created. The user may modify the suggestion set.</summary>
        public event Action<SuggestionSet> OnSuggestionSetGenerated;
        #endregion

        private bool IsBlockedByAsync => _blockOnAsync
                                         && _currentTasks.Count > 0
                                         || _currentActions.Count > 0;

        private readonly QuantumSerializer _serializer = new QuantumSerializer();

        private SuggestionStack _suggestionStack;
        private ILogStorage _logStorage;
        private ILogQueue _logQueue;

        public bool IsActive { get; private set; }

        /// <summary>
        /// If any actions are currently executing
        /// </summary>
        public bool AreActionsExecuting => _currentActions.Count > 0;

        private readonly List<string> _previousCommands = new List<string>();
        private readonly List<Task> _currentTasks = new List<Task>();
        private readonly List<IEnumerator<ICommandAction>> _currentActions = new List<IEnumerator<ICommandAction>>();
        private readonly StringBuilderPool _stringBuilderPool = new StringBuilderPool();

        private int _selectedPreviousCommandIndex = -1;
        private string _currentInput;
        private string _previousInput;
        private bool _isGeneratingTable;
        private bool _consoleRequiresFlush;

        private TextMeshProUGUI[] _textComponents;

        private readonly Type _voidTaskType = typeof(Task<>).MakeGenericType(Type.GetType("System.Threading.Tasks.VoidTaskResult"));

        /// <summary>Applies a theme to the Quantum Console.</summary>
        /// <param name="theme">The desired theme to apply.</param>
        public void ApplyTheme(QuantumTheme theme, bool forceRefresh = false)
        {
            _theme = theme;
            if (theme)
            {
                if (_textComponents == null || forceRefresh) { _textComponents = GetComponentsInChildren<TextMeshProUGUI>(true); }
                foreach (TextMeshProUGUI text in _textComponents)
                {
                    if (theme.Font)
                    {
                        text.font = theme.Font;
                    }
                }

                foreach (Image panel in _panels)
                {
                    panel.material = theme.PanelMaterial;
                    panel.color = theme.PanelColor;
                }
            }
        }

        protected virtual void Update()
        {
            if (!IsActive)
            {
                if (_keyConfig.ShowConsoleKey.IsPressed() || _keyConfig.ToggleConsoleVisibilityKey.IsPressed())
                {
                    Activate();
                }
            }
            else
            {
                ProcessAsyncTasks();
                ProcessActions();
                HandleAsyncJobCounter();

                if (_keyConfig.HideConsoleKey.IsPressed() || _keyConfig.ToggleConsoleVisibilityKey.IsPressed())
                {
                    Deactivate();
                    return;
                }

                if (QuantumConsoleProcessor.TableIsGenerating)
                {
                    _consoleInput.interactable = false;
                    string consoleText = $"{_logStorage.GetLogString()}\n{GetTableGenerationText()}".Trim();
                    if (consoleText != _consoleLogText.text)
                    {
                        if (_showInitLogs)
                        {
                            OnStateChange?.Invoke();
                            _consoleLogText.text = consoleText;
                        }
                        if (_inputPlaceholderText) { _inputPlaceholderText.text = _localization.Loading; }
                    }

                    return;
                }
                else if (IsBlockedByAsync)
                {
                    OnStateChange?.Invoke();
                    _consoleInput.interactable = false;
                    if (_inputPlaceholderText) { _inputPlaceholderText.text = _localization.ExecutingAsyncCommand; }
                }
                else if (!_consoleInput.interactable)
                {
                    OnStateChange?.Invoke();
                    _consoleInput.interactable = true;
                    if (_inputPlaceholderText) { _inputPlaceholderText.text = _localization.EnterCommand; }
                    OverrideConsoleInput(string.Empty);

                    if (_isGeneratingTable)
                    {
                        if (_showInitLogs)
                        {
                            AppendLog(new Log(GetTableGenerationText()));
                            _consoleLogText.text = _logStorage.GetLogString();
                        }

                        _isGeneratingTable = false;
                        ScrollConsoleToLatest();
                    }
                }

                _previousInput = _currentInput;
                _currentInput = _consoleInput.text;
                if (_currentInput != _previousInput)
                {
                    OnInputChange();
                }

                if (!IsBlockedByAsync)
                {
                    if (InputHelper.GetKeyDown(_keyConfig.SubmitCommandKey)) { InvokeCommand(); }
                    if (_storeCommandHistory) { ProcessCommandHistory(); }
                    ProcessAutocomplete();
                }
            }
        }

        private void LateUpdate()
        {
            if (IsActive)
            {
                FlushQueuedLogs();
                FlushToConsoleText();
            }
        }

        private string GetTableGenerationText()
        {
            string text = string.Format(_localization.InitializationProgress, QuantumConsoleProcessor.LoadedCommandCount);

            if (QuantumConsoleProcessor.TableIsGenerating)
            {
                text += "...";
            }
            else
            {
                string completionText =
                    _theme == null
                        ? _localization.InitializationComplete
                        : _localization.InitializationComplete.ColorText(_theme.SuccessColor);

                text += $"\n{completionText}";
            }

            return text;
        }

        private void ProcessCommandHistory()
        {
            if (InputHelper.GetKeyDown(_keyConfig.NextCommandKey) || InputHelper.GetKeyDown(_keyConfig.PreviousCommandKey))
            {
                if (InputHelper.GetKeyDown(_keyConfig.NextCommandKey)) { _selectedPreviousCommandIndex++; }
                else if (_selectedPreviousCommandIndex > 0) { _selectedPreviousCommandIndex--; }
                _selectedPreviousCommandIndex = Mathf.Clamp(_selectedPreviousCommandIndex, -1, _previousCommands.Count - 1);

                if (_selectedPreviousCommandIndex > -1)
                {
                    string command = _previousCommands[_previousCommands.Count - _selectedPreviousCommandIndex - 1];
                    OverrideConsoleInput(command);
                }
            }
        }

        private void UpdateSuggestions()
        {
            SuggestorOptions options = new SuggestorOptions
            {
                CaseSensitive = _caseSensitiveSearch,
                Fuzzy = _useFuzzySearch,
                CollapseOverloads = _collapseSuggestionOverloads,
            };

            _suggestionStack.UpdateStack(_currentInput, options);

            UpdateSuggestionText();
            if (_showPopupDisplay)
            {
                UpdatePopupDisplay();
            }
        }

        private void ProcessAutocomplete()
        {
            if (!_enableAutocomplete)
            {
                return;
            }

            if (_keyConfig.SelectNextSuggestionKey.IsPressed() || _keyConfig.SelectPreviousSuggestionKey.IsPressed())
            {
                SuggestionSet set = _suggestionStack.TopmostSuggestionSet;
                if (set != null && set.Suggestions.Count > 0)
                {
                    if (_keyConfig.SelectNextSuggestionKey.IsPressed()) { set.SelectionIndex++; }
                    if (_keyConfig.SelectPreviousSuggestionKey.IsPressed()) { set.SelectionIndex--; }

                    set.SelectionIndex += set.Suggestions.Count;
                    set.SelectionIndex %= set.Suggestions.Count;
                    SetSuggestion(set.SelectionIndex);
                }
            }
        }

        private void FormatSuggestion(IQcSuggestion suggestion, bool selected, StringBuilder buffer)
        {
            if (!_theme)
            {
                buffer.Append(suggestion.FullSignature);
                return;
            }

            Color primaryColor = Color.white;
            Color secondaryColor = _theme.SuggestionColor;
            if (selected)
            {
                primaryColor *= _theme.SelectedSuggestionColor;
                secondaryColor *= _theme.SelectedSuggestionColor;
            }

            buffer.AppendColoredText(suggestion.PrimarySignature, primaryColor);
            buffer.AppendColoredText(suggestion.SecondarySignature, secondaryColor);
        }

        private string GetFormattedSuggestions(SuggestionSet suggestionSet)
        {
            StringBuilder buffer = _stringBuilderPool.GetStringBuilder();
            GetFormattedSuggestions(suggestionSet, buffer);
            return _stringBuilderPool.ReleaseAndToString(buffer);
        }

        private void GetFormattedSuggestions(SuggestionSet suggestionSet, StringBuilder buffer)
        {
            int displaySize = suggestionSet.Suggestions.Count;
            if (_maxSuggestionDisplaySize > 0)
            {
                displaySize = Mathf.Min(displaySize, _maxSuggestionDisplaySize + 1);
            }

            for (int i = 0; i < displaySize; i++)
            {
                if (_maxSuggestionDisplaySize > 0 && i >= _maxSuggestionDisplaySize)
                {
                    const string remainingSuggestion = "...";
                    if (_theme && suggestionSet.SelectionIndex >= _maxSuggestionDisplaySize)
                    {
                        buffer.AppendColoredText(remainingSuggestion, _theme.SelectedSuggestionColor);
                    }
                    else
                    {
                        buffer.Append(remainingSuggestion);
                    }
                }
                else
                {
                    bool selected = i == suggestionSet.SelectionIndex;

                    buffer.Append("<link=");
                    buffer.Append(i);
                    buffer.Append(">");
                    FormatSuggestion(suggestionSet.Suggestions[i], selected, buffer);
                    buffer.AppendLine("</link>");
                }
            }
        }

        private void UpdatePopupDisplay()
        {
            SuggestionSet suggestionSet = _suggestionStack.TopmostSuggestionSet;
            if (suggestionSet == null || suggestionSet.Suggestions.Count == 0)
            {
                ClearPopup();
            }
            else
            {
                if (_suggestionPopupRect && _suggestionPopupText)
                {
                    string formattedSuggestions = GetFormattedSuggestions(suggestionSet);
                    if (_suggestionDisplayOrder == SortOrder.Ascending)
                    {
                        formattedSuggestions = formattedSuggestions.ReverseItems('\n');
                    }

                    _suggestionPopupRect.gameObject.SetActive(true);
                    _suggestionPopupText.text = formattedSuggestions;
                }
            }
        }

        /// <summary>
        /// Sets the selected suggestion on the console.
        /// </summary>
        /// <param name="suggestionIndex">The index of the suggestion to set.</param>
        public void SetSuggestion(int suggestionIndex)
        {
            if (!_suggestionStack.SetSuggestionIndex(suggestionIndex))
            {
                throw new ArgumentException($"Cannot set suggestion to index {suggestionIndex}.");
            }

            OverrideConsoleInput(_suggestionStack.GetCompletion());
            UpdateSuggestionText();
        }

        private void UpdateSuggestionText()
        {
            Color suggestionColor = _theme 
                ? _theme.SuggestionColor 
                : Color.gray;

            StringBuilder buffer = _stringBuilderPool.GetStringBuilder();
            buffer.AppendColoredText(_currentInput, Color.clear);
            buffer.AppendColoredText(_suggestionStack.GetCompletionTail(), suggestionColor);

            _consoleSuggestionText.text = _stringBuilderPool.ReleaseAndToString(buffer);
        }

        /// <summary>
        /// Overrides the console input field.
        /// </summary>
        /// <param name="newInput">The text to override the current input with.</param>
        /// <param name="shouldFocus">If the input field should be automatically focused.</param>
        public void OverrideConsoleInput(string newInput, bool shouldFocus = true)
        {
            _currentInput = newInput;
            _previousInput = newInput;
            _consoleInput.text = newInput;

            if (shouldFocus)
            {
                FocusConsoleInput();
            }

            OnInputChange();
        }

        /// <summary>
        /// Selects and focuses the input field for the console.
        /// </summary>
        public void FocusConsoleInput()
        {
            _consoleInput.Select();
            _consoleInput.caretPosition = _consoleInput.text.Length;
            _consoleInput.selectionAnchorPosition = _consoleInput.text.Length;
            _consoleInput.MoveTextEnd(false);
            _consoleInput.ActivateInputField();
        }

        private void OnInputChange()
        {
            if (_selectedPreviousCommandIndex >= 0 && _currentInput.Trim() !=
                _previousCommands[_previousCommands.Count - _selectedPreviousCommandIndex - 1])
            {
                ClearHistoricalSuggestions();
            }

            if (_enableAutocomplete)
            {
                UpdateSuggestions();
            }
        }

        private void ClearHistoricalSuggestions()
        {
            _selectedPreviousCommandIndex = -1;
        }

        private void ClearSuggestions()
        {
            _suggestionStack.Clear();
            _consoleSuggestionText.text = string.Empty;
        }

        private void ClearPopup()
        {
            if (_suggestionPopupRect) { _suggestionPopupRect.gameObject.SetActive(false); }
            if (_suggestionPopupText) { _suggestionPopupText.text = string.Empty; }
        }

        /// <summary>
        /// Invokes the command currently inputted into the Quantum Console.
        /// </summary>
        public void InvokeCommand()
        {
            if (!string.IsNullOrWhiteSpace(_consoleInput.text))
            {
                string command = _consoleInput.text.Trim();
                InvokeCommand(command);
                OverrideConsoleInput(string.Empty);
                StoreCommand(command);
            }
        }

        protected ILog GenerateCommandLog(string command)
        {
            string format =
                _theme != null
                    ? _theme.CommandLogFormat
                    : "> {0}";

            if (command.Contains("<"))
            {
                // Add no parse as rich tags are possible
                command = $"<noparse>{command}</noparse>";
            }

            string logValue = string.Format(format, command);
            if (_theme)
            {
                logValue = logValue.ColorText(_theme.CommandLogColor);
            }

            return new Log(logValue);
        }

        /// <summary>
        /// Invokes the given command.
        /// </summary>
        /// <param name="command">The command to invoke.</param>
        /// <returns>The return value, if any, of the invoked command.</returns>
        public object InvokeCommand(string command)
        {
            object commandResult = null;
            if (!string.IsNullOrWhiteSpace(command))
            {
                ILog commandLog = GenerateCommandLog(command);
                LogToConsole(commandLog);

                string logTrace = string.Empty;
                try
                {
                    commandResult = QuantumConsoleProcessor.InvokeCommand(command);

                    switch (commandResult)
                    {
                        case Task task: _currentTasks.Add(task); break;
                        case IEnumerator<ICommandAction> action: StartAction(action); break;
                        case IEnumerable<ICommandAction> action: StartAction(action.GetEnumerator()); break;
                        default: logTrace = Serialize(commandResult); break;
                    }
                }
                catch (System.Reflection.TargetInvocationException e) { logTrace = GetInvocationErrorMessage(e.InnerException); }
                catch (Exception e) { logTrace = GetErrorMessage(e); }

                LogToConsole(logTrace);
                OnInvoke?.Invoke(command);

                if (_autoScroll == AutoScrollOptions.OnInvoke) { ScrollConsoleToLatest(); }
                if (_closeOnSubmit) { Deactivate(); }
            }
            else { OverrideConsoleInput(string.Empty); }

            return commandResult;
        }

        [Command("qc-script-extern", "Executes an external source of QC script file, where each line is a separate QC command.", MonoTargetType.Registry, Platform.AllPlatforms ^ Platform.WebGLPlayer)]
        public async Task InvokeExternalCommandsAsync(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string command = await reader.ReadLineAsync();
                    if (InvokeCommand(command) is Task ret)
                    {
                        await ret;
                        ProcessAsyncTasks();
                    }
                }
            }
        }

        /// <summary>
        /// Invokes a sequence of commands, only starting a new command when the previous is complete.
        /// </summary>
        /// <param name="commands">The commands to invoke.</param>
        public async Task InvokeCommandsAsync(IEnumerable<string> commands)
        {
            foreach (string command in commands)
            {
                if (InvokeCommand(command) is Task ret)
                {
                    await ret;
                    ProcessAsyncTasks();
                }
            }
        }

        private string GetErrorMessage(Exception e)
        {
            return GetErrorMessage(e, _localization.ConsoleError);
        }

        private string GetInvocationErrorMessage(Exception e)
        {
            return GetErrorMessage(e, _localization.CommandError);
        }

        private string GetErrorMessage(Exception e, string label)
        {
            string message = _verboseErrors
                ? $"{label} ({e.GetType()}): {e.Message}\n{e.StackTrace}"
                : $"{label}: {e.Message}";

            return _theme
                ? message.ColorText(_theme.ErrorColor)
                : message;
        }

        /// <summary>Thread safe API to format and log text to the Quantum Console.</summary>
        /// <param name="logText">Text to be logged.</param>
        /// <param name="logType">The type of the log to be logged.</param>
        public void LogToConsoleAsync(string logText, LogType logType = LogType.Log)
        {
            if (!string.IsNullOrWhiteSpace(logText))
            {
                Log log = new Log(logText, logType);
                LogToConsoleAsync(log);
            }
        }

        /// <summary>Thread safe API to format and log text to the Quantum Console.</summary>
        /// <param name="log">Log to be logged.</param>
        public void LogToConsoleAsync(ILog log)
        {
            OnLog?.Invoke(log);
            _logQueue.QueueLog(log);
        }

        private void FlushQueuedLogs()
        {
            bool scroll = false;
            bool open = false;

            while (_logQueue.TryDequeue(out ILog log))
            {
                AppendLog(log);
                LoggingThreshold severity = log.Type.ToLoggingThreshold();
                scroll |= _autoScroll == AutoScrollOptions.Always;
                open |= severity <= _openOnLogLevel;
            }

            if (scroll) { ScrollConsoleToLatest(); }
            if (open) { Activate(false); }
        }

        private void ProcessAsyncTasks()
        {
            for (int i = _currentTasks.Count - 1; i >= 0; i--)
            {
                if (_currentTasks[i].IsCompleted)
                {
                    if (_currentTasks[i].IsFaulted)
                    {
                        foreach (Exception e in _currentTasks[i].Exception.InnerExceptions)
                        {
                            string error = GetInvocationErrorMessage(e);
                            LogToConsole(error);
                        }
                    }
                    else
                    {
                        Type taskType = _currentTasks[i].GetType();
                        if (taskType.IsGenericTypeOf(typeof(Task<>)) && !_voidTaskType.IsAssignableFrom(taskType))
                        {
                            System.Reflection.PropertyInfo resultProperty = _currentTasks[i].GetType().GetProperty("Result");
                            object result = resultProperty.GetValue(_currentTasks[i]);
                            string log = _serializer.SerializeFormatted(result, _theme);
                            LogToConsole(log);
                        }
                    }

                    _currentTasks.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Starts executing an action.
        /// </summary>
        /// <param name="action">The action to start.</param>
        public void StartAction(IEnumerator<ICommandAction> action)
        {
            _currentActions.Add(action);
            ProcessActions();
        }

        /// <summary>
        /// Cancels any actions currently executing.
        /// </summary>
        public void CancelAllActions()
        {
            _currentActions.Clear();
        }

        private void ProcessActions()
        {
            if (_keyConfig.CancelActionsKey.IsPressed())
            {
                CancelAllActions();
                return;
            }

            ActionContext context = new ActionContext
            {
                Console = this
            };

            for (int i = _currentActions.Count - 1; i >= 0; i--)
            {
                IEnumerator<ICommandAction> action = _currentActions[i];

                try
                {
                    if (action.Execute(context) != ActionState.Running)
                    {
                        _currentActions.RemoveAt(i);
                    }
                }
                catch (Exception e)
                {
                    _currentActions.RemoveAt(i);
                    string error = GetInvocationErrorMessage(e);
                    LogToConsole(error);
                    break;
                }
            }
        }

        private void HandleAsyncJobCounter()
        {
            if (_showCurrentJobs)
            {
                if (_jobCounterRect && _jobCounterText)
                {
                    if (_currentTasks.Count == 0) { _jobCounterRect.gameObject.SetActive(false); }
                    else
                    {
                        _jobCounterRect.gameObject.SetActive(true);
                        _jobCounterText.text = $"{_currentTasks.Count} job{(_currentTasks.Count == 1 ? "" : "s")} in progress";
                    }
                }
            }
        }

        /// <summary>
        /// Serializes a value using the current serializer and theme.
        /// </summary>
        /// <param name="value">The value to the serialize.</param>
        /// <returns>The serialized value.</returns>
        public string Serialize(object value)
        {
            return _serializer.SerializeFormatted(value, _theme);
        }

        /// <summary>
        /// Logs text to the Quantum Console.
        /// </summary>
        /// <param name="logText">Text to be logged.</param>
        /// <param name="newLine">If a newline should be ins</param>
        public void LogToConsole(string logText, bool newLine = true)
        {
            bool logExists = !string.IsNullOrEmpty(logText);
            if (logExists)
            {
                LogToConsole(new Log(logText, LogType.Log, newLine));
            }
        }

        /// <summary>
        /// Logs text to the Quantum Console.
        /// </summary>
        /// <param name="log">Log to be logged.</param>
        public void LogToConsole(ILog log)
        {
            FlushQueuedLogs();
            AppendLog(log);
            OnLog?.Invoke(log);

            if (_autoScroll == AutoScrollOptions.Always)
            {
                ScrollConsoleToLatest();
            }
        }

        private void FlushToConsoleText()
        {
            if (_consoleRequiresFlush)
            {
                _consoleRequiresFlush = false;
                _consoleLogText.text = _logStorage.GetLogString();
            }
        }

        private ILog TruncateLog(ILog log)
        {
            if (log.Text.Length <= _maxLogSize && _maxLogSize >= 0)
            {
                return log;
            }

            string msg = string.Format(_localization.MaxLogSizeExceeded, log.Text.Length, _maxLogSize);
            if (_theme)
            {
                msg = msg.ColorText(_theme.ErrorColor);
            }

            return new Log(msg, LogType.Error);
        }

        protected void AppendLog(ILog log)
        {
            _logStorage.AddLog(TruncateLog(log));
            RequireFlush();
        }
        
        protected void RequireFlush()
        {
            _consoleRequiresFlush = true;
        }

        /// <summary>
        /// Removes the last log from the console.
        /// </summary>
        public void RemoveLogTrace()
        {
            _logStorage.RemoveLog();
            RequireFlush();
        }

        private void ScrollConsoleToLatest()
        {
            if (_scrollRect)
            {
                _scrollRect.verticalNormalizedPosition = 0;
            }
        }

        private void StoreCommand(string command)
        {
            if (_storeCommandHistory)
            {
                if (!_storeDuplicateCommands) { _previousCommands.Remove(command); }
                if (_storeAdjacentDuplicateCommands || _previousCommands.Count == 0 || _previousCommands[_previousCommands.Count - 1] != command) { _previousCommands.Add(command); }
                if (_commandHistorySize > 0 && _previousCommands.Count > _commandHistorySize) { _previousCommands.RemoveAt(0); }
            }
        }

        /// <summary>
        /// Clears the Quantum Console.
        /// </summary>
        [Command("clear", "Clears the Quantum Console", MonoTargetType.Registry)]
        public void ClearConsole()
        {
            _logStorage.Clear();
            _logQueue.Clear();
            _consoleLogText.text = string.Empty;
            _consoleLogText.SetLayoutDirty();
            ClearBuffers();
            OnClear?.Invoke();
        }

        public string GetConsoleText()
        {
            return _consoleLogText.text;
        }

        protected virtual void ClearBuffers()
        {
            ClearHistoricalSuggestions();
            ClearSuggestions();
            ClearPopup();
        }

        private void Awake()
        {
            InitializeLogging();
        }

        private void OnEnable()
        {
            QuantumRegistry.RegisterObject(this);
            Application.logMessageReceivedThreaded += DebugIntercept;

            if (IsSupportedState())
            {
                if (_singletonMode)
                {
                    if (Instance == null)
                    {
                        Instance = this;
                        DontDestroyOnLoad(gameObject);
                    }
                    else if (Instance != this)
                    {
                        Destroy(gameObject);
                    }
                }

                if (_activateOnStartup)
                {
                    bool shouldFocus = SystemInfo.deviceType == DeviceType.Desktop;
                    Activate(shouldFocus);
                }
                else
                {
                    if (_initialiseOnStartup) { Initialize(); }
                    Deactivate();
                }
            }
            else { DisableQC(); }
        }

        private bool IsSupportedState()
        {
#if QC_DISABLED
            return false;
#endif
            SupportedState currentState = SupportedState.Always;
#if DEVELOPMENT_BUILD
            currentState = SupportedState.Development;
#elif UNITY_EDITOR
            currentState = SupportedState.Editor;
#endif
            return _supportedState <= currentState;
        }

        private void OnDisable()
        {
            QuantumRegistry.DeregisterObject(this);
            Application.logMessageReceivedThreaded -= DebugIntercept;

            Deactivate();
        }

        private void DisableQC()
        {
            Deactivate();
            enabled = false;
        }

        private void Initialize()
        {
            if (!QuantumConsoleProcessor.TableGenerated)
            {
                QuantumConsoleProcessor.GenerateCommandTable(true);
                _consoleInput.interactable = false;
                _isGeneratingTable = true;
            }

            InitializeSuggestionStack();
            InitializeLogging();

            _consoleLogText.richText = true;
            _consoleSuggestionText.richText = true;

            ApplyTheme(_theme);
            if (!_keyConfig) { _keyConfig = ScriptableObject.CreateInstance<QuantumKeyConfig>(); }
            if (!_localization) { _localization = ScriptableObject.CreateInstance<QuantumLocalization>(); }
        }

        private void InitializeSuggestionStack()
        {
            if (_suggestionStack == null)
            {
                _suggestionStack = CreateSuggestionStack();
                _suggestionStack.OnSuggestionSetCreated += OnSuggestionSetGenerated;
            }
        }

        private void InitializeLogging()
        {
            _logStorage = _logStorage ?? CreateLogStorage();
            _logQueue = _logQueue ?? CreateLogQueue();
        }

        protected virtual ILogStorage CreateLogStorage() => new LogStorage(_maxStoredLogs);
        protected virtual ILogQueue CreateLogQueue() => new LogQueue(_maxStoredLogs);
        protected virtual SuggestionStack CreateSuggestionStack() => new SuggestionStack();
        
        /// <summary>
        /// Toggles the Quantum Console.
        /// </summary>
        public void Toggle()
        {
            if (IsActive) { Deactivate(); }
            else { Activate(); }
        }

        /// <summary>
        /// Activates the Quantum Console.
        /// </summary>
        /// <param name="shouldFocus">If the input field should be automatically focused.</param>
        public void Activate(bool shouldFocus = true)
        {
            Initialize();
            IsActive = true;
            _containerRect.gameObject.SetActive(true);
            OverrideConsoleInput(string.Empty, shouldFocus);

            OnActivate?.Invoke();
        }

        /// <summary>
        /// Deactivates the Quantum Console.
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            _containerRect.gameObject.SetActive(false);

            OnDeactivate?.Invoke();
        }

        private void DebugIntercept(string condition, string stackTrace, LogType type)
        {
            if (_interceptDebugLogger && (IsActive || _interceptWhilstInactive) && _loggingLevel >= type.ToLoggingThreshold())
            {
                bool appendStackTrace = _verboseLogging >= type.ToLoggingThreshold();
                ILog log = ConstructDebugLog(condition, stackTrace, type, _prependTimestamps, appendStackTrace);
                LogToConsoleAsync(log);
            }
        }

        protected virtual ILog ConstructDebugLog(string condition, string stackTrace, LogType type, bool prependTimeStamp, bool appendStackTrace)
        {
            if (prependTimeStamp)
            {
                DateTime now = DateTime.Now;
                string format = _theme
                    ? _theme.TimestampFormat
                    : "[{0:00}:{1:00}:{2:00}]";

                condition = $"{string.Format(format, now.Hour, now.Minute, now.Second)} {condition}";
            }

            if (appendStackTrace)
            {
                condition += $"\n{stackTrace}";
            }
           
            if (_theme)
            {
                switch (type)
                {
                    case LogType.Warning:
                    {
                        condition = ColorExtensions.ColorText(condition, _theme.WarningColor);
                        break;
                    }
                    case LogType.Error: 
                    case LogType.Assert:
                    case LogType.Exception:
                    {
                        condition = ColorExtensions.ColorText(condition, _theme.ErrorColor);
                        break;
                    }
                }
            }

            return new Log(condition, type, true);
        }

        protected virtual void OnValidate()
        {
            MaxStoredLogs = _maxStoredLogs;
        }
    }
}
