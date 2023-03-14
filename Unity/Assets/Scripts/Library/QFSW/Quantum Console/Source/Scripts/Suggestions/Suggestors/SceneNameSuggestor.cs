using QFSW.QC.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace QFSW.QC.Suggestors
{
    public class SceneNameSuggestor : BasicCachedQcSuggestor<string>
    {
        protected override bool CanProvideSuggestions(SuggestionContext context, SuggestorOptions options)
        {
            return context.HasTag<Tags.SceneNameTag>();
        }

        protected override IQcSuggestion ItemToSuggestion(string sceneName)
        {
            return new RawSuggestion(sceneName, true);
        }

        protected override IEnumerable<string> GetItems(SuggestionContext context, SuggestorOptions options)
        {
            if (context.GetTag<Tags.SceneNameTag>().LoadedOnly)
            {
                return SceneUtilities.GetLoadedScenes()
                    .Select(x => x.name);
            }

            return SceneUtilities.GetAllSceneNames();
        }
    }
}