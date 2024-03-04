using System.Collections.Generic;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.AltActions;
using UnityEditor;

namespace JetBrains.RiderFlow.Since2021_2.SceneIntegration
{
    public class NewToolbarCompositeAltActionProvider : CompositeAltActionProvider
    {
        public NewToolbarCompositeAltActionProvider(IEnumerable<IPartialAltActionProvider> tools)
            : base(tools)
        {
        }

        public override bool IsAvailable()
        {
            if (SceneView.lastActiveSceneView.TryGetOverlay(RiderFlowToolbarConstants.ToolbarId, out var toolbox))
            {
                return toolbox.displayed;
            }

            return false;
        }
    }
}