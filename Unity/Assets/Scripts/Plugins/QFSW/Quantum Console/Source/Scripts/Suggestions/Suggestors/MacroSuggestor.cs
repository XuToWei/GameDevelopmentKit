using System.Collections.Generic;
using System.Linq;

namespace QFSW.QC.Suggestors
{
    public class MacroSuggestor : BasicCachedQcSuggestor<string>
    {
        protected override bool CanProvideSuggestions(SuggestionContext context, SuggestorOptions options)
        {
            return context.Prompt.StartsWith("#");
        }

        protected override IQcSuggestion ItemToSuggestion(string macro)
        {
            return new RawSuggestion($"#{macro}");
        }

        protected override IEnumerable<string> GetItems(SuggestionContext context, SuggestorOptions options)
        {
            return QuantumMacros.GetMacros()
                .Select(x => x.Key);
        }
    }
}