using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.SceneHighlighting;
using UnityEditor.Overlays;

namespace JetBrains.RiderFlow.Since2021_2.SceneIntegration
{
    public class VisualElementToolbarDisplay : ISceneToolbarDisplay
    {

        private Overlay myToolbox;

        public VisualElementToolbarDisplay(Overlay toolbox)
        {
            myToolbox = toolbox;
        }

        public void OpenToolbox()
        {
            myToolbox.collapsed = false;
            myToolbox.displayed = true;
        }

        public void CloseToolbox()
        {
            myToolbox.displayed = false;
        }
    }
}