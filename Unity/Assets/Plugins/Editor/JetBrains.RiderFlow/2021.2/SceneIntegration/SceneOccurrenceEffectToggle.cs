using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.SceneHighlighting;
using UnityEngine.UIElements;

namespace JetBrains.RiderFlow.Since2021_2.SceneIntegration
{
    class SceneOccurrenceEffectToggle : RiderFlowEditorToolbarToggle
    {
        private SceneOccurrenceExplorerTool myTool;
        
        public SceneOccurrenceEffectToggle(SceneOccurrenceExplorerTool tool)
        {
            myTool = tool;
            name = "EffectToggle";
            icon = myTool.EffectIcon;
            tooltip = myTool.EffectTooltip;
            rawValue = myTool.OccurrencesHighLighted;
        }
        
        protected override void ValueChanged(ChangeEvent<bool> evt)
        {
            myTool.ToggleSceneHighlight();
            rawValue = (myTool.OccurrencesHighLighted);
        }
    }
}