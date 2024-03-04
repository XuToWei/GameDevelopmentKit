using JetBrains.Lifetimes;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools;
using UnityEngine.UIElements;

namespace JetBrains.RiderFlow.Since2021_2.SceneIntegration
{
    public class SceneSearchToggle : RiderFlowEditorToolbarToggle
    {
        private readonly SceneSearchPopupTool myTool;
        

        public SceneSearchToggle(SceneSearchPopupTool tool, Lifetime lifetime)
        {
            myTool = tool;
            name = "CameraPresetExplorer";
            icon = tool.Icon;
            tooltip = tool.ToolTip;
            lifetime.OnTermination(() => myTool.CloseWindow());
        }
        
        protected override void ValueChanged(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                var windowPosition = this.CalculateDropdownPosition();
                var window = myTool.OpenSearchPopupAt(windowPosition, this);
                window.WindowLifetime.OnTermination(() =>
                    this.SetValueWithoutNotify(false));
            }
            else
            {
                myTool.CloseWindow();
            }
        }
    }
}