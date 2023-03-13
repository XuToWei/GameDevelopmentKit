using QFSW.QC.Pooling;
using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QFSW.QC
{
    /// <summary>
    /// Managed stack of suggestion sets updated from a user prompt.
    /// Each suggestion set represents a layer of suggestions, allowing for nested suggestions.
    /// </summary>
    public class SuggestionStack
    {
        private readonly QuantumSuggestor _suggestor;
        private readonly List<SuggestionSet> _suggestionSets = new List<SuggestionSet>();
        private readonly Pool<SuggestionSet> _setPool = new Pool<SuggestionSet>();
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        /// <summary>
        /// The topmost valid suggestion set in the stack.
        /// </summary>
        public SuggestionSet TopmostSuggestionSet => _suggestionSets.LastOrDefault();

        /// <summary>
        /// The selected suggestion, if any, in the topmost suggestion set.
        /// </summary>
        public IQcSuggestion TopmostSuggestion => TopmostSuggestionSet?.CurrentSelection;

        /// <summary>
        /// Callback invoked when a new suggestion set is created.
        /// </summary>
        public event Action<SuggestionSet> OnSuggestionSetCreated;

        /// <summary>
        /// Creates a SuggestionStack with a default QuantumSuggestor
        /// </summary>
        public SuggestionStack() : this(new QuantumSuggestor())
        {

        }

        /// <summary>
        /// Creates a SuggestionStack with a user provided QuantumSuggestor
        /// </summary>
        /// <param name="suggestor">The QuantumSuggestor to use when creating suggestions for the stack.</param>
        public SuggestionStack(QuantumSuggestor suggestor)
        {
            _suggestor = suggestor;
        }

        /// <summary>
        /// Clears the stack of all suggestions.
        /// </summary>
        public void Clear()
        {
            while (PopSet()) { }
        }

        /// <summary>
        /// Updates the stack to the new prompt.
        /// </summary>
        /// <param name="prompt">The prompt to update the stack with.</param>
        /// <param name="options">Options to provide to the suggestor.</param>
        public void UpdateStack(string prompt, SuggestorOptions options)
        {
            // Clear if empty
            if (string.IsNullOrWhiteSpace(prompt))
            {
                Clear();
                return;
            }

            PropagateContextChanges(prompt);
            PopInvalidLayers();
            BuildInitialLayer(prompt, options);
            BuildNewLayers(options);
        }

        private SuggestionContext? GetInnerSuggestionContext(SuggestionSet set)
        {
            IQcSuggestion currentSuggestion = set.CurrentSelection;
            SuggestionContext currentContext = set.Context;
            return currentSuggestion?.GetInnerSuggestionContext(currentContext);
        }

        private void InvalidateLayersFrom(int index)
        {
            PopSets(_suggestionSets.Count - index);
        }

        private void PropagateContextChanges(string prompt)
        {
            if (_suggestionSets.Count == 0)
            {
                // Nothing to propagate
                return;
            }

            // Initialize initial set with new prompt
            _suggestionSets[0].Context.Prompt = prompt;

            // Propagate context changes
            for (int i = 0; i < _suggestionSets.Count - 1; i++)
            {
                SuggestionSet currentSet = _suggestionSets[i];
                SuggestionContext? newNextContext = GetInnerSuggestionContext(currentSet);

                if (newNextContext != null)
                {
                    // Update context for existing layers
                    SuggestionSet nextSet = _suggestionSets[i + 1];
                    nextSet.Context = newNextContext.Value;
                }
                else
                {
                    // Null context means the layer must be invalidated
                    InvalidateLayersFrom(i + 1);
                }
            }
        }

        private void PopInvalidLayers()
        {
            for (int i = 0; i < _suggestionSets.Count; i++)
            {
                SuggestionSet set = _suggestionSets[i];
                SuggestionContext context = set.Context;
                IQcSuggestion suggestion = set.CurrentSelection;

                if (suggestion == null || !suggestion.MatchesPrompt(context.Prompt))
                {
                    InvalidateLayersFrom(i);
                }
            }
        }
        private void BuildInitialLayer(string prompt, SuggestorOptions options)
        {
            if (_suggestionSets.Count == 0)
            {
                SuggestionContext context = new SuggestionContext
                {
                    Prompt = prompt,
                    Depth = 0,
                    TargetType = null,
                };

                CreateLayer(context, options);
            }
        }

        private void BuildNewLayers(SuggestorOptions options)
        {
            // Create new layers if possible
            if (TopmostSuggestion != null)
            {
                SuggestionSet set = TopmostSuggestionSet;
                SuggestionContext? newNextContext = GetInnerSuggestionContext(set);

                if (newNextContext != null)
                {
                    // Create new layer, then recursively try to build more
                    if (CreateLayer(newNextContext.Value, options))
                    {
                        BuildNewLayers(options);
                    }
                }
            }
        }

        private void TryAutoSelectSuggestion(SuggestionSet set, string prompt)
        {
            if (set.CurrentSelection != null)
            {
                // Don't try to select something if we already have a selection
                return;
            }

            // Try to select the first item if it matches
            IQcSuggestion candidate = set.Suggestions.FirstOrDefault();
            if (candidate != null && candidate.MatchesPrompt(prompt))
            {
                set.SelectionIndex = 0;
            }
        }

        private bool CreateLayer(SuggestionContext context, SuggestorOptions options)
        {
            // Get the suggestions and create the new set
            IEnumerable<IQcSuggestion> suggestions =
                _suggestor.GetSuggestions(context, options);

            SuggestionSet set = PushSet();
            set.Context = context;
            set.Suggestions.AddRange(suggestions);

            // Remove the new layer if its empty
            if (set.Suggestions.Count == 0)
            {
                PopSet();
                return false;
            }

            OnSuggestionSetCreated?.Invoke(set);

            // Try to auto select a suggestion in the set
            TryAutoSelectSuggestion(set, context.Prompt);
            return true;
        }

        /// <summary>
        /// Gets the current completion value of the stack.
        /// </summary>
        /// <returns>The combined completion value of the stack.</returns>
        public string GetCompletion()
        {
            if (_suggestionSets.Count == 0)
            {
                return string.Empty;
            }

            IEnumerable<IQcSuggestion> suggestionChain =
                _suggestionSets
                    .Select(x => x.CurrentSelection)
                    .Where(x => x != null);

            SuggestionContext context = _suggestionSets[0].Context;
            _stringBuilder.Clear();

            foreach (IQcSuggestion suggestion in suggestionChain)
            {
                string part = context.Prompt;
                SuggestionContext? newContext = suggestion.GetInnerSuggestionContext(context);

                if (newContext != null)
                {
                    _stringBuilder.Append(part, 0, part.Length - newContext.Value.Prompt.Length);
                }
                else
                {
                    _stringBuilder.Append(suggestion.GetCompletion(part));
                }
            }

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the current completion tail of the stack.
        /// </summary>
        /// <returns>The combined completion tail of the stack.</returns>
        public string GetCompletionTail()
        {
            _stringBuilder.Clear();
            foreach (SuggestionSet set in _suggestionSets.Reversed())
            {
                SuggestionContext context = set.Context;
                _stringBuilder.Append(set.CurrentSelection?.GetCompletionTail(context.Prompt));
            }

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Attempts to set the selected suggestion in the topmost set.
        /// </summary>
        /// <param name="suggestionIndex">The index of the suggestion in the topmost set to select.</param>
        /// <returns>If the suggestion could be selected.</returns>
        public bool SetSuggestionIndex(int suggestionIndex)
        {
            if (_suggestionSets.Count == 0)
            {
                return false;
            }

            if (suggestionIndex < 0 || suggestionIndex > TopmostSuggestionSet.Suggestions.Count)
            {
                return false;
            }

            TopmostSuggestionSet.SelectionIndex = suggestionIndex;
            TopmostSuggestionSet.Context.Prompt = TopmostSuggestion.PrimarySignature;
            return true;
        }

        private SuggestionSet PushSet()
        {
            SuggestionSet set = _setPool.GetObject();
            set.SelectionIndex = -1;
            set.Suggestions.Clear();

            _suggestionSets.Add(set);
            return set;
        }

        private bool PopSet()
        {
            if (_suggestionSets.Count > 0)
            {
                int removeIndex = _suggestionSets.Count - 1;
                SuggestionSet set = _suggestionSets[removeIndex];
                _suggestionSets.RemoveAt(removeIndex);
                _setPool.Release(set);

                return true;
            }

            return false;
        }

        private bool PopSets(int count)
        {
            bool successful = true;
            while (successful && count-- > 0)
            {
                successful &= PopSet();
            }

            return successful;
        }
    }
}