using JetBrains.Collections.Viewable;
using JetBrains.Lifetimes;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.SceneHighlighting;
using UnityEngine;
using UnityEngine.UIElements;

namespace JetBrains.RiderFlow.Since2021_2.SceneIntegration
{
    class ShowUsagesExplorerButton : RiderFlowEditorToolbarButton, IPresentationRefreshable
    {
        private SceneOccurrenceExplorerTool myTool;
        private Label myCounter;
        private VisualElement myCaption;

        public ShowUsagesExplorerButton(Lifetime lifetime, SceneOccurrenceExplorerTool tool, bool isHorizontal)
        {
            myTool = tool;
            text = isHorizontal ?  "Waiting for usages..." : "";
            tooltip = "Explore usages";
            
            SetOnClick(() =>
            {
                var windowPosition = this.CalculateDropdownPosition();
                tool.ShowOccurrencesInListView(windowPosition);
            });
            myCounter = new Label("")
            {
                style =
                {
                    display = isHorizontal ? DisplayStyle.Flex : DisplayStyle.None,
                    unityTextAlign = TextAnchor.MiddleRight
                }
            };
            Add(myCounter);
            
            tool.Session.Value.IsCompletedViewable.WhenTrue(lifetime, _ =>
            {
                RefreshPresentation();
            });
        }
        
        public void RefreshPresentation()
        {
            if (myCaption == null)
                myCaption = ElementAt(1);
            
            var isHorizontal = parent?.style.flexDirection == FlexDirection.Row;
            var content = myTool.GetUsageContentWithIcon();
            text = content.text;
            icon = (Texture2D)content.image;
            if (isHorizontal)
            {
                var guiStyle = new GUIStyle("label");
                var width = guiStyle.CalcSize(content).x;
                if (myTool.Session.HasValue() && myTool.Session.Value.IsCompleted)
                {
                    foreach (var occurrence in myTool.Session.Value.Occurrences)
                    {
                        var pair = SceneOccurrenceExplorerTool.GetUsageContentFor(occurrence);
                        var occurrenceContent = new GUIContent(pair.Item2.text, pair.Item1);
                        var newWidth = guiStyle.CalcSize(occurrenceContent).x;
                        if (width < newWidth) width = newWidth;
                    }
                }
                myCaption.style.width = width;
            }

            myCounter.text = myTool?.Counter;
            myCounter.style.display = isHorizontal ? DisplayStyle.Flex : DisplayStyle.None;
            myCaption.style.display = isHorizontal ? DisplayStyle.Flex : DisplayStyle.None;
        }

    }

    internal interface IPresentationRefreshable
    {
        void RefreshPresentation();
    }
}