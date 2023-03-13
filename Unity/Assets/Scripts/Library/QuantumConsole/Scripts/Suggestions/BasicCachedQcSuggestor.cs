using System.Collections.Generic;
using System.Linq;

namespace QFSW.QC
{
    /// <summary>
    /// An IQcSuggestor that caches the created IQcSuggestion objects
    /// </summary>
    /// <typeparam name="TItem">The item type that suggestions are produced from.</typeparam>
    public abstract class BasicCachedQcSuggestor<TItem> : IQcSuggestor
    {
        private readonly Dictionary<TItem, IQcSuggestion> _suggestionCache = new Dictionary<TItem, IQcSuggestion>();

        /// <summary>
        /// If suggestions can be produced for the provided context.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="options">Options used by the suggestor.</param>
        /// <returns>If suggestions can be produced.</returns>
        protected abstract bool CanProvideSuggestions(SuggestionContext context, SuggestorOptions options);

        /// <summary>
        /// Converts an item to a suggestion.
        /// </summary>
        /// <param name="item">The item to convert to a suggestion.</param>
        /// <returns>The converted suggestion.</returns>
        protected abstract IQcSuggestion ItemToSuggestion(TItem item);

        /// <summary>
        /// Gets the items for a given context.
        /// </summary>
        /// <param name="context">The context to produce items for.</param>
        /// <param name="options">Options used by the suggestor.</param>
        /// <returns>The items produced.</returns>
        protected abstract IEnumerable<TItem> GetItems(SuggestionContext context, SuggestorOptions options);

        /// <summary>
        /// Determines if the provided suggestion matches the provided context.
        /// Override to remove the filtering or add custom filtering.
        /// </summary>
        /// <param name="context">The context to test the suggestion against.</param>
        /// <param name="suggestion">The suggestion to test.</param>
        /// <param name="options">Options used to test the suggestion.</param>
        /// <returns>If the suggestion matches the context.</returns>
        protected virtual bool IsMatch(SuggestionContext context, IQcSuggestion suggestion, SuggestorOptions options)
        {
            return SuggestorUtilities.IsCompatible(context.Prompt, suggestion.PrimarySignature, options);
        }

        public IEnumerable<IQcSuggestion> GetSuggestions(SuggestionContext context, SuggestorOptions options)
        {
            if (!CanProvideSuggestions(context, options))
            {
                return Enumerable.Empty<IQcSuggestion>();
            }

            return GetItems(context, options)
                .Select(ItemToSuggestionCached)
                .Where(suggestion => IsMatch(context, suggestion, options));
        }

        private IQcSuggestion ItemToSuggestionCached(TItem item)
        {
            if (_suggestionCache.TryGetValue(item, out IQcSuggestion suggestion))
            {
                return suggestion;
            }

            return _suggestionCache[item] = ItemToSuggestion(item);
        }
    }
}