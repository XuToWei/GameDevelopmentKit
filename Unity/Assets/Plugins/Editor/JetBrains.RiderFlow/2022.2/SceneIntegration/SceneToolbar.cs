using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace JetBrains.RiderFlow.Since2022_2.SceneIntegration
{
    [Overlay(typeof(SceneView), RiderFlowToolbarConstants.ToolbarId, 
        "RiderFlow", true, defaultDisplay = true, 
        defaultDockZone = DockZone.Floating, defaultLayout = Layout.HorizontalToolbar)]
    public class SceneToolbar : Overlay, ICreateHorizontalToolbar, ICreateVerticalToolbar
    {
        private RiderFlowSceneToolbarRoot myRoot;

        public override void OnWillBeDestroyed()
        {
            myRoot.Destroy();
            base.OnWillBeDestroyed();
        }

        public override VisualElement CreatePanelContent() => CreateHorizontalToolbarContent();

        public OverlayToolbar CreateHorizontalToolbarContent()
        {
            if (myRoot == null)
                myRoot = new RiderFlowSceneToolbarRoot();
            return myRoot.MakeHorizontalToolbar();
        }

        public OverlayToolbar CreateVerticalToolbarContent()
        {
            if (myRoot == null)
                myRoot = new RiderFlowSceneToolbarRoot();
            return myRoot.MakeVerticalToolbar();
        }
    }
}