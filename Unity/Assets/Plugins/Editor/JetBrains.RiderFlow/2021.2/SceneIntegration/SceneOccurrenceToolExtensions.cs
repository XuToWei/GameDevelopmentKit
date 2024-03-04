using System.Collections.Generic;
using JetBrains.Lifetimes;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.SceneHighlighting;
using UnityEngine;
using UnityEngine.UIElements;

namespace JetBrains.RiderFlow.Since2021_2.SceneIntegration
{
    public static class SceneOccurrenceToolExtensions
    {
        public static IEnumerable<VisualElement> MakeVisualElements(this SceneOccurrenceExplorerTool tool, Lifetime lifetime, bool isHorizontal)
        {
            var popupButton = new ShowUsagesExplorerButton(lifetime, tool, isHorizontal);
            yield return popupButton;

            var prevButton = new RiderFlowEditorToolbarButton()
            {
                name = "PrevOccurrence",
                icon = tool.PreviousIcon,
                tooltip = tool.PreviousTooltip,
            };
            prevButton.SetOnClick(() =>
            {
                tool.SelectPrevious();
                popupButton.RefreshPresentation();
            });

            yield return prevButton;
            
            var nextButton = new RiderFlowEditorToolbarButton()
            {
                name = "NextOccurrence",
                icon = tool.NextIcon,
                tooltip = tool.NextTooltip,
            };
            nextButton.SetOnClick(() =>
            {
                tool.SelectNext();
                popupButton.RefreshPresentation();
            });
            yield return nextButton;
            
            var effectToggle = new SceneOccurrenceEffectToggle(tool);
            yield return effectToggle;
        }


    }
}