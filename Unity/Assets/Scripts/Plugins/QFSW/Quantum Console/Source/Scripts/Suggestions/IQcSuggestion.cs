namespace QFSW.QC
{
    /// <summary>
    /// A suggestion that can be auto completed and displayed.
    /// </summary>
    public interface IQcSuggestion
    {
        /// <summary>
        /// The full signature of the suggestion for display purposes.
        /// </summary>
        string FullSignature { get; }

        /// <summary>
        /// The primary component of the signature.
        /// </summary>
        string PrimarySignature { get; }

        /// <summary>
        /// The secondary component of the signature.
        /// </summary>
        string SecondarySignature { get; }

        /// <summary>
        /// Determines if the provided prompt matches this suggestion
        /// </summary>
        /// <param name="prompt">The prompt to check.</param>
        /// <returns>If the prompt matches the suggestion.</returns>
        bool MatchesPrompt(string prompt);

        /// <summary>
        /// Gets the completion value for this a prompt with this suggestion.
        /// </summary>
        /// <param name="prompt">The prompt to complete.</param>
        /// <returns>The completion value.</returns>
        string GetCompletion(string prompt);

        /// <summary>
        /// Gets the completion tail, similar to the secondary signature, for a prompt with this suggestion.
        /// </summary>
        /// <param name="prompt">The prompt to complete the tail for.</param>
        /// <returns>The completion tail.</returns>
        string GetCompletionTail(string prompt);

        /// <summary>
        /// Gets the inner suggestion context for this suggestion, allowing for further suggestions to be provided.
        /// </summary>
        /// <param name="context">The outer suggestion context.</param>
        /// <returns>The inner suggestion context, null if none can be created.</returns>
        SuggestionContext? GetInnerSuggestionContext(SuggestionContext context);
    }
}