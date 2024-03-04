using JetBrains.Lifetimes;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.BookmarkExplorer;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.CameraExplorer;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace JetBrains.RiderFlow.Since2021_2.SceneIntegration
{
    public static class SceneViewToolsExtensions
    {
        public static Vector2 CalculateDropdownPosition(this VisualElement button)
        {
            var rect = button.worldBound;
            var windowPosition = (rect.position) + SceneView.lastActiveSceneView.position.position;
            if (button.parent.style.flexDirection == FlexDirection.Column)
            {
                windowPosition += new Vector2(rect.width, 0);
            }
            else
            {
                windowPosition += new Vector2(0, rect.height);
            }
                
            return windowPosition;
        }
        
        public static RiderFlowEditorToolbarButton MakeVisualElement(this CameraPresetCreationTool tool)
        {
            var button = new RiderFlowEditorToolbarButton()
            {
                name = "CameraCreation",
                icon = tool.Icon,
                tooltip = tool.ToolTip
            };
            button.SetOnClick(() =>
            {
                var windowPosition = button.CalculateDropdownPosition();

                CameraPresetCreationTool.OpenCreationWindow(SceneView.lastActiveSceneView, windowPosition);
            });
            return button;
        }

        public static EditorToolbarButton MakeVisualElement(this CameraPresetExplorerTool tool)
        {
            var button = new RiderFlowEditorToolbarButton()
            {
                name = "CameraPresetExplorer",
                icon = tool.Icon,
                tooltip = tool.ToolTip
            };
            button.SetOnClick(() =>
            {
                var windowPosition = button.CalculateDropdownPosition();
                tool.OpenPresetExplorerWindow(SceneView.lastActiveSceneView, windowPosition);
            });
            return button;
        }
        public static RiderFlowEditorToolbarToggle MakeVisualElement(this SceneSearchPopupTool tool,
            Lifetime lifetime)
        {
            var button = new SceneSearchToggle(tool, lifetime);
            return button;
        }

        public static RiderFlowEditorToolbarButton MakeVisualElement(this BookmarkExplorerTool tool)
        {
            var button = new RiderFlowEditorToolbarButton()
            {
                name = "CameraCreation",
                icon = tool.Icon,
                tooltip = tool.ToolTip
            };
            button.SetOnClick(() =>
            {
                var windowPosition = button.CalculateDropdownPosition();
                tool.OpenBookmarkExplorer(SceneView.lastActiveSceneView, windowPosition);
            });
            return button;
        }
    }
}