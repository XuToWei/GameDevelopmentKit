using System.Collections.Generic;

namespace QFSW.QC
{
    /// <summary>
    /// A managed set of suggestions for a given context.
    /// </summary>
    public class SuggestionSet
    {
        /// <summary>
        /// The context this suggestion set was produced for.
        /// </summary>
        public SuggestionContext Context;

        /// <summary>
        /// The index of the current selection in the set.
        /// </summary>
        public int SelectionIndex;

        /// <summary>
        /// The suggestions contained within the set.
        /// </summary>
        public readonly List<IQcSuggestion> Suggestions = new List<IQcSuggestion>();

        /// <summary>
        /// The currently selected suggestion in the set, if any.
        /// </summary>
        public IQcSuggestion CurrentSelection =>
            SelectionIndex >= 0 && SelectionIndex < Suggestions.Count
                ? Suggestions[SelectionIndex]
                : null;
    }
}