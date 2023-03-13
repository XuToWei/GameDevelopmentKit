using System.Collections.Generic;

namespace QFSW.QC
{
    /// <summary>
    /// A suggestor that is loaded by the QuantumSuggestor to suggest IQcSuggestions
    /// </summary>
    public interface IQcSuggestor
    {
        /// <summary>
        /// Gets the suggestions for a given context.
        /// </summary>
        /// <param name="context">The context to provide suggestions for.</param>
        /// <param name="options">Options used by the suggestor.</param>
        /// <returns>The suggestions produced for the context.</returns>
        IEnumerable<IQcSuggestion> GetSuggestions(SuggestionContext context, SuggestorOptions options);
    }
}