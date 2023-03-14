using QFSW.QC.Comparators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QFSW.QC
{
    /// <summary>
    /// Provides a filtered and sorted list of suggestions for a given context using IQcSuggestors and IQcSuggestionFilter 
    /// </summary>
    public class QuantumSuggestor
    {
        private readonly IQcSuggestor[] _suggestors;
        private readonly IQcSuggestionFilter[] _suggestionFilters;
        private readonly List<IQcSuggestion> _suggestionBuffer = new List<IQcSuggestion>();

        /// <summary>
        /// Creates a Quantum Suggestor with a custom set of suggestors an suggestion filters.
        /// </summary>
        /// <param name="suggestors">The IQcSuggestors to use in this Quantum Suggestor.</param>
        /// /// <param name="suggestionFilters">The IQcSuggestionFilters to use in this Quantum Suggestor.</param>
        public QuantumSuggestor(IEnumerable<IQcSuggestor> suggestors, IEnumerable<IQcSuggestionFilter> suggestionFilters)
        {
            _suggestors = suggestors.ToArray();
            _suggestionFilters = suggestionFilters.ToArray();
        }

        /// <summary>
        /// Creates a Quantum Suggestor with the default injected suggestors and suggestion filters.
        /// </summary>
        public QuantumSuggestor() : this(
            new InjectionLoader<IQcSuggestor>().GetInjectedInstances(),
            new InjectionLoader<IQcSuggestionFilter>().GetInjectedInstances())
        {

        }

        /// <summary>
        /// Gets suggestions for a given context.
        /// </summary>
        /// <param name="context">The context to get suggestions for.</param>
        /// <param name="options">Options for the suggestor.</param>
        /// <returns>The sorted and filtered suggestions for the provided context.</returns>
        public IEnumerable<IQcSuggestion> GetSuggestions(SuggestionContext context, SuggestorOptions options)
        {
            // Get and filter suggestions
            IEnumerable<IQcSuggestion> suggestions = 
                _suggestors
                    .SelectMany(x => x.GetSuggestions(context, options))
                    .Where(x => IsSuggestionPermitted(x, context));

            _suggestionBuffer.Clear();
            _suggestionBuffer.AddRange(suggestions);

            // Sort suggestions
            AlphanumComparator comparator = new AlphanumComparator();
            IOrderedEnumerable<IQcSuggestion> sortedSuggestions =
                _suggestionBuffer
                    .OrderBy(x => x.PrimarySignature.Length)
                    .ThenBy(x => x.PrimarySignature, comparator)
                    .ThenBy(x => x.SecondarySignature.Length)
                    .ThenBy(x => x.SecondarySignature, comparator);

            if (options.Fuzzy)
            {
                StringComparison comparisonType = options.CaseSensitive
                    ? StringComparison.CurrentCulture
                    : StringComparison.CurrentCultureIgnoreCase;

                sortedSuggestions = sortedSuggestions
                        .OrderBy(x => x.PrimarySignature.IndexOf(context.Prompt, comparisonType));
            }

            // Return suggestions to user
            return sortedSuggestions;
        }

        private bool IsSuggestionPermitted(IQcSuggestion suggestion, SuggestionContext context)
        {
            // LINQ alternative produces too much garbage
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (IQcSuggestionFilter filter in _suggestionFilters)
            {
                if (!filter.IsSuggestionPermitted(suggestion, context))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
