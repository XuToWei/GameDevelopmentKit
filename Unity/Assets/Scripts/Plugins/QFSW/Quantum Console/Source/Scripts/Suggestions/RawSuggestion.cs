namespace QFSW.QC
{
    /// <summary>
    /// Raw suggestion of a given value.
    /// </summary>
    public class RawSuggestion : IQcSuggestion
    {
        private readonly string _value;
        private readonly bool _singleLiteral;
        private readonly string _completion;

        public string FullSignature => _value;
        public string PrimarySignature => _value;
        public string SecondarySignature => string.Empty;

        /// <summary>
        /// Constructs a suggestion from the provided value.
        /// </summary>
        /// <param name="value">The value to suggest.</param>
        /// <param name="singleLiteral">If the value should be treated as a single literal then "" will be used as necessary.</param>
        public RawSuggestion(string value, bool singleLiteral = false)
        {
            _value = value;
            _singleLiteral = singleLiteral;
            _completion = _value;

            if (_completion.CanSplitScoped(' ', '"', '"'))
            {
                _completion = $"\"{_completion}\"";
            }
        }

        public bool MatchesPrompt(string prompt)
        {
            if (_singleLiteral)
            {
                prompt = prompt.Trim('"');
            }

            return prompt == _value;
        }

        public string GetCompletion(string prompt)
        {
            return _completion;
        }

        public string GetCompletionTail(string prompt)
        {
            return string.Empty;
        }

        public SuggestionContext? GetInnerSuggestionContext(SuggestionContext context)
        {
            return null;
        }
    }
}