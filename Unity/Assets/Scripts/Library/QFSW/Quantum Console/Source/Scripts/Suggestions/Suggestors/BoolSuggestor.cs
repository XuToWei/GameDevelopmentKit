using System.Collections.Generic;

namespace QFSW.QC.Suggestors
{
    public class BoolSuggestor : BasicCachedQcSuggestor<string>
    {
        private readonly string[] _values =
        {
            "true",
            "false"
        };

        protected override bool CanProvideSuggestions(SuggestionContext context, SuggestorOptions options)
        {
            return context.TargetType == typeof(bool);
        }

        protected override IQcSuggestion ItemToSuggestion(string value)
        {
            return new RawSuggestion(value);
        }

        protected override IEnumerable<string> GetItems(SuggestionContext context, SuggestorOptions options)
        {
            return _values;
        }
    }
}