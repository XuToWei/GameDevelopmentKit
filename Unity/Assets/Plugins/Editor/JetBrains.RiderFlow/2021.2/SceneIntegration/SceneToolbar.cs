using UnityEditor;
using UnityEditor.Overlays;

namespace JetBrains.RiderFlow.Since2021_2.SceneIntegration
{
    [Overlay(typeof(SceneView), RiderFlowToolbarConstants.ToolbarId, "RiderFlow", true)]
    public class SceneToolbar : ToolbarOverlay
    {
        public SceneToolbar() : base("RiderFlow.ToolbarRoot")
        { }
    }
}