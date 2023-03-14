namespace QFSW.QC
{
    /// <summary>
    /// A filter that can remove suggestions produced by the QuantumSuggestor
    /// </summary>
    public interface IQcSuggestionFilter
    {
        /// <summary>
        /// Determines if a suggestion should be permitted given the provided context.
        /// </summary>
        /// <param name="suggestion">The suggestion to query.</param>
        /// <param name="context">The context for the suggestion.</param>
        /// <returns>If the suggestion should be permitted.</returns>
        bool IsSuggestionPermitted(IQcSuggestion suggestion, SuggestionContext context);
    }
}